targetScope = 'subscription'

@description('The location of the resources to be deployed')
param location string

@description('The name of the resource group to be created')
param rgName string 

var resourceToken = toLower((subscription().id))

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}
