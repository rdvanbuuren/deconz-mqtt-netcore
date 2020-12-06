using System.Runtime.Serialization;

namespace DeconzToMqtt.Deconz.Websocket.Models
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