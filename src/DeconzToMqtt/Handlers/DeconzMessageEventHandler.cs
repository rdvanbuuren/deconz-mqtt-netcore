using DeconzToMqtt.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt.Handlers
{
    public class DeconzMessageEventHandler : INotificationHandler<DeconzMessageEvent>
    {
        private readonly ILogger<DeconzMessageEventHandler> _logger;

        public DeconzMessageEventHandler(ILogger<DeconzMessageEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DeconzMessageEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received DeconzMessageEvent");
            return Task.CompletedTask;
        }
    }
}