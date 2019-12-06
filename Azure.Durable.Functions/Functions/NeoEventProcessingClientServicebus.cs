using System;
using System.Threading.Tasks;
using Azure.Durable.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure.Durable.Functions.Functions
{
    public static class NeoEventProcessingClientServicebus
    {
        [FunctionName(nameof(NeoEventProcessingClientServicebus))]
        public static async Task Run([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "NEOEventsTopic")]string message,
            [DurableClient]IDurableClient orchestrationClient,
            ILogger log)
        {
            try
            {
                var detectedNeoEvent = JsonConvert.DeserializeObject<DetectedNeoEvent>(message);
                log.LogInformation($"C# ServiceBus topic trigger function detected neo event with id: {detectedNeoEvent.Id}");
                
                var instanceId = await orchestrationClient.StartNewAsync(
                    "NeoEventProcessingOrchestrator",
                    detectedNeoEvent);
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(NeoEventProcessingClientServicebus)}: {ex}");
                throw new FunctionFailedException(ex.Message);
            }
        }
    }
}
