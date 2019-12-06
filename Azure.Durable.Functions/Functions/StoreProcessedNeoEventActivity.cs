using Azure.Durable.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Azure.Durable.Functions.Functions
{
    public static class StoreProcessedNeoEventActivity
    {
        [FunctionName(nameof(StoreProcessedNeoEventActivity))]
        public static async Task Run(
            [ActivityTrigger] ProcessedNeoEvent processedNeoEvent,
            IBinder binder,
            ILogger log)
        {
            try
            {
                var blobPath = $"neo/processed/{processedNeoEvent.DateDetected:yyyyMMdd}/{processedNeoEvent.TorinoImpact}/{processedNeoEvent.Id}.json";
                var dynamicBlobBinding = new BlobAttribute(blobPath: blobPath) { Connection = "ProcessedNeoStorage" };
                using (var writer = await binder.BindAsync<TextWriter>(dynamicBlobBinding))
                {
                    await writer.WriteAsync(JsonConvert.SerializeObject(processedNeoEvent, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(StoreProcessedNeoEventActivity)}: {ex}");
                throw new FunctionFailedException(ex.Message);
            }
        }
    }
}
