using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Net;
using System.Threading.Tasks;

namespace AzureFunctionApp
{
    public class AuthFunction
    {
        private readonly ILogger<AuthFunction> _logger;

        public AuthFunction(ILogger<AuthFunction> logger)
        {
            _logger = logger;
        }

        [Function("AuthFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("AuthFunction invoked.");

            string clientId = "ce295dbb-20b4-4dfe-9aae-a5c93cae6ba0";
            string tenantId = "418ddeda-4cb4-493f-881b-69cd97e34fa5";
            string redirectUri = "https://testing001vk.azurewebsites.net/api/AuthFunction";

            var pca = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithRedirectUri(redirectUri)
                .Build();

            try
            {
                var scopes = new[] { "User.Read" };
                var result = await pca.AcquireTokenInteractive(scopes).ExecuteAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Access Token: {result.AccessToken}");
                return response;
            }
            catch (MsalException ex)
            {
                _logger.LogError($"Authentication error: {ex.Message}");

                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteStringAsync($"Authentication error: {ex.Message}");
                return response;
            }
        }
    }
}
