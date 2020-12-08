using DeConzToMqtt.Domain.DeConz.Apis;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DeConzToMqtt.Domain.DeConz.Tests.Apis
{
    public class DeConzLightsApiTest : TestBase
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetLights_Should_Return_Result()
        {
            var services = new ServiceCollection();
            services.AddRefitClient<IDeConzLightsApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri($"http://{Options.Host}:{Options.Port}/api"));

            var serviceProvider = services.BuildServiceProvider();
            var api = serviceProvider.GetRequiredService<IDeConzLightsApi>();

            var result = await api.GetLightsAsync(Options.ApiKey, default);

            result.Should().ContainKey("1", because: "deCONZ should return one device.");
        }
    }
}