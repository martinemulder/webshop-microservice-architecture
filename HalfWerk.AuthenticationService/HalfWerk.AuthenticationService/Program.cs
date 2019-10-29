using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HalfWerk.AuthenticationService
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        public static string Secret { get; set; } = "SuperGeheim. Komt zeker niet in git (≧∇≦)/";
        public static string PasswordSalt { get; set; } = "Koude friet is vies";

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
