using DeConzToMqtt.Domain.DeConz.Dtos.WebSocket;
using MediatR;

namespace DeConzToMqtt.Domain.DeConz.Events
{
    public class DeConzMessageEvent : INotification
    {
        public DeConzMessageEvent(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}