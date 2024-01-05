targetScope = 'subscription'

@description('The location of the resources to be deployed')
param location string

@description('The name of the resource group to be created')
param rgName string 

var suffix = toLower(uniqueString(subscription().id))

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}

module storage 'modules/storage/storage.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'storage'
  params: {
    location: location
    suffix: suffix
  }
}

// module formRecognizer 'modules/cognitive/aidocument.bicep' = {
//   scope: resourceGroup(rg.name)
//   name: 'formRecognizer'
//   params: {
//     location: location
//     suffix: suffix
//   }
// }

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
    formRecognizerName: formRecognizer.outputs.frmRecognizerName
    storageName: storage.outputs.storageName
  }
}

// module openAi 'modules/cognitive/openai.bicep' = {
//   scope: resourceGroup(rg.name)
//   name: 'openAi'
//   params: {
//     location: location
//     suffix: suffix
//   }
// }

output functionName string = function.outputs.functionName
