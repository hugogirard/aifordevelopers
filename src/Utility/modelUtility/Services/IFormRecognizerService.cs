namespace modelUtility.Services;

public interface IFormRecognizerService
{
    Task<BuildDocumentModelOperation> BuildDocumentModelAsync(Uri sas, string modelId, string description);
}

