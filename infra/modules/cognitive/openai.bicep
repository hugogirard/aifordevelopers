param location string
param suffix string
param completionModel string
param embeddingModel string

resource openAI 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: 'openai-${suffix}'
  location: location
  sku: {
    name: 'S0'
  }
  kind: 'OpenAI'
  properties: {
    customSubDomainName: 'frmRecognizer-${suffix}'
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      virtualNetworkRules: [
        
      ]
    }
  }
}

resource openAI_completionModel 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: openAI
  name: completionModel
  sku: {
    name: 'Standard'
    capacity: 30
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: completionModel
    }
  }
}

resource openAI_embeddingModel 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: openAI
  name: embeddingModel
  sku: {
    name: 'Standard'
    capacity: 30
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: embeddingModel
    }
  }
  dependsOn: [// This "dependency" is to create models sequentially because the resource
    openAI_completionModel // provider does not support parallel creation of models properly.
  ]
}

output openAIName string = openAI.name
