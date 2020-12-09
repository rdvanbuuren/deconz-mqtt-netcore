using AutoFixture;
using DeConzToMqtt.App.DeConz;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Requests;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Websocket.Client;
using Websocket.Client.Models;
using Xunit;

namespace DeConzToMqq.App.Tests.DeConz
{
    public class WebsocketServiceTest
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public async Task StartAsync_Should_Setup_Connection_And_Listen_To_Observables()
        {
            var options = _fixture.Create<DeConzOptions>();
            var websocketPort = _fixture.Create<int>();

            var disposable = new Mock<IDisposable>();

            var reconnectionHappenedMock = new Mock<IObservable<ReconnectionInfo>>(MockBehavior.Strict);
            reconnectionHappenedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<ReconnectionInfo>>())).Returns(disposable.Object).Verifiable();
            var disconnectionHappenedMock = new Mock<IObservable<DisconnectionInfo>>(MockBehavior.Strict);
            disconnectionHappenedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<DisconnectionInfo>>())).Returns(disposable.Object).Verifiable();
            var messageReceivedMock = new Mock<IObservable<ResponseMessage>>(MockBehavior.Strict);
            messageReceivedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<ResponseMessage>>())).Returns(disposable.Object).Verifiable();

            var clientMock = new Mock<IWebsocketClient>(MockBehavior.Strict);
            clientMock.SetupSet(c => c.Name = It.IsAny<string>()).Callback<string>(value => value.Should().Be("deCONZ"));
            clientMock.SetupSet(c => c.ReconnectTimeout = It.IsAny<TimeSpan>()).Callback<TimeSpan?>(value => value.Should().Be(TimeSpan.FromMinutes(5)));
            clientMock.SetupSet(c => c.ErrorReconnectTimeout = It.IsAny<TimeSpan>()).Callback<TimeSpan?>(value => value.Should().Be(TimeSpan.FromSeconds(30)));
            clientMock.SetupGet(c => c.ReconnectionHappened).Returns(reconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.DisconnectionHappened).Returns(disconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.MessageReceived).Returns(messageReceivedMock.Object);
            clientMock.Setup(c => c.Start()).Returns(Task.CompletedTask).Verifiable();

            var clientFactoryMock = new Mock<IWebsocketClientFactory>(MockBehavior.Strict);
            clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<Uri>())).Returns(clientMock.Object).Verifiable();

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            mediatorMock.Setup(m => m.Send(It.IsAny<DeConzWebsocketRequest>(), default)).ReturnsAsync(websocketPort).Verifiable();

            var service = new WebsocketService(clientFactoryMock.Object, mediatorMock.Object, Options.Create(options), new NullLogger<WebsocketService>());

            // act
            await service.StartAsync(default);

            // assert
            mediatorMock.Verify(m => m.Send(It.IsAny<DeConzWebsocketRequest>(), default), Times.Once, "Needs to be called once.");
            clientFactoryMock.Verify(f => f.CreateClient(It.IsAny<Uri>()), Times.Once, "Needs to be called once.");
            reconnectionHappenedMock.Verify(o => o.Subscribe(It.IsAny<IObserver<ReconnectionInfo>>()), Times.Once, "Needs one subsccription.");
            disconnectionHappenedMock.Verify(o => o.Subscribe(It.IsAny<IObserver<DisconnectionInfo>>()), Times.Once, "Needs one subsccription.");
            messageReceivedMock.Verify(o => o.Subscribe(It.IsAny<IObserver<ResponseMessage>>()), Times.Once, "Needs one subsccription.");
            clientMock.Verify(c => c.Start(), Times.Once, "Websocket client should be started.");
        }
    }
}