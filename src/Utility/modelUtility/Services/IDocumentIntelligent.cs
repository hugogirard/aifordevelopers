namespace modelUtility.Services;

public interface IDocumentIntelligent
{
    Task<BuildDocumentModelOperation> BuildDocumentModelAsync(Uri sas, string modelId, string description);
}

