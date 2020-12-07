using DeConzToMqtt.Domain.DeConz.Dtos.WebSocket;
using MediatR;

namespace DeConzToMqtt.Domain.DeConz.Events
{
    public class DeconzMessageEvent : INotification
    {
        public DeconzMessageEvent(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}