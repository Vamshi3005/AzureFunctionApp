using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureFunctionApp
{
    public static class AuthFunction
    {
        private static string clientId = "ce295dbb-20b4-4dfe-9aae-a5c93cae6ba0";
        private static string tenantId = "418ddeda-4cb4-493f-881b-69cd97e34fa5";
        private static string redirectUri = "https://testing001vk.azurewebsites.net/api/AuthFunction";

        [Function("AuthFunction")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("AuthFunction");
            logger.LogInformation("Processing a request.");

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await response.WriteStringAsync("Hello from Azure Function!");

            return response;
        }
    }
}
