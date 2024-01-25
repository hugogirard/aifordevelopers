targetScope = 'subscription'

@description('The location of the resources to be deployed')
param location string

@description('The name of the resource group to be created')
param rgName string 

param completionModel string = 'gpt-35-turbo'

param embeddingModel string = 'text-embedding-ada-002'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}

var suffix = uniqueString(rg.id)

module storage 'modules/storage/storage.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'storage'
  params: {
    location: location
    suffix: suffix
  }
}

module aiService 'modules/cognitive/aiservices.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'aiService'
  params: {
    location: location
    suffix: suffix
  }
}

module search 'modules/cognitive/aisearch.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'search'
  params: {
    location: location
    suffix: suffix
  }
}

module insights 'modules/insights/insights.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'insights'
  params: {
    location: location
    suffix: suffix
  }
}

module function 'modules/function/function.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'function'
  params: {
    location: location
    suffix: suffix    
    appInsightname: insights.outputs.appInsightsName
    aiServiceName: aiService.outputs.aiServicesName
    storageName: storage.outputs.storageName
  }
}

module openAi 'modules/cognitive/openai.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'openAi'
  params: {
    location: location
    suffix: suffix
    completionModel: completionModel
    embeddingModel: embeddingModel
  }
}

module webapp 'modules/webapp/webapp.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'webapp'
  params: {
    location: location
    suffix: suffix
    appInsightsName: insights.outputs.appInsightsName
    aiServicesName: aiService.outputs.aiServicesName
    openAIName: openAi.outputs.openAIName
    completionModel: completionModel
    embeddingModel: embeddingModel
    searchName: search.outputs.searchName
    searchIndex: 'order'
    storageName: storage.outputs.storageName
  }
}

output functionName string = function.outputs.functionName
