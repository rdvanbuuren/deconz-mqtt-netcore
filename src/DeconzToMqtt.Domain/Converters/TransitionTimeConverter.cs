using Newtonsoft.Json;
using System;

namespace DeConzToMqtt.Domain.DeConz.Converters
{
    internal class TransitionTimeConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan?) || objectType == typeof(TimeSpan);
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                long value = (long)reader.Value;
                return (TimeSpan?)TimeSpan.FromMilliseconds(value * 100);
            }

            return null;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TimeSpan span;

            if (value == null)
            {
                writer.WriteValue(0);
                return;
            }

            if (value is TimeSpan?)
                span = ((TimeSpan?)value).Value;
            else
                span = (TimeSpan)value;

            writer.WriteValue((int?)(span.TotalSeconds * 10));
        }
    }
}