using DeconzToMqtt.Deconz.Websocket.Models;
using MediatR;

namespace DeconzToMqtt.Events
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