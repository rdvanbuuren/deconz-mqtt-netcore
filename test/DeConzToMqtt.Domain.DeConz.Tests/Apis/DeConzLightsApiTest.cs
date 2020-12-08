using DeConzToMqtt.Domain.DeConz.Apis;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DeConzToMqtt.Domain.DeConz.Tests.Apis
{
    /// <summary>
    /// Integration test for the <see cref="IDeConzLightsApi"/>
    /// </summary>
    public class DeConzLightsApiTest : TestBase
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetLights_Should_Return_Result()
        {
            // arrange
            var api = RestService.For<IDeConzLightsApi>(ApiUrl);

            // act
            var result = await api.GetLightsAsync(Options.ApiKey, default);

            // assert
            result.Should().NotBeNull()
                .And.HaveCountGreaterThan(0)
                .And.ContainKey("1", because: "deCONZ should return one device");
        }
    }
}