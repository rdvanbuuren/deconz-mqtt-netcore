using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DeConzToMqtt.Domain.DeConz.Dtos.WebSocket
{
    public class Message
    {
        [JsonProperty("t")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        [JsonProperty("e")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EventType? Event { get; set; }

        [JsonProperty("r")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ResourceType ResourceType { get; set; }

        public string Id { get; set; }

        public SensorState State { get; set; }

        public SensorConfig Config { get; set; }
    }
}