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
    public class TorinoImpactEstimatorHttpTrigger
    {
        [FunctionName(nameof(TorinoImpactEstimatorHttpTrigger))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "estimate/torinoimpact")]
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
                    var torinoImpactRequest = JsonConvert.DeserializeObject<TorinoImpactRequest>(requestBody);
                    var torinoImpact = TorinoImpactManager.CalculateImpact(
                        torinoImpactRequest.KineticEnergyInMegatonTnt,
                        torinoImpactRequest.ImpactProbability);

                    result = new JsonResult(new TorinoImpactResult
                    {
                        Id = torinoImpactRequest.Id,
                        TorinoImpact = torinoImpact
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
                log.LogError($"Error in {nameof(TorinoImpactEstimatorHttpTrigger)}: {ex}"); 
                return new BadRequestObjectResult($"Error in {nameof(TorinoImpactEstimatorHttpTrigger)}: {ex}");
            }
        }
    }
}
