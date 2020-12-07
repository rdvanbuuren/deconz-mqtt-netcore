using System.Runtime.Serialization;

namespace DeConzToMqtt.Domain.DeConz.Dtos.WebSocket
{
    /// <summary>
    /// Specifies the message type
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// the message holds an event
        /// </summary>
        [EnumMember(Value = "event")]
        Event
    }
}