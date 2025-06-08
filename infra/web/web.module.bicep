@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param outputs_azure_container_apps_environment_default_domain string

param outputs_azure_container_apps_environment_id string

param outputs_azure_container_registry_endpoint string

param outputs_azure_container_registry_managed_identity_id string

param web_containerimage string

param web_identity_outputs_id string

param web_containerport string

param azure_storage_outputs_tableendpoint string

param web_identity_outputs_clientid string

param certificateName string

param domainApex string

param wwwCertificateName string

param domainWww string

resource web 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'web'
  location: location
  properties: {
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: int(web_containerport)
        transport: 'http'
        customDomains: [
          {
            name: domainApex
            bindingType: (certificateName != '') ? 'SniEnabled' : 'Disabled'
            certificateId: (certificateName != '') ? '${outputs_azure_container_apps_environment_id}/managedCertificates/${certificateName}' : null
          }
          {
            name: domainWww
            bindingType: (wwwCertificateName != '') ? 'SniEnabled' : 'Disabled'
            certificateId: (wwwCertificateName != '') ? '${outputs_azure_container_apps_environment_id}/managedCertificates/${wwwCertificateName}' : null
          }
        ]
      }
      registries: [
        {
          server: outputs_azure_container_registry_endpoint
          identity: outputs_azure_container_registry_managed_identity_id
        }
      ]
    }
    environmentId: outputs_azure_container_apps_environment_id
    template: {
      containers: [
        {
          image: web_containerimage
          name: 'web'
          env: [
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY'
              value: 'in_memory'
            }
            {
              name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
              value: 'true'
            }
            {
              name: 'HTTP_PORTS'
              value: web_containerport
            }
            {
              name: 'ConnectionStrings__tables'
              value: azure_storage_outputs_tableendpoint
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: web_identity_outputs_clientid
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${web_identity_outputs_id}': { }
      '${outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}