using DeconzToMqtt.Deconz;
using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt.App.Handlers
{
    public class DeConzWebSocketRequestHandler : IRequestHandler<DeConzWebSocketRequest, int>
    {
        private readonly IDeConzConfigurationApi _deconzApi;
        private readonly DeConzOptions _options;
        private readonly ILogger<DeConzWebSocketRequestHandler> _logger;

        public DeConzWebSocketRequestHandler(IDeConzConfigurationApi deconzApi, IOptions<DeConzOptions> options, ILogger<DeConzWebSocketRequestHandler> logger)
        {
            _deconzApi = deconzApi;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<int> Handle(DeConzWebSocketRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting deCONZ configuration.");

            var config = await _deconzApi.GetConfigurationAsync(_options.ApiKey, cancellationToken);
            return config.WebSocketPort;
        }
    }
}