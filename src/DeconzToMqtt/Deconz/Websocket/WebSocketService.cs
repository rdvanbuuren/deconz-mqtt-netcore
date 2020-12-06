using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Websocket.Client;

namespace DeconzToMqtt.Deconz.Websocket
{
    public interface IWebSocketService
    {
        Task StartAsync();
    }

    public class WebSocketService : IWebSocketService
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

        /// <inheritdoc/>
        public Task StartAsync()
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
            return client.Start();
        }

        private void MessageReceived(ResponseMessage message)
        {
            _logger.LogInformation($"Message received: {message}");
        }
    }
}