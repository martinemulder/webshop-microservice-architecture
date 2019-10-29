using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using Minor.Nijn.Helpers;
using Minor.Nijn.WebScale;
using Minor.Nijn.WebScale.Helpers;
using HalfWerk.AuthenticationService.Entities;
using HalfWerk.AuthenticationService.DAL;
using HalfWerk.AuthenticationService.DAL.DataMappers;

namespace HalfWerk.AuthenticationService
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
            _services.AddTransient<AuthenticationContext, AuthenticationContext>();
            _services.AddTransient<ICredentialDataMapper, CredentialDataMapper>();
        }

        private void ConfigureDatabase()
        {
            DbContextOptions<AuthenticationContext> options = new DbContextOptionsBuilder<AuthenticationContext>()
                .UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new InvalidOperationException())
                .Options;

            _services.AddSingleton(options);

            using (var authenticationContext = new AuthenticationContext(options))
            {
                authenticationContext.Database.EnsureDeleted();
                authenticationContext.Database.EnsureCreated();

//#if DEBUG
                var dummyAccount = new Helper.DummyAccount(new CredentialDataMapper(authenticationContext));
                dummyAccount.CreateKlant();
                dummyAccount.CreateMagazijn();
                dummyAccount.CreateSales();
                dummyAccount.CreateEigenaar();
                dummyAccount.CreateHolyGrail();
//#endif            
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
