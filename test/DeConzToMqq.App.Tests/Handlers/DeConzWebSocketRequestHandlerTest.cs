using AutoFixture;
using DeconzToMqtt.App.Handlers;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Dtos.Configuration;
using DeConzToMqtt.Domain.DeConz.Requests;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DeConzToMqq.App.Tests.Handlers
{
    /// <summary>
    /// Unit tests for the <see cref="DeConzWebsocketRequestHandler"/> class.
    /// </summary>
    public class DeConzWebSocketRequestHandlerTest
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public async Task Handle_Should_Call_Api_GetConfigurationAsync()
        {
            // arrange
            var options = _fixture.Create<DeConzOptions>();
            var configuration = _fixture.Create<Configuration>();

            var mockApi = new Mock<IDeConzConfigurationApi>(MockBehavior.Strict);
            mockApi.Setup(api => api.GetConfigurationAsync(options.ApiKey, default)).ReturnsAsync(configuration);

            var handler = new DeConzWebsocketRequestHandler(mockApi.Object, Options.Create(options), new NullLogger<DeConzWebsocketRequestHandler>());

            // act
            var result = await handler.Handle(new DeConzWebsocketRequest(), default);

            // assert
            result.Should().Be(configuration.WebSocketPort, because: "The WebSocket port should be returned from the configuration");
            mockApi.Verify(api => api.GetConfigurationAsync(options.ApiKey, default), Times.Once);
        }
    }
}