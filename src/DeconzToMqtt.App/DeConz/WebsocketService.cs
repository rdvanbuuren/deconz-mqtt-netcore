using DeConzToMqtt.Domain.DeConz.Dtos.WebSocket;
using DeConzToMqtt.Domain.DeConz.Events;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace DeConzToMqtt.App.DeConz
{
    /// <summary>
    /// Websocket service that listens for incoming messages from deCONZ.
    /// </summary>
    public class WebsocketService : IHostedService, IDisposable
    {
        private readonly IWebsocketClientFactory _clientFactory;
        private readonly IMediator _mediator;
        private readonly ILogger<WebsocketService> _logger;

        private IWebsocketClient _client;
        private bool isDisposed;

        /// <summary>
        /// Creates a new instance of the <see cref="WebsocketService"/> class.
        /// </summary>
        /// <param name="clientFactory">Websocket client factory.</param>
        /// <param name="mediator">The mediator to send events.</param>
        /// <param name="options">The options used to connect to deCONZ.</param>
        /// <param name="logger">The logger for this class.</param>
        public WebsocketService(IWebsocketClientFactory clientFactory, IMediator mediator, ILogger<WebsocketService> logger)
        {
            _clientFactory = clientFactory;
            _mediator = mediator;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client = await _clientFactory.CreateClientAsync();

            _client.Name = "deCONZ";
            _client.ReconnectTimeout = TimeSpan.FromMinutes(5);
            _client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);

            _client.ReconnectionHappened.Subscribe(info =>
            {
                if (info.Type == ReconnectionType.Initial)
                {
                    _logger.LogInformation($"Connecting to websocket: {_client.Url}");
                }
                else
                {
                    _logger.LogInformation($"Reconnecting to websocket: {_client.Url} - cause: {info.Type}");
                }
            });

            _client.DisconnectionHappened.Subscribe(info =>
            {
                _logger.LogWarning($"Websocket got disconnected - cause: {info.Type}");
            });

            _client.MessageReceived.Subscribe(async msg =>
            {
                _logger.LogInformation($"Message received: {msg}");
                var message = JsonConvert.DeserializeObject<Message>(msg.Text);

                // emit event so mqtt service will pick it up.
                await _mediator.Publish(new DeConzMessageEvent(message), cancellationToken);
            });

            _logger.LogInformation("Starting websocket client...");
            await _client.Start();
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _client?.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "WebsocketService stopped.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    _client?.Dispose();
                    _client = null;
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}