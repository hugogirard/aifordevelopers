using Contoso;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DocumentIntelligent;

public class SkillsetAnalyzeModel
{
    private readonly ILogger<SkillsetAnalyzeModel> _logger;
    private readonly IDocumentIntelligentService _documentIntelligent;
    private readonly string _modelId;
    private readonly IStorageSevice _storageService;

    public SkillsetAnalyzeModel(ILogger<SkillsetAnalyzeModel> logger,
                                IDocumentIntelligentService documentIntelligent,
                                IConfiguration configuration,
                                IStorageSevice storageSevice)
    {
        _logger = logger;
        _documentIntelligent = documentIntelligent;
        _modelId = configuration["ModelId"] ?? "PurchaseOrder";
        _storageService = storageSevice;
    }

    [Function("AnalyzeModel")]
    public async Task<HttpResponseData> AnalyzeModel([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {

        _logger.LogDebug("AnalyzeModel function processed a request.");

        var data = await req.ReadFromJsonAsync<WebApiRequestDocumentIntelligent>();
        
        _logger.LogDebug($"Request Input from Azure Search: {data}");

        if (data == null)
        {
            _logger.LogError("The request schema does not match expected schema.");                
            return req.CreateBadRequestResponse("The request schema does not match expected schema.");
        }

        if (data.Values == null) 
        {
            _logger.LogError("The request schema does not match expected schema. Could not find values array.");
           return req.CreateBadRequestResponse("The request schema does not match expected schema. Could not find values array.");
        }


        // Create a response object
        var response = new WebApiResponseDocumentIntelligent
        {
            Values = new List<OutputRecordDocumentIntelligent>()
        };

        foreach (var record in data.Values)
        {
            _logger.LogDebug($"Record to process: {record}");

            if (record == null || record.RecordId == null) continue;

            OutputRecordDocumentIntelligent responseRecord = new OutputRecordDocumentIntelligent
            {
                RecordId = record.RecordId
            };

            try
            {
                Uri sas = _storageService.GetSasBlobUrl(record.Data.metadata_storage_name);

                _logger.LogDebug($"SAS URL: {sas}");

                // Call Document Intelligent
                OutputRecordDataDocumentIntelligent outputRecordData  = await _documentIntelligent.AnalyzeDocument(sas,
                                                                                                _modelId);

                responseRecord.Data = outputRecordData;
            }
            catch (Exception e)
            {
                // Something bad happened, log the issue.
                var error = new OutputRecordMessage
                {
                    Message = e.Message
                };

                responseRecord.Errors = new List<OutputRecordMessage>
                {
                    error
                };
            }
            finally
            {
                response.Values.Add(responseRecord);
            }
        }
    
        return req.CreateOkRequest(response);
    }
    
}
