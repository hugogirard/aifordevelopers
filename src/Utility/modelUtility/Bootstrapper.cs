using modelUtility.Services;

namespace modelUtility;

public class Bootstrapper : IBootstrapper
{
    private readonly ILogger<Bootstrapper> _logger;
    private readonly IDocumentIntelligent _formRecognizerService;
    private readonly IStorageService _storageService;
    private readonly IAISearchService _aiSearchService;

    public Bootstrapper(ILogger<Bootstrapper> logger,
                        IDocumentIntelligent formRecognizerService,
                        IStorageService storageService,
                        IAISearchService aiSearchService)
    {
        _logger = logger;
        _formRecognizerService = formRecognizerService;
        _storageService = storageService;
        _aiSearchService = aiSearchService;
    }

    /// <summary>
    /// Create the model in Azure AI Document Intelligence
    /// </summary>    
    public async Task CreateModelDocument(string modelId, string description)
    {
        var sasContainer = _storageService.GetSasContainer();

        await _formRecognizerService.BuildDocumentModelAsync(sasContainer, modelId, description);
    }

    public async Task CreateIndexingResources() 
    { 
        await _aiSearchService.CreateIndexingResources();
    }
}
