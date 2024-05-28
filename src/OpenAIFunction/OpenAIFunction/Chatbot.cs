using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenAI.TextCompletion;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace OpenAIFunction
{
    public class Chatbot
    {
        private readonly ILogger<Chatbot> _logger;

        public Chatbot(ILogger<Chatbot> logger)
        {
            _logger = logger;
        }

        [Function("GetSQL")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            [TextCompletionInput("%PROMPT%", Model = "%CHAT_MODEL_DEPLOYMENT_NAME%")] TextCompletionResponse response)
        {
            HttpResponseData responseData = req.CreateResponse(HttpStatusCode.OK);
            await responseData.WriteStringAsync(response.Content);
            return responseData;
        }
    }
}
