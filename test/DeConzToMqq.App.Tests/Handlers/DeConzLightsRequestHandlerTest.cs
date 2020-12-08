using AutoFixture;
using DeConzToMqtt.App.Handlers;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Dtos.Lights;
using DeConzToMqtt.Domain.DeConz.Requests;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DeConzToMqq.App.Tests.Handlers
{
    /// <summary>
    /// Unit tests for the <see cref="DeConzLightsRequestHandler"/> class.
    /// </summary>
    public class DeConzLightsRequestHandlerTest
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public async Task Handle_Should_Call_Api_GetLightsAsync()
        {
            // arrange
            var options = _fixture.Create<DeConzOptions>();
            var lights = _fixture.Create<IDictionary<string, Light>>();

            var mockApi = new Mock<IDeConzLightsApi>(MockBehavior.Strict);
            mockApi.Setup(api => api.GetLightsAsync(options.ApiKey, default)).ReturnsAsync(lights);

            var handler = new DeConzLightsRequestHandler(mockApi.Object, Options.Create(options), new NullLogger<DeConzLightsRequestHandler>());

            // act
            var result = await handler.Handle(new DeConzLightsRequest(), default);

            // assert
            result.Should().BeEquivalentTo(lights.Values);
            mockApi.Verify(api => api.GetLightsAsync(options.ApiKey, default), Times.Once);
        }
    }
}