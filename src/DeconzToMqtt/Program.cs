using DeconzToMqtt.Deconz;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Threading;

namespace DeconzToMqtt
{
    internal class Program
    {
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .AddSingleton<IWebSocketService, WebSocketService>()
                .BuildServiceProvider();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            Log.Information("Starting application.");

            Console.CancelKeyPress += (sender, e) =>
            {
                Log.Information("Exiting...");
                e.Cancel = true;
                ExitEvent.Set();
            };

            var wss = serviceProvider.GetService<IWebSocketService>();
            wss.StartAsync();

            Log.Information("Press any key to stop application.");

            ExitEvent.WaitOne();

            Log.CloseAndFlush();
            Environment.Exit(0);
        }

        private static IConfiguration LoadConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}