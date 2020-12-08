using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Dtos.Lights;
using DeConzToMqtt.Domain.DeConz.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeConzToMqtt.App.Handlers
{
    public class DeConzLightsRequestHandler : IRequestHandler<DeConzLightsRequest, ICollection<Light>>
    {
        private readonly IDeConzLightsApi _deConzLightsApi;
        private readonly DeConzOptions _options;
        private readonly ILogger<DeConzLightsRequestHandler> _logger;

        public DeConzLightsRequestHandler(IDeConzLightsApi deConzLightsApi, IOptions<DeConzOptions> options, ILogger<DeConzLightsRequestHandler> logger)
        {
            _deConzLightsApi = deConzLightsApi;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<ICollection<Light>> Handle(DeConzLightsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting deCONZ lights.");

            var lights = await _deConzLightsApi.GetLightsAsync(_options.ApiKey, cancellationToken);
            foreach (var light in lights)
            {
                light.Value.Id = light.Key;
            }

            return lights.Values;
        }
    }
}