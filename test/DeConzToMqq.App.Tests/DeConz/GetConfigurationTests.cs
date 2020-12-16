using AutoFixture;
using DeConzToMqtt.App.DeConz;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Dtos.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DeConzToMqq.App.Tests.Handlers
{
    /// <summary>
    /// Unit tests for the <see cref="GetConfiguration"/> class.
    /// </summary>
    public class GetConfigurationTests
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public async Task Handler_Handle_Should_Call_Api_GetConfigurationAsync()
        {
            // arrange
            var options = _fixture.Create<DeConzOptions>();
            var configuration = _fixture.Create<Configuration>();

            var mockApi = new Mock<IDeConzConfigurationApi>(MockBehavior.Strict);
            mockApi.Setup(api => api.GetConfigurationAsync(options.ApiKey, default)).ReturnsAsync(configuration);

            var handler = new GetConfiguration.Handler(mockApi.Object, Options.Create(options), new NullLogger<GetConfiguration.Handler>());

            // act
            var result = await handler.Handle(new GetConfiguration.Request(), default);

            // assert
            result.Should().Be(configuration, because: "The configuration should be returned.");
            mockApi.Verify(api => api.GetConfigurationAsync(options.ApiKey, default), Times.Once);
        }
    }
}