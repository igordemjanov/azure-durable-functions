using Azure.Durable.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.Durable.Functions.Functions
{
    public class EstimateImpactProbabilityActivity
    {
        private readonly HttpClient client;

        public EstimateImpactProbabilityActivity(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        [FunctionName(nameof(EstimateImpactProbabilityActivity))]
        public async Task<ImpactProbabilityResult> Run(
            [ActivityTrigger] DetectedNeoEvent neoEvent,
            ILogger log)
        {
            try
            {
                var impactProbabilityEndpoint = new Uri(Environment.GetEnvironmentVariable("ImpactProbabilityEndpoint"));
                var response = await client.PostAsJsonAsync(impactProbabilityEndpoint, neoEvent);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new FunctionFailedException(content);
                }
                var result = await response.Content.ReadAsAsync<ImpactProbabilityResult>();

                return result;
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(EstimateImpactProbabilityActivity)}: {ex}");
                throw new FunctionFailedException(ex.Message);
            }
        }
    }
}
