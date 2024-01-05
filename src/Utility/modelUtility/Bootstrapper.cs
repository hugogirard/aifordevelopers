using modelUtility.Services;

namespace modelUtility;

public class Bootstrapper : IBootstrapper
{
    private readonly ILogger<Bootstrapper> _logger;
    private readonly IFormRecognizerService _formRecognizerService;
    private readonly IStorageService _storageService;

    public Bootstrapper(ILogger<Bootstrapper> logger,
                        IFormRecognizerService formRecognizerService,
                        IStorageService storageService)
    {
        _logger = logger;
        _formRecognizerService = formRecognizerService;
        _storageService = storageService;
    }

    /// <summary>
    /// Create the model in Azure AI Document Intelligence
    /// </summary>    
    public async Task CreateModelDocument(string modelId, string description)
    {
        var sasContainer = _storageService.GetSasContainer();

        await _formRecognizerService.BuildDocumentModelAsync(sasContainer, modelId, description);
    }
}
