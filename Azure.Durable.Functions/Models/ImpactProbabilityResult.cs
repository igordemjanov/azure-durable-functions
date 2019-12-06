using Newtonsoft.Json;
using System;

namespace Azure.Durable.Functions.Models
{
    public class ImpactProbabilityResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("impact_probability")]
        public float ImpactProbability { get; set; }
    }
}
