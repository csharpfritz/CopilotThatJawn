---
title: "PowerShell Azure AI Deployment Automation"
category: "Development"
tags: ["powershell", "azure", "deployment", "automation"]
difficulty: "Advanced"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Learn how to automate Azure AI application deployments using PowerShell with proper error handling and resource management."
---

# PowerShell Azure AI Deployment Automation

This guide demonstrates how to automate Azure AI application deployments using PowerShell, including resource creation, configuration, and proper error handling.

## Deployment Script

```powershell
# Azure AI Application Deployment Script
param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$AppServiceName,
    
    [Parameter(Mandatory=$true)]
    [string]$Location = "East US",
    
    [Parameter(Mandatory=$false)]
    [string]$OpenAIServiceName = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$CreateOpenAIService
)

# Set error action preference
$ErrorActionPreference = "Stop"

Write-Host "Starting deployment of AI application..." -ForegroundColor Green

try {
    # Login to Azure (if not already logged in)
    $context = Get-AzContext
    if (!$context) {
        Write-Host "Logging into Azure..." -ForegroundColor Yellow
        Connect-AzAccount
    }

    # Create resource group if it doesn't exist
    $rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
    if (!$rg) {
        Write-Host "Creating resource group: $ResourceGroupName" -ForegroundColor Yellow
        New-AzResourceGroup -Name $ResourceGroupName -Location $Location
    }

    # Create App Service Plan
    $appServicePlan = "${AppServiceName}-plan"
    Write-Host "Creating App Service Plan: $appServicePlan" -ForegroundColor Yellow
    
    New-AzAppServicePlan -ResourceGroupName $ResourceGroupName `
                         -Name $appServicePlan `
                         -Location $Location `
                         -Tier "Standard" `
                         -NumberofWorkers 1 `
                         -WorkerSize "Small"

    # Create Web App
    Write-Host "Creating Web App: $AppServiceName" -ForegroundColor Yellow
    
    $webApp = New-AzWebApp -ResourceGroupName $ResourceGroupName `
                           -Name $AppServiceName `
                           -Location $Location `
                           -AppServicePlan $appServicePlan

    # Create Azure OpenAI Service if requested
    if ($CreateOpenAIService) {
        if ([string]::IsNullOrEmpty($OpenAIServiceName)) {
            $OpenAIServiceName = "${AppServiceName}-openai"
        }
        
        Write-Host "Creating Azure OpenAI Service: $OpenAIServiceName" -ForegroundColor Yellow
        
        # Note: This requires the Azure CLI as PowerShell modules for OpenAI are limited
        $openAIResult = az cognitiveservices account create `
            --name $OpenAIServiceName `
            --resource-group $ResourceGroupName `
            --location $Location `
            --kind "OpenAI" `
            --sku "S0" `
            --output json | ConvertFrom-Json

        if ($openAIResult) {
            Write-Host "Azure OpenAI Service created successfully!" -ForegroundColor Green
            
            # Get the endpoint and keys
            $endpoint = $openAIResult.properties.endpoint
            $keys = az cognitiveservices account keys list `
                --name $OpenAIServiceName `
                --resource-group $ResourceGroupName `
                --output json | ConvertFrom-Json
            
            # Configure app settings
            $appSettings = @{
                "AzureOpenAI:Endpoint" = $endpoint
                "AzureOpenAI:ApiKey" = $keys.key1
                "ASPNETCORE_ENVIRONMENT" = "Production"
            }
            
            Write-Host "Configuring application settings..." -ForegroundColor Yellow
            Set-AzWebApp -ResourceGroupName $ResourceGroupName `
                         -Name $AppServiceName `
                         -AppSettings $appSettings
        }
    }

    # Build and deploy the application
    Write-Host "Building application..." -ForegroundColor Yellow
    dotnet build --configuration Release

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Publishing application..." -ForegroundColor Yellow
        dotnet publish --configuration Release --output "./publish"
        
        # Create deployment package
        $publishPath = Resolve-Path "./publish"
        Compress-Archive -Path "$publishPath\*" -DestinationPath "./deployment.zip" -Force
        
        # Deploy to Azure
        Write-Host "Deploying to Azure App Service..." -ForegroundColor Yellow
        Publish-AzWebApp -ResourceGroupName $ResourceGroupName `
                         -Name $AppServiceName `
                         -ArchivePath "./deployment.zip" `
                         -Force
        
        Write-Host "Deployment completed successfully!" -ForegroundColor Green
        Write-Host "Application URL: https://$AppServiceName.azurewebsites.net" -ForegroundColor Cyan
    } else {
        throw "Build failed with exit code $LASTEXITCODE"
    }

} catch {
    Write-Error "Deployment failed: $($_.Exception.Message)"
    exit 1
} finally {
    # Cleanup
    if (Test-Path "./deployment.zip") {
        Remove-Item "./deployment.zip" -Force
    }
    if (Test-Path "./publish") {
        Remove-Item "./publish" -Recurse -Force
    }
}
```

## Key Features

1. **Parameter Validation**: Required parameters with proper types
2. **Error Handling**: Comprehensive try-catch blocks with cleanup
3. **Resource Management**: Creates and configures all necessary Azure resources
4. **Configuration**: Automatic app settings configuration
5. **Cleanup**: Proper cleanup of temporary deployment files

## Best Practices

- Always use proper error handling with try-catch blocks
- Include parameter validation
- Implement proper cleanup in finally blocks
- Use clear, informative status messages
- Check for existing resources before creation
- Handle authentication state
- Use proper Azure PowerShell cmdlets
- Implement proper logging with colors for visibility
