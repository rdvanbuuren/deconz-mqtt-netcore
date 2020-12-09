using System;
using System.Diagnostics.CodeAnalysis;
using Websocket.Client;

namespace DeConzToMqtt.App.DeConz
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
        /// <param name="url">The url to pass to the client.</param>
        /// <returns></returns>
        public IWebsocketClient CreateClient(Uri url);
    }

    /// <summary>
    /// Factory that creates new <see cref="IWebsocketClient"/> implementations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class WebsocketClientFactory : IWebsocketClientFactory
    {
        /// <inheritdoc/>
        public IWebsocketClient CreateClient(Uri url) => new WebsocketClient(url);
    }
}