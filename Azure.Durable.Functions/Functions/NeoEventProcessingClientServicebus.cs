using System;
using Azure.Durable.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure.Durable.Functions.Functions
{
    public static class NeoEventProcessingClientServicebus
    {
        [FunctionName(nameof(NeoEventProcessingClientServicebus))]
        public static void Run([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "NEOEventsTopic")]string message, ILogger log)
        {
            var detectedNeoEvent = JsonConvert.DeserializeObject<DetectedNeoEvent>(message);
            log.LogInformation($"C# ServiceBus topic trigger function detected neo event with id: {detectedNeoEvent.Id}");
        }
    }
}
