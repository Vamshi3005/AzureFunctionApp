using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
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
            logger.LogInformation("AuthFunction triggered.");

            string[] scopes = new string[] { "User.Read" };
            var pca = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithRedirectUri(redirectUri)
                .Build();

            try
            {
                var result = await pca.AcquireTokenInteractive(scopes).ExecuteAsync();
                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteStringAsync($"Access Token: {result.AccessToken}");
                return response;
            }
            catch (MsalException ex)
            {
                logger.LogError($"Authentication error: {ex.Message}");
                var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await response.WriteStringAsync($"Authentication error: {ex.Message}");
                return response;
            }
        }
    }
}
