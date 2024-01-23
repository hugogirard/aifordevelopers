$resourceGroupName = "rg-ai-demo"

$deployment=$(az deployment group show --name webapp --resource-group $resourceGroupName -o json | ConvertFrom-Json)
$frontendUrl = "https://$($deployment.properties.outputs.webapiUrl.value)"
$webApiName = $deployment.properties.outputs.webapiName.value

$frontendName = "$webApiName-frontend"
$backendName = "$webApiName-backend"

$frontendIds = $(az ad app create --display-name $frontendName --query [id,appId] -o tsv)
$body = (@{
    "spa" = @{
        "redirectUris" = @(
            "$frontendUrl"
        )
    }
} | ConvertTo-Json -Compress -Depth 4) -Replace '"', '\"'
az rest --method PATCH --uri "https://graph.microsoft.com/v1.0/applications/$($frontendIds[0])" --headers "Content-Type=application/json" --body "$body"

$backendIds = $(az ad app create --display-name $backendName --query [id,appId] -o tsv)
$body = (@{
    "identifierUris" = @( "api://$($backendIds[1])" )
    "api" = @{
        "oauth2PermissionScopes" = @(
            @{
                "adminConsentDescription" = "Allow the application to access the chat backend on behalf of the signed-in user."
                "adminConsentDisplayName" = "Access chat backend"
                "id" = "1dffdad0-e9e8-461a-944d-29f3f9b57ebb"
                "isEnabled" = $true
                "type" = "User"
                "value" = "user_impersonation"
            }
        )    
    }
} | ConvertTo-Json -Compress -Depth 4) -Replace '"', '\"'
az rest --method PATCH --uri "https://graph.microsoft.com/v1.0/applications/$($backendIds[0])" --headers "Content-Type=application/json" --body "$body"

$body = (@{
    "api" = @{
        "preAuthorizedApplications" = @(
            @{
                "appId" = "$($frontendIds[1])"
                "delegatedPermissionIds" = @( "1dffdad0-e9e8-461a-944d-29f3f9b57ebb" )
            }
        )        
    }
} | ConvertTo-Json -Compress -Depth 4) -Replace '"', '\"'
az rest --method PATCH --uri "https://graph.microsoft.com/v1.0/applications/$($backendIds[0])" --headers "Content-Type=application/json" --body "$body"

$tenantId=$(az account show --query tenantId -o tsv)
az webapp config appsettings set --resource-group $resourceGroupName --name $webApiName --settings Authentication:Type="AzureAd" Authentication:AzureAd:TenantId="$tenantId" Authentication:AzureAd:ClientId="$($backendIds[1])" Authentication:AzureAd:Scopes="user_impersonation" Frontend:AadClientId="$($frontendIds[1])"