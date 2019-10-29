using HalfWerk.PcsBetaalService.DAL;
using HalfWerk.PcsBetaalService.DAL.DataMappers;
using HalfWerk.PcsBetaalService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minor.Nijn.Helpers;
using Minor.Nijn.WebScale;
using Minor.Nijn.WebScale.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace HalfWerk.PcsBetaalService
{
    [ExcludeFromCodeCoverage]
    public class Application
    {
        private readonly ManualResetEvent _stopEvent;

        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public Application(ILoggerFactory loggerFactory)
        {
            _stopEvent = new ManualResetEvent(false);

            _services = new ServiceCollection();
            _loggerFactory = loggerFactory;
        }

        private void ConfigureServices()
        {
            _services.AddSingleton(_loggerFactory);
            _services.AddTransient<BetaalContext>();
            _services.AddTransient<IBetalingDataMapper, BetalingDataMapper>();
            _services.AddTransient<IBestellingDataMapper, BestellingDataMapper>();
            _services.AddTransient<IBetalingVerwerkenService, BetalingVerwerkenService>();

            AutoMapperConfiguration.Configure();
        }

        private void ConfigureDatabase()
        {
            DbContextOptions<BetaalContext> options = new DbContextOptionsBuilder<BetaalContext>()
                .UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new InvalidOperationException())
                .Options;

            _services.AddSingleton(options);

            using (var bestelContext = new BetaalContext(options))
            {
                bestelContext.Database.EnsureDeleted();
                bestelContext.Database.EnsureCreated();
            }
        }

        private MicroserviceHostBuilder ConfigureNijn()
        {
            var busContext = _services.AddNijn(options =>
            {
                options.SetLoggerFactory(_loggerFactory);
                options.ReadFromEnvironmentVariables();
            });

            return _services.AddNijnWebScale(options =>
            {
                options.SetLoggerFactory(_loggerFactory);
                options.WithContext(busContext);
                options.UseConventions();
                options.ScanForExceptions();
            });
        }

        public void Run(string[] args)
        {
            Thread.Sleep(int.Parse(Environment.GetEnvironmentVariable("startup-delay-in-seconds")) * 1000);
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            ConfigureDatabase();
            ConfigureServices();

            var hostBuilder = ConfigureNijn();
            using (var host = hostBuilder.CreateHost())
            {
                host.StartListening(currentTimestamp);

                _stopEvent.WaitOne();
            }
        }
    }
}