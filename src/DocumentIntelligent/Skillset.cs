using Contoso;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DocumentIntelligent
{
    public class Skillset
    {
        private readonly ILogger<Skillset> _logger;
        private readonly IDocumentIntelligentService _documentIntelligent;
        private readonly string _modelId;
        private readonly IStorageSevice _storageService;

        public Skillset(ILogger<Skillset> logger,
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

            var data = await req.ReadFromJsonAsync<WebApiRequest>();
            
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
            var response = new WebApiResponse
            {
                Values = new List<OutputRecord>()
            };

            foreach (var record in data.Values)
            {
                if (record == null || record.RecordId == null) continue;

                OutputRecord responseRecord = new OutputRecord
                {
                    RecordId = record.RecordId
                };

                try
                {
                    Uri sas = _storageService.GetSasBlobUrl(record.Data.metadata_storage_name);

                    // Call Document Intelligent
                    OutputRecordData outputRecordData  = await _documentIntelligent.AnalyzeDocument(sas,
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
}
