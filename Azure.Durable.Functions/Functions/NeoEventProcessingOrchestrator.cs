using Azure.Durable.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Azure.Durable.Functions.Functions
{
    public static class NeoEventProcessingOrchestrator
    {
        [FunctionName(nameof(NeoEventProcessingOrchestrator))]
        public static async Task<ProcessedNeoEvent> Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            try
            {
                var detectedNeoEvent = context.GetInput<DetectedNeoEvent>();

                var kineticEnergy = await context.CallActivityWithRetryAsync<KineticEnergyResult>(
                    nameof(EstimateKineticEnergyActivity),
                    new RetryOptions(TimeSpan.FromSeconds(10), 5),
                    detectedNeoEvent);

                var impactProbability = await context.CallActivityWithRetryAsync<ImpactProbabilityResult>(
                    nameof(EstimateImpactProbabilityActivity),
                    new RetryOptions(TimeSpan.FromSeconds(10), 5),
                    detectedNeoEvent);

                TorinoImpactRequest torinoImpactRequest = new TorinoImpactRequest
                {
                    Id = detectedNeoEvent.Id,
                    KineticEnergyInMegatonTnt = kineticEnergy.KineticEnergyInMegatonTnt,
                    ImpactProbability = impactProbability.ImpactProbability
                };

                var torinoImpact = await context.CallActivityWithRetryAsync<TorinoImpactResult>(
                    nameof(EstimateTorinoImpactActivity),
                    new RetryOptions(TimeSpan.FromSeconds(10), 5),
                    torinoImpactRequest);

                ProcessedNeoEvent pne = new ProcessedNeoEvent(
                    detectedNeoEvent,
                    kineticEnergy.KineticEnergyInMegatonTnt,
                    impactProbability.ImpactProbability,
                    torinoImpact.TorinoImpact);

                if (pne.TorinoImpact >= 1)
                {
                    await context.CallActivityWithRetryAsync(
                    nameof(StoreProcessedNeoEventActivity),
                    new RetryOptions(TimeSpan.FromSeconds(10), 5),
                    pne);
                }

                return pne;
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(NeoEventProcessingOrchestrator)}: {ex}");
                throw new FunctionFailedException(ex.Message);
            }   
        }
    }
}
