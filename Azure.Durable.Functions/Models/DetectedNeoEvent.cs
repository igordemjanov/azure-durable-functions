using Newtonsoft.Json;
using System;

namespace Azure.Durable.Functions.Models
{
    public class DetectedNeoEvent
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("distance")]
        public float Distance { get; set; }

        [JsonProperty("velocity")]
        public float Velocity { get; set; }

        [JsonProperty("diameter")]
        public float Diameter { get; set; }
    }
}
