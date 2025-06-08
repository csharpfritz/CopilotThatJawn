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
        allowInsecure: false
        customDomains: [
          {
            name: 'copilotthatjawn.com'
            bindingType: 'SniEnabled'
            certificateId: '/subscriptions/09153f92-3cbc-46f1-8872-1683749eda4b/resourceGroups/rg-copilotthatjawn/providers/Microsoft.App/managedEnvironments/cae-uanpydy4xv63a/managedCertificates/copilotthatjawn.com-cae-uanp-250608192822'
          }
          {
            name: 'www.copilotthatjawn.com'
            bindingType: 'SniEnabled'
            certificateId: '/subscriptions/09153f92-3cbc-46f1-8872-1683749eda4b/resourceGroups/rg-copilotthatjawn/providers/Microsoft.App/managedEnvironments/cae-uanpydy4xv63a/managedCertificates/www.copilotthatjawn.com-cae-uanp-250608193349'
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
