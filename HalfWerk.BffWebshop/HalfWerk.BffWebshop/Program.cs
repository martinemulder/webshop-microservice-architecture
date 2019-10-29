using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace HalfWerk.BffWebshop
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private const string OutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message} (at {SourceContext}){NewLine}{Exception}";

        public static void Main(string[] args)
        {
            var logBuilder = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: OutputTemplate);
                      
            #if DEBUG
            logBuilder.MinimumLevel.Debug();
            #else
            logBuilder.MinimumLevel.Information();
            #endif

            Log.Logger = logBuilder.CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
