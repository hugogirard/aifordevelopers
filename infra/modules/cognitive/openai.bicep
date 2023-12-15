param location string
param suffix string

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
