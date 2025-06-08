targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''

var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}
module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
    principalId: principalId
  }
}

module azure_storage 'azure-storage/azure-storage.module.bicep' = {
  name: 'azure-storage'
  scope: rg
  params: {
    location: location
  }
}
module web_identity 'web-identity/web-identity.module.bicep' = {
  name: 'web-identity'
  scope: rg
  params: {
    location: location
  }
}
module web_roles_azure_storage 'web-roles-azure-storage/web-roles-azure-storage.module.bicep' = {
  name: 'web-roles-azure-storage'
  scope: rg
  params: {
    azure_storage_outputs_name: azure_storage.outputs.name
    location: location
    principalId: web_identity.outputs.principalId
  }
}

output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_NAME string = resources.outputs.MANAGED_IDENTITY_NAME
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_NAME
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = resources.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID
output AZURE_CONTAINER_REGISTRY_NAME string = resources.outputs.AZURE_CONTAINER_REGISTRY_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_NAME string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN
output AZURE_STORAGE_TABLEENDPOINT string = azure_storage.outputs.tableEndpoint
output WEB_IDENTITY_CLIENTID string = web_identity.outputs.clientId
output WEB_IDENTITY_ID string = web_identity.outputs.id
