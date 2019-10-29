using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using HalfWerk.CommonModels.DummyData;
using HalfWerk.DsKlantBeheer.DAL;
using HalfWerk.DsKlantBeheer.DAL.DataMappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minor.Nijn.Helpers;
using Minor.Nijn.WebScale;
using Minor.Nijn.WebScale.Helpers;

namespace HalfWerk.DsKlantBeheer
{
    [ExcludeFromCodeCoverage]
    public class Application
    {
        private readonly ManualResetEvent _stopEvent;

        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public Application(ILoggerFactory loggerFactory)
        {
            _stopEvent = new ManualResetEvent(false);

            _services = new ServiceCollection();
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Application>();
        }

        private void ConfigureServices()
        {
            _services.AddSingleton(_loggerFactory);

            _services.AddTransient<KlantContext, KlantContext>();
            _services.AddTransient<IKlantDataMapper, KlantDataMapper>();
        }

        private void ConfigureDatabase()
        {
            DbContextOptions<KlantContext> options = new DbContextOptionsBuilder<KlantContext>()
                .UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new InvalidOperationException())
                .Options;

            _services.AddSingleton(options);

            using (var context = new KlantContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Klanten.AddRange(DummyKlanten.Klanten);
                context.SaveChanges();
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

            ConfigureDatabase();
            ConfigureServices();

            var hostBuilder = ConfigureNijn();
            using (var host = hostBuilder.CreateHost())
            {
                host.StartListening();
                _stopEvent.WaitOne();
            }
        }
    }
}