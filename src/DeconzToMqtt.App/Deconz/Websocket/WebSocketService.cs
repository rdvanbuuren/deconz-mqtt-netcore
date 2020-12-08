using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Dtos.WebSocket;
using DeConzToMqtt.Domain.DeConz.Events;
using DeConzToMqtt.Domain.DeConz.Requests;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace DeconzToMqtt.Deconz.Websocket
{
    public class WebSocketService : IHostedService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WebSocketService> _logger;
        private readonly DeConzOptions _options;

        /// <summary>
        /// Creates a new instance of the <see cref="WebSocketService"/> class.
        /// </summary>
        /// <param name="logger">The logger for this class.</param>
        public WebSocketService(IMediator mediator, IOptions<DeConzOptions> options, ILogger<WebSocketService> logger)
        {
            _options = options.Value;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var websocketPort = await _mediator.Send(new DeConzWebSocketRequest(), cancellationToken);

            var url = new Uri($"ws://{_options.Host}:{websocketPort}");

            var client = new WebsocketClient(url)
            {
                Name = "deCONZ",
                ReconnectTimeout = TimeSpan.FromMinutes(5),
                ErrorReconnectTimeout = TimeSpan.FromSeconds(30)
            };

            client.ReconnectionHappened.Subscribe(info => _logger.LogInformation($"Reconnection happened, type: {info.Type}, url: {client.Url}"));
            client.DisconnectionHappened.Subscribe(info => _logger.LogWarning($"Disconnection happened, type: {info.Type}"));
            client.MessageReceived.Subscribe(msg => MessageReceived(msg, cancellationToken));

            _logger.LogDebug("Started websocket client");
            await client.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void MessageReceived(ResponseMessage message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Message received: {message}");
            var msg = JsonConvert.DeserializeObject<Message>(message.Text);

            // emit event so mqtt service will pick it up.
            _mediator.Publish(new DeConzMessageEvent(msg), cancellationToken);
        }
    }
}