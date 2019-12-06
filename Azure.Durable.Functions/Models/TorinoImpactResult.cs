using Newtonsoft.Json;
using System;

namespace Azure.Durable.Functions.Models
{
    public class TorinoImpactResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("torino_impact")]
        public int TorinoImpact { get; set; }
    }
}
