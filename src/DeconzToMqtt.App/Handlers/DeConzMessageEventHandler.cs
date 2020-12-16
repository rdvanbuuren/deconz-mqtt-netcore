using DeConzToMqtt.Domain.DeConz.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt.App.Handlers
{
    // TODO: move to MQTT
    public class DeConzMessageEventHandler : INotificationHandler<DeConzMessageEvent>
    {
        private readonly ILogger<DeConzMessageEventHandler> _logger;

        public DeConzMessageEventHandler(ILogger<DeConzMessageEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DeConzMessageEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received DeconzMessageEvent");
            return Task.CompletedTask;
        }
    }
}