using Azure;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modelUtility.Services
{
    public class FormRecognizerService : IFormRecognizerService
    {
        private DocumentModelAdministrationClient _adminClient;

        public FormRecognizerService(IConfiguration configuration)
        {
            string endpoint = configuration["FORM_RECOGNIZER_ENDPOINT"] 
                                ?? throw new ArgumentNullException("FORM_RECOGNIZER_ENDPOINT not found");

            AzureKeyCredential credential = new(configuration["FORM_RECOGNIZER_API_KEY"] 
                                                ?? throw new ArgumentNullException("FORM_RECOGNIZER_API_KEY not found"));

            _adminClient = new DocumentModelAdministrationClient(new Uri(endpoint), credential);
                                                                 
        }

        public async Task<BuildDocumentModelOperation> BuildDocumentModelAsync(string sas, string modelId, string description)
        {
            BuildDocumentModelOptions options = new BuildDocumentModelOptions() 
            { 
                Description = description
            };

            var operation = await _adminClient.BuildDocumentModelAsync(WaitUntil.Completed,
                                                                       new Uri(sas),
                                                                       DocumentBuildMode.Template,
                                                                       modelId);

            return operation;

        }        
    }
}
