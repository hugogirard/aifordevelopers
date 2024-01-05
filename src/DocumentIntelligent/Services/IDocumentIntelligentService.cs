
namespace Contoso
{
    public interface IDocumentIntelligentService
    {
        Task<List<DocumentModelSummary>> GetModels();

        Task DeleteModel(string id);
    }
}