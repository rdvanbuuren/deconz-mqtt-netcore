using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt
{
    internal class Program
    {
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private static async Task Main(string[] args)
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

            Log.Information("Starting application...");

            Console.CancelKeyPress += (sender, e) =>
            {
                _cancellationTokenSource.Cancel();
                Console.WriteLine("Exiting...");
                Environment.Exit(0);
            };

            var cancellationToken = _cancellationTokenSource.Token;

            // run websocket task in background
            var websocketService = serviceProvider.GetService<IWebSocketService>();
            var websocketTask = Task.Run(() => websocketService.StartAsync(cancellationToken), cancellationToken);

            Log.Information("Press any key to stop application...");
            Task.WaitAll(websocketTask);
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