using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ApiClient> _logger;

        private const string ApiKey = "1570120947"; // TODO config

        public ApiClient(IDeconzConfigurationApi deconzApi, ILogger<ApiClient> logger)
        {
            _deconzApi = deconzApi;
            _logger = logger;
        }

        public async Task<int> GetWebSocketPortAsync()
        {
            var config = await _deconzApi.GetConfiguration(ApiKey);
            return config.WebSocketPort;
        }
    }
}