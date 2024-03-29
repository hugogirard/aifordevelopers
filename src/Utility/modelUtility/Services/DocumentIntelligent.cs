﻿using Azure;
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
    public class DocumentIntelligent : IDocumentIntelligent
    {
        private DocumentModelAdministrationClient _adminClient;

        public DocumentIntelligent(IConfiguration configuration)
        {
            string endpoint = configuration["FORM_RECOGNIZER_ENDPOINT"] 
                                ?? throw new ArgumentNullException("FORM_RECOGNIZER_ENDPOINT not found");

            AzureKeyCredential credential = new(configuration["FORM_RECOGNIZER_API_KEY"] 
                                                ?? throw new ArgumentNullException("FORM_RECOGNIZER_API_KEY not found"));

            _adminClient = new DocumentModelAdministrationClient(new Uri(endpoint), credential);
                                                                 
        }

        public async Task<BuildDocumentModelOperation> BuildDocumentModelAsync(Uri sas, string modelId, string description)
        {
            BuildDocumentModelOptions options = new BuildDocumentModelOptions() 
            { 
                Description = description,                
            };

            options.Tags.Add("custom", "true");

            var operation = await _adminClient.BuildDocumentModelAsync(WaitUntil.Completed,
                                                                       sas,
                                                                       DocumentBuildMode.Template,
                                                                       modelId,
                                                                       string.Empty, 
                                                                       options);

            return operation;

        }        
    }
}
