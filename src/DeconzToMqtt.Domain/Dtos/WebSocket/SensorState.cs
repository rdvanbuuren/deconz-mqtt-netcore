﻿using DeConzToMqtt.Domain.DeConz.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DeConzToMqtt.Domain.DeConz.Dtos.WebSocket
{
    [DataContract]
    public class SensorState
    {
        [JsonProperty("buttonevent")]
        public int? ButtonEvent { get; set; }

        [JsonProperty("gesture")]
        public int? Gesture { get; set; }

        [JsonProperty("dark")]
        public bool? Dark { get; set; }

        [JsonProperty("daylight")]
        public bool? Daylight { get; set; }

        [JsonProperty("flag")]
        public bool? Flag { get; set; }

        [JsonProperty("humidity")]
        public int? Humidity { get; set; }

        [JsonProperty("lastupdated")]
        [JsonConverter(typeof(NullableDateTimeConverter))]
        public DateTime? LastUpdated { get; set; }

        [JsonProperty("lux")]
        public long? Lux { get; set; }

        [JsonProperty("lightlevel")]
        public long? LightLevel { get; set; }

        [JsonProperty("open")]
        public bool? Open { get; set; }

        [JsonProperty("presence")]
        public bool? Presence { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("temperature")]
        public int? Temperature { get; set; }

        [JsonProperty("pressure")]
        public int? Pressure { get; set; }

        [JsonProperty("alarm")]
        public bool? Alarm { get; set; }

        [JsonProperty("carbonmonoxide")]
        public bool? CarbonMonoxide { get; set; }

        [JsonProperty("fire")]
        public bool? Fire { get; set; }

        [JsonProperty("lowbattery")]
        public bool? LowBattery { get; set; }

        [JsonProperty("tampered")]
        public bool? Tampered { get; set; }

        [JsonProperty("water")]
        public bool? Water { get; set; }

        [JsonProperty("vibration")]
        public bool? Vibration { get; set; }

        [JsonProperty("vibrationstrength")]
        public int? VibrationStrength { get; set; }

        [JsonProperty("tiltangle")]
        public int? TiltAngle { get; set; }

        [JsonProperty("orientation")]
        public IEnumerable<int> Orientation { get; set; }

        [JsonProperty("consumption")]
        public int? Consumption { get; set; }

        [JsonProperty("power")]
        public int? Power { get; set; }

        [JsonProperty("voltage")]
        public int? Voltage { get; set; }

        [JsonProperty("current")]
        public int? Current { get; set; }

        [JsonProperty("on")]
        public bool? On { get; set; }
    }
}