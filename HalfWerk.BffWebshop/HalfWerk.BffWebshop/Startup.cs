using AutoMapper;
using HalfWerk.BffWebshop.DAL;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Helpers;
using HalfWerk.BffWebshop.Services;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using HalfWerk.CommonModels.DummyData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minor.Nijn.Helpers;
using Minor.Nijn.RabbitMQBus;
using Minor.Nijn.WebScale.Helpers;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace HalfWerk.BffWebshop
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private readonly ILoggerFactory _loggerFactory;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Thread.Sleep(int.Parse(Environment.GetEnvironmentVariable("startup-delay-in-seconds")) * 1000);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register dependencies here
            services.AddSingleton(_loggerFactory);
            services.AddTransient<BffContext, BffContext>();
            services.AddTransient<IArtikelDataMapper, ArtikelDataMapper>();
            services.AddTransient<IKlantDataMapper, KlantDataMapper>();
            services.AddTransient<IBestellingDataMapper, BestellingDataMapper>();
            services.AddTransient<IMagazijnSessionDataMapper, MagazijnSessionDataMapper>();
            services.AddTransient<IJwtHelper, JwtHelper>();

            // Configure other services
            ConfigAutoMapper.Initialize();
            ConfigureDatabase(services);
            ConfigureNijn(services);
            AddSwagger(services);
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            DbContextOptions<BffContext> options = new DbContextOptionsBuilder<BffContext>()
                 .UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new InvalidOperationException())
                 .Options;

            services.AddSingleton(options);
            using (var context = new BffContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var klanten = Mapper.Map<List<Klant>>(DummyKlanten.Klanten);
                context.KlantEntities.AddRange(klanten);
                context.SaveChanges();
            }
        }

        private void ConfigureReplayEvents(IServiceCollection services)
        {
            var contextBuilder = new RabbitMQContextBuilder()
                .SetLoggerFactory(_loggerFactory)
                .WithConnectionTimeout(5000)
                .ReadFromEnvironmentVariables()
                .WithExchange(NameConstants.BffWebshopEventReplayExchange);

            using (var context = contextBuilder.CreateContext())
            using (var replayService = new EventReplayService(context, services, 5000))
            {
                replayService.StartEventReplay();
            }
        }
       
        private void ConfigureNijn(IServiceCollection services)
        {
            var busContext = services.AddNijn(options =>
            {
                options.SetLoggerFactory(_loggerFactory);
                options.ReadFromEnvironmentVariables();
            });

            var hostBuilder = services.AddNijnWebScale(options =>
            {
                options.SetLoggerFactory(_loggerFactory);
                options.WithContext(busContext);
                options.UseConventions();
                options.ScanForExceptions();
            });

            ThreadPool.QueueUserWorkItem(state =>
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                using (var host = hostBuilder.CreateHost())
                {
                    host.RegisterListeners();
                    ConfigureReplayEvents(services);
                    host.StartListening(timestamp);
                    _stopEvent.WaitOne();
                }
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BffWebshop WebAPI", Version = "v1" });
                c.EnableAnnotations();

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    Type = "apiKey",
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureSwashbuckle(app);

            app.UseMvc();
        }

        private void ConfigureSwashbuckle(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Artikel Service Commands API V1");
            });
        }
    }
}
