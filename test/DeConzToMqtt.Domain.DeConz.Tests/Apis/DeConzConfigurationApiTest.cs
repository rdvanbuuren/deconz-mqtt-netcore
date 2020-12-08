using DeConzToMqtt.Domain.DeConz.Apis;
using FluentAssertions;
using Refit;
using System.Threading.Tasks;
using Xunit;

namespace DeConzToMqtt.Domain.DeConz.Tests.Apis
{
    public class DeConzConfigurationApiTest : TestBase
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetLights_Should_Return_Result()
        {
            // arrange
            var api = RestService.For<IDeConzConfigurationApi>(ApiUrl);

            // act
            var result = await api.GetConfigurationAsync(Options.ApiKey, default);

            // assert
            result.Should().NotBeNull();
            result.WebSocketPort.Should().BeGreaterThan(0, because: "deCONZ should return websocket");
        }
    }
}