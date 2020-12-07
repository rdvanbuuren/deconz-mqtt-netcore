using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace DeConzToMqtt.Domain.DeConz.Converters
{
    /// <summary>
    /// A super simple datetime converter that serializes "none" -> null
    /// Note: serializing won't work because of the NullValueHandling of Json.Net but isn't required for DeConz anyways.
    /// </summary>
    internal class NullableDateTimeConverter : IsoDateTimeConverter
    {
        private const string None = "none";

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.TokenType != JsonToken.String || reader.Value?.ToString().Equals(None, StringComparison.InvariantCultureIgnoreCase) != true
                ? base.ReadJson(reader, objectType, existingValue, serializer)
                : null;
        }
    }
}