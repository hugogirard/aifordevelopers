using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contoso;

public class DocumentIntelligentService : IDocumentIntelligentService
{
    private readonly ILogger<DocumentIntelligentService> _logger;
    private readonly DocumentModelAdministrationClient _documentAdminClient;

    public DocumentIntelligentService(ILogger<DocumentIntelligentService> logger,
                                     IConfiguration configuration)
    {
        _logger = logger;
        string endpoint = configuration["DocumentIntelligentServiceEndpoint"];
        string apiKey = configuration["DocumentIntelligentServiceApiKey"];
        var credential = new AzureKeyCredential(apiKey);
        _documentAdminClient = new DocumentModelAdministrationClient(new Uri(endpoint), credential);
    }

    public async Task<List<DocumentModelSummary>> GetModels()
    {
        try
        {
            // List the models currently stored in the resource.
            AsyncPageable<DocumentModelSummary> models = _documentAdminClient.GetDocumentModelsAsync();
            List<DocumentModelSummary> documents = new();

            await foreach (DocumentModelSummary modelSummary in models)
            {
                if (modelSummary.Tags.ContainsKey("custom"))
                    documents.Add(modelSummary);
            }

            return documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new List<DocumentModelSummary>();
        }
    }

    public async Task DeleteModel(string id)
    {
        try
        {
            await _documentAdminClient.DeleteDocumentModelAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);            
        }
    }
}