using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace AzureFunctionApp
{
    public static class AuthFunction
    {
        private static string clientId = "ce295dbb-20b4-4dfe-9aae-a5c93cae6ba0";
        private static string tenantId = "418ddeda-4cb4-493f-881b-69cd97e34fa5";
        private static string redirectUri = "https://testing001vk.azurewebsites.net.azurewebsites.net/api/AuthFunction";

        [FunctionName("AuthFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string[] scopes = new string[] { "User.Read" };
            var pca = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{418ddeda-4cb4-493f-881b-69cd97e34fa5}")
                .WithRedirectUri(redirectUri)
                .Build();

            try
            {
                var result = await pca.AcquireTokenInteractive(scopes).ExecuteAsync();
                return new OkObjectResult($"Access Token: {result.AccessToken}");
            }
            catch (MsalException ex)
            {
                log.LogError($"Authentication error: {ex.Message}");
                return new BadRequestObjectResult($"Authentication error: {ex.Message}");
            }
        }
    }
}
