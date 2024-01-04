using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;

namespace Contoso;

public class FormRecognizerService : IFormRecognizerService
{
    private DocumentModelAdministrationClient _adminClient;

    public FormRecognizerService(IConfiguration configuration)
    {

        AzureKeyCredential credential = new(configuration["FORM_RECOGNIZER_API_KEY"]);

        _adminClient = new DocumentModelAdministrationClient(new Uri(configuration["FORM_RECOGNIZER_ENDPOINT"]),
                                                             credential);
    }

    public async Task<BuildDocumentModelOperation> BuildDocumentModelAsync(string sas, string modelId)
    {
        return await _adminClient.BuildDocumentModelAsync(WaitUntil.Started,
                                                   new Uri(sas),
                                                   DocumentBuildMode.Template,
                                                   modelId);
    }
}