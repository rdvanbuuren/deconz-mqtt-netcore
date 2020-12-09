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

namespace DeConzToMqtt.App.DeConz
{
    public class WebsocketService : IHostedService
    {
        private readonly IWebsocketClientFactory _clientFactory;
        private readonly IMediator _mediator;
        private readonly ILogger<WebsocketService> _logger;
        private readonly DeConzOptions _options;

        /// <summary>
        /// Creates a new instance of the <see cref="WebsocketService"/> class.
        /// </summary>
        /// <param name="clientFactory">Websocket client factory.</param>
        /// <param name="mediator">The mediator to send events.</param>
        /// <param name="options">The options used to connect to deCONZ.</param>
        /// <param name="logger">The logger for this class.</param>
        public WebsocketService(IWebsocketClientFactory clientFactory, IMediator mediator, IOptions<DeConzOptions> options, ILogger<WebsocketService> logger)
        {
            _options = options.Value;
            _clientFactory = clientFactory;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var websocketPort = await _mediator.Send(new DeConzWebsocketRequest(), cancellationToken);
            var url = new Uri($"ws://{_options.Host}:{websocketPort}");

            var client = _clientFactory.CreateClient(url);

            client.Name = "deCONZ";
            client.ReconnectTimeout = TimeSpan.FromMinutes(5);
            client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);

            client.ReconnectionHappened.Subscribe(info =>
            {
                if (info.Type == ReconnectionType.Initial)
                {
                    _logger.LogInformation($"Connecting to websocket: {client.Url}");
                }
                else
                {
                    _logger.LogInformation($"Reconnecting to websocket: {client.Url} - cause: {info.Type}");
                }
            });

            client.DisconnectionHappened.Subscribe(info =>
            {
                _logger.LogWarning($"Websocket got disconnected - cause: {info.Type}");
            });

            client.MessageReceived.Subscribe(async msg =>
            {
                await MessageReceivedAsync(msg, cancellationToken);
            });

            _logger.LogInformation("Starting websocket client...");
            await client.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(ResponseMessage message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Message received: {message}");
            var msg = JsonConvert.DeserializeObject<Message>(message.Text);

            // emit event so mqtt service will pick it up.
            await _mediator.Publish(new DeConzMessageEvent(msg), cancellationToken);
        }
    }
}