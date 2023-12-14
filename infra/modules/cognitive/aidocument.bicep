param location string
param suffix string


resource formRecognizer 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: 'form-${suffix}'
  location: location
  kind: 'FormRecognizer'
  sku: {
    name: 'S0'
  }
  properties: {
    
  }
}
