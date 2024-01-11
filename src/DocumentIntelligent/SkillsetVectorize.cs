using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DocumentIntelligent.Model;

namespace DocumentIntelligent;

public class SkillsetVectorize
{
    private readonly ILogger<SkillsetVectorize> _logger;
    private readonly string _openApiEndpoint;
    private readonly string _openApiKey;
    private readonly IHttpClientFactory _httpClientFactory;

    public SkillsetVectorize(ILoggerFactory loggerFactory,
                             IConfiguration configuration,
                             IHttpClientFactory httpClientFactory)
    {
        _logger = loggerFactory.CreateLogger<SkillsetVectorize>();

        // Text embedding model deployment name
        var deploymentName = configuration["TextEmbeddingDeploymentName"] ?? "text-embedding-ada-002";

        var openApiEndpoint = configuration["OpenApiEndpoint"];

        var openApiVersion = configuration["OpenApiVersion"];

        _openApiEndpoint = $"{openApiEndpoint}/openai/deployments/{deploymentName}/embeddings?api-version={openApiVersion}";
        
        _openApiKey = configuration["OpenApiKey"];

        _httpClientFactory = httpClientFactory;
    }

    [Function("Vectorize")]
    public async Task<HttpResponseData> VectorizeModel([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogDebug("AnalyzeModel function processed a request.");

        var data = await req.ReadFromJsonAsync<WebApiRequestVector>();

        _logger.LogDebug($"Request Input from Azure Search: {data}");

        if (data == null)
        {
            _logger.LogError("The request schema does not match expected schema.");
            return req.CreateBadRequestResponse("The request schema does not match expected schema.");
        }

        if (data.Values == null)
        {
            _logger.LogError("The request schema does not match expected schema. Could not find values array.");
            return req.CreateBadRequestResponse("The request schema does not match expected schema. Could not find values array.");
        }

        // Create a response object
        var response = new WebApiResponseVector();

        foreach (var record in data.Values)
        {
            _logger.LogDebug($"Record to process: {record}");

            if (record == null || record.RecordId == null) continue;

            OutputRecordEmbedding responseRecord = new OutputRecordEmbedding
            {
                RecordId = record.RecordId
            };

            try
            {
                // Call AzureOpenAI for vectorization
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("api-key", _openApiKey);
                httpClient.DefaultRequestHeaders.Add("Accept-Type", "application/json");

                //string serializedString = JsonSerializer.Serialize(record.Data);
                // Create a dynamic object with the property called input
                VectorInput input = new() 
                { 
                    Input = JsonSerializer.Serialize(record.Data)
                };
                                
                var openAiResponse = await httpClient.PostAsJsonAsync(_openApiEndpoint, input);

                if (openAiResponse.IsSuccessStatusCode) 
                {
                    var vectorData = await openAiResponse.Content.ReadFromJsonAsync<OpenAiResponse>();
                }
            }
            catch (Exception e)
            {
                // Something bad happened, log the issue.
                var error = new OutputRecordMessage
                {
                    Message = e.Message
                };

                responseRecord.Errors = new List<OutputRecordMessage>
                {
                    error
                };
            }
            finally
            {
                response.Values.Add(responseRecord);
            }
        }

        return req.CreateOkRequest(response);
    }
}
