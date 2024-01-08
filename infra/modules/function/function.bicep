param location string
param suffix string
param storageName string
param appInsightname string
param aiServiceName string

resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: storageName  
}

resource aiService 'Microsoft.CognitiveServices/accounts@2021-04-30' existing = {
  name: aiServiceName  
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightname
}

resource serverFarm 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: 'asp-${suffix}'
  location: location
  sku: {
    tier: 'Dynamic'
    name: 'Y1'
  }
}

resource function 'Microsoft.Web/sites@2020-06-01' = {
  name: 'func-${suffix}'
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: serverFarm.id    
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageName};AccountKey=${storage.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'                  
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageName};AccountKey=${storage.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'                  
        }
        {
          name: 'CONTAINERNAME'
          value: 'documents'
        }
        {
          name: 'STORAGECNXSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageName};AccountKey=${storage.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'                  
        }
        {
          name: 'DocumentIntelligentServiceEndpoint'
          value: aiService.properties.endpoint
        }                  
        {
          name: 'DocumentIntelligentServiceApiKey'
          value: aiService.listKeys().key1
        }                   
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: 'processorapp092'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED'
          value: '1'
        }
      ]
      cors: {
        allowedOrigins: ['https://portal.azure.com']
      }
    }
  }
}

output functionName string = function.name
