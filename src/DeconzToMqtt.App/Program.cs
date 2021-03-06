﻿using DeConzToMqtt.App.DeConz.Websocket;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using Websocket.Client;

namespace DeconzToMqtt
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                Log.Information("Starting host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices)
                .UseSerilog();

        private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var deconzOptions = new DeConzOptions();
            hostContext.Configuration.GetSection("deCONZ").Bind(deconzOptions);

            services.Configure<DeConzOptions>(hostContext.Configuration.GetSection("deCONZ").Bind);

            services.AddScoped<HttpLoggingHandler>();

            services.AddRefitClient<IDeConzConfigurationApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri($"http://{deconzOptions.Host}:{deconzOptions.Port}/api"))
                .AddHttpMessageHandler<HttpLoggingHandler>();
            services.AddRefitClient<IDeConzLightsApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri($"http://{deconzOptions.Host}:{deconzOptions.Port}/api"))
                .AddHttpMessageHandler<HttpLoggingHandler>();

            services.AddSingleton<IWebsocketClientFactory, WebsocketClientFactory>();

            services.AddHostedService<WebsocketService>();

            services.AddMediatR(typeof(Program));
        }
    }
}