using Azure.Durable.Functions.Managers;
using Azure.Durable.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace Azure.Durable.Functions.Functions
{
    public class ProbabilityEstimatorHttpTrigger
    {
        [FunctionName(nameof(ProbabilityEstimatorHttpTrigger))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "estimate/impactprobability")]
            HttpRequest req,
            ILogger log)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return new BadRequestErrorMessageResult($"The request does not contain a valid {nameof(DetectedNeoEvent)} object.");
                }

                JsonResult result;
                try
                {
                    var neoEvent = JsonConvert.DeserializeObject<DetectedNeoEvent>(requestBody);
                    var probability = ImpactProbabilityManager.CalculateByDistance(neoEvent.Distance);
                    result = new JsonResult(
                        new ImpactProbabilityResult
                        {
                            Id = neoEvent.Id,
                            ImpactProbability = probability
                        });
                }
                catch (JsonSerializationException e)
                {
                    return new BadRequestErrorMessageResult(e.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                log.LogError($"Error in {nameof(ProbabilityEstimatorHttpTrigger)}: {ex}");
                return new BadRequestObjectResult($"Error in {nameof(ProbabilityEstimatorHttpTrigger)}: {ex}");
            }
        }
    }
}
