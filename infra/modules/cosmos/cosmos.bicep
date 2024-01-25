param location string
param suffix string

resource cosmos 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
  name: 'cosmos-${suffix}'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }    
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

resource db 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: '${cosmos.name}/sql/CopilotChat'
  properties: {
    resource: {
      id: 'CopilotChat'
    }
  }
}

resource chatmemorysources 'Microsoft.DocumentDB/databaseAccounts/apis/databases/containers@2016-03-31' = {
  name: '${cosmos.name}/sql/${db.name}/chatmemorysources'
  properties: {
    resource: {
      id: 'chatmemorysources'
      partitionKey: {
        paths: [
          '/chatId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'Consistent'
      }
    }        
  }
} 

resource chatmessages 'Microsoft.DocumentDB/databaseAccounts/apis/databases/containers@2016-03-31' = {
  name: '${cosmos.name}/sql/${db.name}/chatmessages'
  properties: {
    resource: {
      id: 'chatmessages'
      partitionKey: {
        paths: [
          '/chatId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'Consistent'
      }
    }        
  }
} 

resource chatparticipants 'Microsoft.DocumentDB/databaseAccounts/apis/databases/containers@2016-03-31' = {
  name: '${cosmos.name}/sql/${db.name}/chatparticipants'
  properties: {
    resource: {
      id: 'chatparticipants'
      partitionKey: {
        paths: [
          '/userId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'Consistent'
      }
    }        
  }
} 

resource chatsessions 'Microsoft.DocumentDB/databaseAccounts/apis/databases/containers@2016-03-31' = {
  name: '${cosmos.name}/sql/${db.name}/chatsessions'
  properties: {
    resource: {
      id: 'chatsessions'
      partitionKey: {
        paths: [
          '/id'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'Consistent'
      }
    }        
  }
} 

output cosmosName string = cosmos.name
