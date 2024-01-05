param location string
param suffix string

resource search 'Microsoft.Search/searchServices@2023-11-01' = {
  name: 'search-${suffix}'
  location: location
  sku: {
    name: 'basic'
  }  
  properties: {
    publicNetworkAccess: 'enabled'
  }
}

output searchName string = search.name
