using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Dtos.Lights;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeConzToMqtt.App.DeConz
{
    public static class GetLights
    {
        public class Request : IRequest<ICollection<Light>>
        {
        }

        public class Handler : IRequestHandler<Request, ICollection<Light>>
        {
            private readonly IDeConzLightsApi _deConzLightsApi;
            private readonly DeConzOptions _options;
            private readonly ILogger<Handler> _logger;

            public Handler(IDeConzLightsApi deConzLightsApi, IOptions<DeConzOptions> options, ILogger<Handler> logger)
            {
                _deConzLightsApi = deConzLightsApi;
                _options = options.Value;
                _logger = logger;
            }

            public async Task<ICollection<Light>> Handle(Request request, CancellationToken cancellationToken)
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
}