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


output storageName string = storageAccount.name
