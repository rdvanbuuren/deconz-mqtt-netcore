using Newtonsoft.Json;

namespace DeConzToMqtt.Domain.DeConz.Dtos.Configuration
{
    public class CreateApiKeyRequest
    {
        [JsonProperty("devicetype")]
        public string DeviceType { get; set; }
    }
}