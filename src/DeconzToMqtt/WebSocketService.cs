using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt
{
    public interface IWebSocketService
    {
        Task StartAsync(CancellationToken token);
    }

    // https://thecodegarden.net/websocket-client-dotnet
    public class WebSocketService : IWebSocketService
    {
        private readonly ILogger<WebSocketService> _logger;

        public WebSocketService(ILogger<WebSocketService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken token)
        {
            do
            {
                using var socket = new ClientWebSocket();
                try
                {
                    await socket.ConnectAsync(new Uri("ws://192.168.0.93:8443"), token);
                    await ReceiveAsync(socket, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Websocket problem.");
                    throw;
                }
            } while (!token.IsCancellationRequested);
        }

        private async Task ReceiveAsync(ClientWebSocket socket, CancellationToken token)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using var ms = new MemoryStream();
                do
                {
                    result = await socket.ReceiveAsync(buffer, token);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                ms.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(ms, Encoding.UTF8);
                _logger.LogInformation(await reader.ReadToEndAsync());
            } while (!token.IsCancellationRequested);
        }
    }
}