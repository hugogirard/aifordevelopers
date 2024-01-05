param suffix string
param location string

resource aiService 'Microsoft.CognitiveServices/accounts@2023-10-01-preview' = {
  name: 'aiService-${suffix}'
  location: location
  kind: 'CognitiveServices'
  sku: {
    name: 'S0'    
  }
  properties: {
    customSubDomainName: 'aiService-${suffix}'
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      virtualNetworkRules: [        
      ]
    }
  }  
}
