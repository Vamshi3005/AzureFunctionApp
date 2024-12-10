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
        private static string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
        private static string tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
        private static string redirectUri = Environment.GetEnvironmentVariable("REDIRECT_URI");

        [FunctionName("AuthFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" }; // Use app-only scopes
            var pca = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithRedirectUri(redirectUri)
                .Build();

            try
            {
                // Use AcquireTokenByAuthorizationCode or AcquireTokenForClient for server-side apps
                var result = await pca.AcquireTokenForClient(scopes).ExecuteAsync();
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
