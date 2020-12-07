using DeconzToMqtt.Deconz;
using DeconzToMqtt.Deconz.Api;
using DeconzToMqtt.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt.Handlers
{
    public class DeconzWebSocketRequestHandler : IRequestHandler<DeconzWebSocketRequest, int>
    {
        private readonly IDeconzConfigurationApi _deconzApi;
        private readonly DeconzOptions _options;
        private readonly ILogger<DeconzWebSocketRequestHandler> _logger;

        public DeconzWebSocketRequestHandler(IDeconzConfigurationApi deconzApi, IOptions<DeconzOptions> options, ILogger<DeconzWebSocketRequestHandler> logger)
        {
            _deconzApi = deconzApi;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<int> Handle(DeconzWebSocketRequest request, CancellationToken cancellationToken)
        {
            var config = await _deconzApi.GetConfiguration(_options.ApiKey, cancellationToken);
            return config.WebSocketPort;
        }
    }
}