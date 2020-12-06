using DeconzToMqtt.Deconz;
using DeconzToMqtt.Deconz.Api;
using DeconzToMqtt.Deconz.Api.Requests;
using DeconzToMqtt.Deconz.Websocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt
{
    internal class Program
    {
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        private static async Task Main(string[] args)
        {
            //setup our DI
            var services = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .AddScoped<HttpLoggingHandler>()
                .AddScoped<IWebSocketService, WebSocketService>()
                .AddScoped<IApiClient, ApiClient>();

            services.AddRefitClient<IDeconzConfigurationApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://192.168.0.93:8080/api"))
                .AddHttpMessageHandler<HttpLoggingHandler>();

            var serviceProvider = services.BuildServiceProvider();

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

            //var wss = serviceProvider.GetService<IWebSocketService>();
            //wss.StartAsync();

            var client = serviceProvider.GetService<IDeconzConfigurationApi>();
            var created = await client.CreateApiKey(new CreateApiKeyRequest { DeviceType = "testabc" });

            await client.DeleteApiKey("1570120947", created.Result.Username);

            //await client.DeleteApiKey("1570120947", "48F131E33D");

            Log.Information("Press any key to stop application.");

            ExitEvent.WaitOne();

            Log.CloseAndFlush();
            Environment.Exit(0);
        }

        private static IConfiguration LoadConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                //.AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}