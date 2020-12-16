using AutoFixture;
using DeConzToMqtt.App.DeConz;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Dtos.Lights;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DeConzToMqq.App.Tests.DeConz
{
    /// <summary>
    /// Unit tests for the <see cref="GetLights"/> class.
    /// </summary>
    public class GetLightsHandlerTest
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public async Task Handler_Handle_Should_Call_Api_GetLightsAsync()
        {
            // arrange
            var options = _fixture.Create<DeConzOptions>();
            var lights = _fixture.Create<IDictionary<string, Light>>();

            var mockApi = new Mock<IDeConzLightsApi>(MockBehavior.Strict);
            mockApi.Setup(api => api.GetLightsAsync(options.ApiKey, default)).ReturnsAsync(lights);

            var handler = new GetLights.Handler(mockApi.Object, Options.Create(options), new NullLogger<GetLights.Handler>());

            // act
            var result = await handler.Handle(new GetLights.Request(), default);

            // assert
            result.Should().BeEquivalentTo(lights.Values);
            mockApi.Verify(api => api.GetLightsAsync(options.ApiKey, default), Times.Once);
        }
    }
}