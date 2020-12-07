using System.Runtime.Serialization;

namespace DeConzToMqtt.Domain.DeConz.Dtos.Lights
{
    /// <summary>
    /// Possible light effects
    /// </summary>
    public enum Effect
    {
        /// <summary>
        /// Stop current effect
        /// </summary>
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// Color loop
        /// </summary>
        [EnumMember(Value = "colorloop")]
        ColorLoop
    }
}