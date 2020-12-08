using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DeConzToMqtt.Domain.DeConz.Tests
{
    public abstract class TestBase
    {
        private static readonly Lazy<DeConzOptions> lazy = new Lazy<DeConzOptions>(() =>
        {
            var deconzOptions = new DeConzOptions();
            GetIConfigurationRoot().GetSection("deCONZ").Bind(deconzOptions);
            return deconzOptions;
        });

        public static DeConzOptions Options => lazy.Value;

        public static string ApiUrl => $"http://{Options.Host}:{Options.Port}/api";

        protected static IConfigurationRoot GetIConfigurationRoot() => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }
}