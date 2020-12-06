using DeconzToMqtt.Deconz.Websocket.Models;
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
        private readonly ILogger<WebSocketService> _logger;
        private readonly DeconzOptions _options;

        /// <summary>
        /// Creates a new instance of the <see cref="WebSocketService"/> class.
        /// </summary>
        /// <param name="logger">The logger for this class.</param>
        public WebSocketService(IOptions<DeconzOptions> options, ILogger<WebSocketService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var url = new Uri($"ws://{_options.Host}:{_options.WebsocketPort}");

            var client = new WebsocketClient(url)
            {
                Name = "deCONZ",
                ReconnectTimeout = TimeSpan.FromMinutes(5),
                ErrorReconnectTimeout = TimeSpan.FromSeconds(30)
            };

            client.ReconnectionHappened.Subscribe(info => _logger.LogInformation($"Reconnection happened, type: {info.Type}, url: {client.Url}"));
            client.DisconnectionHappened.Subscribe(info => _logger.LogWarning($"Disconnection happened, type: {info.Type}"));
            client.MessageReceived.Subscribe(MessageReceived);

            _logger.LogDebug("Started websocket client");
            return client.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void MessageReceived(ResponseMessage message)
        {
            _logger.LogInformation($"Message received: {message}");
            var msg = JsonConvert.DeserializeObject<Message>(message.Text);

            // TODO emit event
        }
    }
}