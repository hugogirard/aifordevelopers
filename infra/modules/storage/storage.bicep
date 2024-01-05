param location string
param suffix string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'str${suffix}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  name: 'default'
  parent: storageAccount
  properties: {}
}

resource containerTraining 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: 'trainingassets'
  parent: blobService
  properties: {}
}

resource containerDocument 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: 'documents'
  parent: blobService
  properties: {}
}

output storageName string = storageAccount.name
