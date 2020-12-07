using System.Runtime.Serialization;

namespace DeConzToMqtt.Domain.DeConz.Dtos.WebSocket
{
    /// <summary>
    /// Specifies the type of an event message
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// the message holds an event
        /// </summary>
        [EnumMember(Value = "changed")]
        Changed
    }
}