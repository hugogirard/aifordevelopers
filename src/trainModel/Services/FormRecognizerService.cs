using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;

namespace Contoso;

public class FormRecognizerService 
{
    private DocumentModelAdministrationClient _adminClient;

    public FormRecognizerService(IConfiguration configuration)
    {

        AzureKeyCredential credential = new(configuration["FORM_RECOGNIZER_API_KEY"]);

        _adminClient = new DocumentModelAdministrationClient(new Uri(configuration["FORM_RECOGNIZER_ENDPOINT"]),
                                                             credential);        
    }

    public void BuildDocumentModelAsync(string sas,string modelId)
    {
        DocumentContentSourceKind kind = new()
        DocumentContentSource source;
        source.
        _adminClient.BuildDocumentModelAsync(WaitUntil.Completed,sas,DocumentBuildMode.Template,modelId)
    }
}