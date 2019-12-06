using Azure.Durable.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Azure.Durable.Functions.Functions
{
    public class EstimateTorinoImpactActivity
    {
        private readonly HttpClient client;

        public EstimateTorinoImpactActivity(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
        }

        [FunctionName(nameof(EstimateTorinoImpactActivity))]
        public async Task<TorinoImpactResult> Run(
            [ActivityTrigger] TorinoImpactRequest torinoImpactRequest,
            ILogger log)
        {
            try
            {
                var torinoImpactEndpoint = new Uri(Environment.GetEnvironmentVariable("TorinoImpactEndpoint"));
                var response = await client.PostAsJsonAsync(torinoImpactEndpoint, torinoImpactRequest);
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new FunctionFailedException(content);
                }
                var result = await response.Content.ReadAsAsync<TorinoImpactResult>();

                return result;
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(EstimateTorinoImpactActivity)}: {ex}");
                throw new FunctionFailedException(ex.Message);
            }
        }
    }
}
