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

resource db 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-11-15' = {
  parent: cosmos
  name: 'CopilotChat'  
  properties: {
    resource: {
      id: 'CopilotChat'
    }
    options: {}
  }
}

resource chatmemorysources 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: db
  name: 'chatmemorysources'
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
    options: {}
  }
} 

resource chatmessages 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  name: 'chatmessages'
  parent: db
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
    options: {}
  }
} 

resource chatparticipants 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: db
  name: 'chatparticipants'
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
    options: {}
  }
} 

resource chatsessions 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-11-15' = {
  parent: db
  name: 'chatsessions'
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
    options: {}
  }
} 

output cosmosName string = cosmos.name
