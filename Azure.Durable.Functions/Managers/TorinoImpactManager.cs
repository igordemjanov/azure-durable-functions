using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Durable.Functions.Managers
{
    public static class TorinoImpactManager
    {
        /// <summary>
        /// Loosely based on https://en.wikipedia.org/wiki/Torino_scale.
        /// This functions returns a fake Torino impact between 1 to 10.
        /// </summary>
        public static int CalculateImpact(
            float kineticEnergyInMegatonTnt,
            float impactProbability)
        {
            var log10Energy = Math.Log10(kineticEnergyInMegatonTnt);
            var result = Math.Round(log10Energy * impactProbability);

            if (result < 1) result = 0;
            if (result > 10) result = 10;

            return Convert.ToInt32(result);
        }
    }
}
