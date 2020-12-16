using DeConzToMqtt.Domain.DeConz;
using DeConzToMqtt.Domain.DeConz.Apis;
using DeConzToMqtt.Domain.DeConz.Dtos.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace DeConzToMqtt.App.DeConz
{
    public static class GetConfiguration
    {
        public class Request : IRequest<Configuration>
        {
        }

        public class Handler : IRequestHandler<Request, Configuration>
        {
            private readonly IDeConzConfigurationApi _deconzApi;
            private readonly DeConzOptions _options;
            private readonly ILogger<Handler> _logger;

            public Handler(IDeConzConfigurationApi deconzApi, IOptions<DeConzOptions> options, ILogger<Handler> logger)
            {
                _deconzApi = deconzApi;
                _options = options.Value;
                _logger = logger;
            }

            public Task<Configuration> Handle(Request request, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Getting deCONZ configuration.");

                return _deconzApi.GetConfigurationAsync(_options.ApiKey, cancellationToken);
            }
        }
    }
}