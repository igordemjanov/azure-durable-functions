using Azure.Durable.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.Durable.Functions.Functions
{
    public class EstimateKineticEnergyActivity
    {
        private readonly HttpClient client;

        public EstimateKineticEnergyActivity(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        [FunctionName(nameof(EstimateKineticEnergyActivity))]
        public async Task<KineticEnergyResult> Run(
            [ActivityTrigger] DetectedNeoEvent neoEvent,
            ILogger log)
        {
            try
            {
                var kineticEnergyEndpoint = new Uri(Environment.GetEnvironmentVariable("KineticEnergyEndpoint"));
                var response = await client.PostAsJsonAsync(kineticEnergyEndpoint, neoEvent);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new FunctionFailedException(content);
                }
                var result = await response.Content.ReadAsAsync<KineticEnergyResult>();

                return result;
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(EstimateKineticEnergyActivity)}: {ex}");
                throw new FunctionFailedException(ex.Message);
            }
        }
    }
}
