namespace modelUtility.Services;

public interface IFormRecognizerService
{
    Task<BuildDocumentModelOperation> BuildDocumentModelAsync(string sas, string modelId, string description);
}

