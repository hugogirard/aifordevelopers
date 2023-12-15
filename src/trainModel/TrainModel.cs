using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Contoso
{
    public class TrainModel
    {
        private readonly ILogger _logger;

        public TrainModel(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TrainModel>();
        }

        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json",typeof(Model),Description = "The model parameter", Required = false)]   
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DocumentModelDetails), Description = "The Custom Model definition")]
        [Function("TrainModel")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            if (req.Body is null)
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                errorResponse.WriteString("Request body is null.");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var model = await req.ReadFromJsonAsync<Model>() ?? new();

            var buildOptions = new BuildDocumentModelOptions();
            if (!string.IsNullOrEmpty(model.Description)) 
            {
                buildOptions.Description = model.Description;
            }            

            BuildDocumentModelOperation operation;     
            if (!string.IsNullOrEmpty(model.ModelId))
            {
                operation = await trainingClient.BuildDocumentModelAsync(WaitUntil.Completed, sas, DocumentBuildMode.Template, modelId, options: buildOptions);
            }
            else 
            {
                operation = await trainingClient.BuildDocumentModelAsync(WaitUntil.Completed, sas, DocumentBuildMode.Template, options: buildOptions);
            }              
        }
    }
}
