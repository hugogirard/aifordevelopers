param location string
param suffix string

resource search 'Microsoft.Search/searchServices@2023-11-01' = {
  name: 'search-${suffix}'
  location: location
  sku: {
    name: 'free'
  }  
  properties: {
    publicNetworkAccess: 'enabled'
  }
}
