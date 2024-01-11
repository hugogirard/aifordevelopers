
using DocumentIntelligent;

namespace Contoso
{
    public interface IDocumentIntelligentService
    {
        Task<List<DocumentModelSummary>> GetModels();

        Task DeleteModel(string id);

        Task<OutputRecordDataDocumentIntelligent> AnalyzeDocument(Uri documentUrl, string modelId);

        Task<DocumentModelDetails?> GetModelById(string modelId);
    }
}