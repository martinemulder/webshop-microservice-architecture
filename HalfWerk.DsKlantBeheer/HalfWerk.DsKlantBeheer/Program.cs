using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HalfWerk.DsKlantBeheer
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        private const string OutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message} (at {SourceContext}){NewLine}{Exception}";

        static void Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: OutputTemplate);

            #if DEBUG
            loggerConfiguration.MinimumLevel.Debug();
            #else
            loggerConfiguration.MinimumLevel.Information();
            #endif

            var loggerFactory = new LoggerFactory()
                .AddSerilog(loggerConfiguration.CreateLogger());

            new Application(loggerFactory).Run(args);
        }
    }
}
