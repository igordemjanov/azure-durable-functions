using Newtonsoft.Json;
using System;

namespace Azure.Durable.Functions.Models
{
    public class TorinoImpactRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("kinetic_energy_megaton_tnt")]
        public float KineticEnergyInMegatonTnt { get; set; }

        [JsonProperty("impact_probability")]
        public float ImpactProbability { get; set; }
    }
}
