using Newtonsoft.Json;

namespace DeconzToMqtt.Deconz.Api.Requests
{
    public class CreateApiKeyRequest
    {
        [JsonProperty("devicetype")]
        public string DeviceType { get; set; }
    }
}