using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Azure.Durable.Functions.Models;
using System.Net;

namespace Azure.Durable.Functions.Functions
{
    public static class NeoEventProcessingClientHttp
    {
        [FunctionName(nameof(NeoEventProcessingClientHttp))]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "start")] HttpRequestMessage message,
            [DurableClient]IDurableClient orchestrationClient,
            ILogger log)
        {
            try
            {
                var detectedNeoEvent = await message.Content.ReadAsAsync<DetectedNeoEvent>();
                var instanceId = await orchestrationClient.StartNewAsync("NeoEventProcessingOrchestrator", detectedNeoEvent);

                log.LogInformation($"HTTP started orchestration with ID {instanceId}.");

                return orchestrationClient.CreateCheckStatusResponse(message, instanceId);
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(NeoEventProcessingClientHttp)}: {ex}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
