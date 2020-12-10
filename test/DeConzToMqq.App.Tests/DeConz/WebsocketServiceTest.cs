using AutoFixture;
using DeConzToMqtt.App.DeConz;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Dtos.WebSocket;
using DeConzToMqtt.Domain.DeConz.Events;
using DeConzToMqtt.Domain.DeConz.Requests;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
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

            var disposableMock = new Mock<IDisposable>();
            var reconnectionHappenedMock = new Mock<IObservable<ReconnectionInfo>>(MockBehavior.Strict);
            reconnectionHappenedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<ReconnectionInfo>>())).Returns(disposableMock.Object).Verifiable();
            var disconnectionHappenedMock = new Mock<IObservable<DisconnectionInfo>>(MockBehavior.Strict);
            disconnectionHappenedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<DisconnectionInfo>>())).Returns(disposableMock.Object).Verifiable();
            var messageReceivedMock = new Mock<IObservable<ResponseMessage>>(MockBehavior.Strict);
            messageReceivedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<ResponseMessage>>())).Returns(disposableMock.Object).Verifiable();

            var clientMock = new Mock<IWebsocketClient>(MockBehavior.Strict);
            clientMock.SetupSet(c => c.Name = It.IsAny<string>()).Callback<string>(value => value.Should().Be("deCONZ"));
            clientMock.SetupSet(c => c.ReconnectTimeout = It.IsAny<TimeSpan>()).Callback<TimeSpan?>(value => value.Should().Be(TimeSpan.FromMinutes(5)));
            clientMock.SetupSet(c => c.ErrorReconnectTimeout = It.IsAny<TimeSpan>()).Callback<TimeSpan?>(value => value.Should().Be(TimeSpan.FromSeconds(30)));
            clientMock.SetupGet(c => c.ReconnectionHappened).Returns(reconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.DisconnectionHappened).Returns(disconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.MessageReceived).Returns(messageReceivedMock.Object);
            clientMock.Setup(c => c.Start()).Returns(Task.CompletedTask).Verifiable();

            var clientFactoryMock = new Mock<IWebsocketClientFactory>(MockBehavior.Strict);
            clientFactoryMock.Setup(f => f.CreateClientAsync(null)).ReturnsAsync(clientMock.Object).Verifiable();

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);

            var service = new WebsocketService(clientFactoryMock.Object, mediatorMock.Object, new NullLogger<WebsocketService>());

            // act
            await service.StartAsync(default);

            // assert
            clientFactoryMock.Verify(f => f.CreateClientAsync(null), Times.Once, "Needs to be called once.");
            reconnectionHappenedMock.Verify(o => o.Subscribe(It.IsAny<IObserver<ReconnectionInfo>>()), Times.Once, "Needs one subsccription.");
            disconnectionHappenedMock.Verify(o => o.Subscribe(It.IsAny<IObserver<DisconnectionInfo>>()), Times.Once, "Needs one subsccription.");
            messageReceivedMock.Verify(o => o.Subscribe(It.IsAny<IObserver<ResponseMessage>>()), Times.Once, "Needs one subsccription.");
            clientMock.Verify(c => c.Start(), Times.Once, "Websocket client should be started.");
        }

        [Fact]
        public async Task StartAsync_Should_ReceiveMessage_And_Publish()
        {
            var websocketPort = _fixture.Create<int>();
            var message = _fixture.Create<Message>();
            var messageJson = JsonConvert.SerializeObject(message);

            var disposable = new Mock<IDisposable>();
            var reconnectionHappenedMock = new Mock<IObservable<ReconnectionInfo>>();
            var disconnectionHappenedMock = new Mock<IObservable<DisconnectionInfo>>();
            var messageReceivedMock = new Mock<IObservable<ResponseMessage>>(MockBehavior.Strict);
            messageReceivedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<ResponseMessage>>()))
                .Callback((IObserver<ResponseMessage> observer) =>
                {
                    observer.OnNext(ResponseMessage.TextMessage(messageJson));
                })
                .Returns(disposable.Object);

            var clientMock = new Mock<IWebsocketClient>(MockBehavior.Strict);
            clientMock.SetupSet(c => c.Name = It.IsAny<string>());
            clientMock.SetupSet(c => c.ReconnectTimeout = It.IsAny<TimeSpan>());
            clientMock.SetupSet(c => c.ErrorReconnectTimeout = It.IsAny<TimeSpan>());
            clientMock.SetupGet(c => c.ReconnectionHappened).Returns(reconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.DisconnectionHappened).Returns(disconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.MessageReceived).Returns(messageReceivedMock.Object);
            clientMock.Setup(c => c.Start()).Returns(Task.CompletedTask);

            var clientFactoryMock = new Mock<IWebsocketClientFactory>(MockBehavior.Strict);
            clientFactoryMock.Setup(f => f.CreateClientAsync(null)).ReturnsAsync(clientMock.Object);

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            mediatorMock.Setup(m => m.Send(It.IsAny<DeConzWebsocketRequest>(), default)).ReturnsAsync(websocketPort);
            mediatorMock.Setup(m => m.Publish(It.IsAny<DeConzMessageEvent>(), default)).Returns(Task.CompletedTask).Verifiable();

            var service = new WebsocketService(clientFactoryMock.Object, mediatorMock.Object, new NullLogger<WebsocketService>());

            // act
            await service.StartAsync(default);

            // assert
            mediatorMock.Verify(m => m.Publish(It.IsAny<DeConzMessageEvent>(), default), Times.Once, "Needs to be called once.");
        }

        [Fact]
        public async Task StopAsync_Should_Stop_WebsocketClient()
        {
            // arrange
            var options = _fixture.Create<DeConzOptions>();

            var reconnectionHappenedMock = new Mock<IObservable<ReconnectionInfo>>();
            reconnectionHappenedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<ReconnectionInfo>>()));
            var disconnectionHappenedMock = new Mock<IObservable<DisconnectionInfo>>();
            disconnectionHappenedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<DisconnectionInfo>>()));
            var messageReceivedMock = new Mock<IObservable<ResponseMessage>>();
            messageReceivedMock.Setup(o => o.Subscribe(It.IsAny<IObserver<ResponseMessage>>()));

            var clientMock = new Mock<IWebsocketClient>();
            clientMock.SetupGet(c => c.ReconnectionHappened).Returns(reconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.DisconnectionHappened).Returns(disconnectionHappenedMock.Object);
            clientMock.SetupGet(c => c.MessageReceived).Returns(messageReceivedMock.Object);
            clientMock.Setup(c => c.Start()).Returns(Task.CompletedTask).Verifiable();
            clientMock.Setup(c => c.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "WebsocketService stopped.")).Verifiable();

            var clientFactoryMock = new Mock<IWebsocketClientFactory>(MockBehavior.Strict);
            clientFactoryMock.Setup(f => f.CreateClientAsync(null)).ReturnsAsync(clientMock.Object);

            var mediatorMock = new Mock<IMediator>(MockBehavior.Strict);

            var service = new WebsocketService(clientFactoryMock.Object, mediatorMock.Object, new NullLogger<WebsocketService>());

            // act
            await service.StartAsync(default);
            await service.StopAsync(default);

            // assert
            clientMock.Verify(c => c.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "WebsocketService stopped."), Times.Once);
        }
    }
}