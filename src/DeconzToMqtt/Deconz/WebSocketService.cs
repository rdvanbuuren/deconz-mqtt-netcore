using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Websocket.Client;

namespace DeconzToMqtt.Deconz
{
    public interface IWebSocketService
    {
        Task StartAsync();
    }

    public class WebSocketService : IWebSocketService
    {
        private readonly ILogger<WebSocketService> _logger;

        public WebSocketService(ILogger<WebSocketService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync()
        {
            var url = new Uri("ws://192.168.0.93:8443");

            var client = new WebsocketClient(url)
            {
                Name = "deCONZ",
                ReconnectTimeout = TimeSpan.FromMinutes(5),
                ErrorReconnectTimeout = TimeSpan.FromSeconds(30)
            };

            client.ReconnectionHappened.Subscribe(info => _logger.LogInformation($"Reconnection happened, type: {info.Type}, url: {client.Url}"));
            client.DisconnectionHappened.Subscribe(info => _logger.LogWarning($"Disconnection happened, type: {info.Type}"));
            client.MessageReceived.Subscribe(msg => _logger.LogInformation($"Message received: {msg}"));
            return client.Start();
        }
    }
}