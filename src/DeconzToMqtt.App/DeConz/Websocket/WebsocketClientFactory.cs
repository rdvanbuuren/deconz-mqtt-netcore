using DeConzToMqtt.Domain.DeConz;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Websocket.Client;

namespace DeConzToMqtt.App.DeConz.Websocket
{
    /// <summary>
    /// Contract for the <see cref="WebsocketClientFactory"/> class.
    /// </summary>
    public interface IWebsocketClientFactory
    {
        /// <summary>
        /// Creates a new <see cref="IWebsocketClient"/> implementation for the given <paramref name="url"/>.
        /// Optionally configures the client via <paramref name="config"/>.
        /// </summary>
        /// <param name="config">Configures the <see cref="WebsocketClient"/>.</param>
        /// <returns></returns>
        public Task<IWebsocketClient> CreateClientAsync(Action<WebsocketClient> config = null);
    }

    /// <summary>
    /// Factory that creates new <see cref="IWebsocketClient"/> implementations.
    /// </summary>
    public class WebsocketClientFactory : IWebsocketClientFactory
    {
        private readonly Lazy<Task<Uri>> _url;

        public WebsocketClientFactory(IMediator mediator, IOptions<DeConzOptions> options)
        {
            _url = new Lazy<Task<Uri>>(async () =>
            {
                var config = await mediator.Send(new GetConfiguration.Request());
                return new Uri($"ws://{options.Value.Host}:{config.WebSocketPort}");
            });
        }

        /// <inheritdoc/>
        public async Task<IWebsocketClient> CreateClientAsync(Action<WebsocketClient> config = null)
        {
            var client = new WebsocketClient(await _url.Value);
            config?.Invoke(client);
            return client;
        }
    }
}