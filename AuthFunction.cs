using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace AzureFunctionApp
{
    public class AuthFunction
    {
        private readonly ILogger _logger;
        private static readonly string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
        private static readonly string tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
        private static readonly string redirectUri = Environment.GetEnvironmentVariable("REDIRECT_URI");

        public AuthFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AuthFunction>();
        }

        [Function("AuthFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("Processing request in AuthFunction...");

            string[] scopes = new[] { "https://graph.microsoft.com/.default" };
            var pca = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .WithRedirectUri(redirectUri)
                .Build();

            try
            {
                var result = await pca.AcquireTokenForClient(scopes).ExecuteAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Access Token: {result.AccessToken}");
                return response;
            }
            catch (MsalException ex)
            {
                _logger.LogError($"Authentication error: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync($"Authentication error: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
