using Azure;
using DocumentIntelligent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contoso;

public class DocumentIntelligentService : IDocumentIntelligentService
{
    private readonly ILogger<DocumentIntelligentService> _logger;
    private readonly DocumentModelAdministrationClient _documentAdminClient;
    private readonly DocumentAnalysisClient _documentAnalysisClient;

    public DocumentIntelligentService(ILogger<DocumentIntelligentService> logger,
                                     IConfiguration configuration)
    {
        _logger = logger;
        string endpoint = configuration["DocumentIntelligentServiceEndpoint"];
        string apiKey = configuration["DocumentIntelligentServiceApiKey"];        
        var credential = new AzureKeyCredential(apiKey);
        _documentAdminClient = new DocumentModelAdministrationClient(new Uri(endpoint), credential);
        _documentAnalysisClient = new DocumentAnalysisClient(new Uri(endpoint), credential);
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

    public async Task<DocumentModelDetails?> GetModelById(string modelId) 
    {
        try
        {
            var response = await _documentAdminClient.GetDocumentModelAsync(modelId);

            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
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

    public async Task<OutputRecordData> AnalyzeDocument(Uri documentUrl, string modelId)
    {
        try
        {
            OutputRecordData outputRecordData = new();
            AnalyzeDocumentOperation operation = await _documentAnalysisClient.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, modelId, documentUrl);
            AnalyzeResult result = operation.Value;

            var fieldActions = new Dictionary<string, Action<string>>
            {
                    { "PurchaseOrderNumber", value => outputRecordData.purchaseOrderNumber = value },
                    { "Merchant", value => outputRecordData.Merchant = value },
                    { "PhoneNumber", value => outputRecordData.PhoneNumber = value },
                    { "Website", value => outputRecordData.Website = value },
                    { "Email", value => outputRecordData.Email = value },
                    { "DatedAs", value => outputRecordData.DatedAs = value },
                    { "ShippedToVendorName", value => outputRecordData.ShippedToVendorName = value },
                    { "ShippedToCompanyName", value => outputRecordData.ShippedToCompanyName = value },
                    { "ShippedToCompanyAddress", value => outputRecordData.ShippedToCompanyAddress = value },
                    { "ShippedToCompanyPhoneNumber", value => outputRecordData.ShippedToCompanyPhoneNumber = value },
                    { "ShippedFromCompanyName", value => outputRecordData.ShippedFromCompanyName = value },
                    { "ShippedFromCompanyAddress", value => outputRecordData.ShippedFromCompanyAddress = value },
                    { "ShippedFromCompanyPhoneNumber", value => outputRecordData.ShippedFromCompanyPhoneNumber = value },
                    { "ShippedFromName", value => outputRecordData.ShippedFromName = value },
                    { "Subtotal", value => outputRecordData.Subtotal = value },
                    { "Tax", value => outputRecordData.Tax = value },
                    { "Total", value => outputRecordData.Total = value },
                    { "Signature", value => outputRecordData.Signature = value }
            };

            AnalyzedDocument document = result.Documents.First();

            foreach (KeyValuePair<string, DocumentField> fieldKvp in document.Fields)
            {
                string fieldName = fieldKvp.Key;
                DocumentField field = fieldKvp.Value;

                if (fieldActions.TryGetValue(fieldName, out var action))
                {
                    action(field.Value.AsString());
                }
            }

            var table = result.Tables.SingleOrDefault(x => x.Cells.Any(x => x.Content == "Details"));

            // Get purchase order items
            if (table != null) 
            {
                int rowIndex = 1;
                while (true) 
                {
                    var cells = table.Cells.Where(x => x.RowIndex == rowIndex);
                    if (cells.Count() == 0) break;

                    var itemPurchased = new ItemPurchased
                    {
                        Detail = cells.SingleOrDefault(x => x.ColumnIndex == 0)?.Content,
                        Quantity = cells.SingleOrDefault(x => x.ColumnIndex == 1)?.Content,
                        UnitPrice = cells.SingleOrDefault(x => x.ColumnIndex == 2)?.Content,
                        Total = cells.SingleOrDefault(x => x.ColumnIndex == 3)?.Content
                    };

                    // Here we return only if all fields in the table are filled
                    if (!string.IsNullOrEmpty(itemPurchased.Detail) &&
                        !string.IsNullOrEmpty(itemPurchased.Quantity) &&
                        !string.IsNullOrEmpty(itemPurchased.UnitPrice) &&
                        !string.IsNullOrEmpty(itemPurchased.Total))
                    {
                        outputRecordData.ItemsPurchased.Add(itemPurchased);
                    }

                    rowIndex++;
                }
            }

            return outputRecordData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
