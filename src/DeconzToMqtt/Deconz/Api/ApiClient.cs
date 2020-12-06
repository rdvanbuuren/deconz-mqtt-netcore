using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt.Deconz.Api
{
    public interface IApiClient
    {
        Task<int> GetWebSocketPortAsync();
    }

    public class ApiClient : IApiClient
    {
        private readonly IDeconzConfigurationApi _deconzApi;
        private readonly DeconzOptions _options;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(IDeconzConfigurationApi deconzApi, IOptions<DeconzOptions> options, ILogger<ApiClient> logger)
        {
            _deconzApi = deconzApi;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<int> GetWebSocketPortAsync()
        {
            _logger.LogInformation("Getting websocket port from deCONZ config.");

            var config = await _deconzApi.GetConfiguration(_options.ApiKey);
            return config.WebSocketPort;
        }
    }
}