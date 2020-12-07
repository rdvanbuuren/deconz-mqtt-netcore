using System.Runtime.Serialization;

namespace DeConzToMqtt.Domain.DeConz.Dtos.WebSocket
{
    /// <summary>
    /// Type of resource to which the message belongs
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// the id field refers to a sensor resource
        /// </summary>
        [EnumMember(Value = "sensors")]
        Sensor,

        /// <summary>
        /// the id field refers to a light resource
        /// </summary>
        [EnumMember(Value = "lights")]
        Light,

        /// <summary>
        /// the id field refers to a group resource
        /// </summary>
        [EnumMember(Value = "groups")]
        Group,

        /// <summary>
        /// the id field refers to a scene resource
        /// </summary>
        [EnumMember(Value = "scenes")]
        Scene
    }
}