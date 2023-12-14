targetScope = 'subscription'

@description('The location of the resources to be deployed')
param location string

@description('The name of the resource group to be created')
param rgName string 

var suffix = toLower((subscription().id))

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

module formRecognizer 'modules/cognitive/aidocument.bicep' = {
  scope: resourceGroup(rg.name)
  name: 'formRecognizer'
  params: {
    location: location
    suffix: suffix
  }
}
