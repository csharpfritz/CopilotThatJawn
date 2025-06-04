---
title: "Multi-Language Code Examples for AI Development"
slug: "multi-language-ai-development"
category: "Development"
tags: ["csharp", "javascript", "python", "powershell", "ai", "examples"]
difficulty: "Intermediate"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-04"
lastModified: "2025-06-04"
description: "Comprehensive examples of AI development patterns across multiple programming languages with proper syntax highlighting."
---

# Multi-Language Code Examples for AI Development

This comprehensive guide showcases AI development patterns across multiple programming languages, demonstrating how syntax highlighting enhances code readability and learning.

## C# - Azure OpenAI Integration

Here's how to integrate Azure OpenAI services into your ASP.NET Core application:

```csharp
using Azure;
using Azure.AI.OpenAI;

public class OpenAIService
{
    private readonly OpenAIClient _client;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
    {
        var endpoint = configuration["AzureOpenAI:Endpoint"];
        var apiKey = configuration["AzureOpenAI:ApiKey"];
        
        _client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _logger = logger;
    }

    public async Task<string> GenerateCompletionAsync(string prompt, string deploymentName = "gpt-35-turbo")
    {
        try
        {
            var completionsOptions = new CompletionsOptions()
            {
                Prompts = { prompt },
                MaxTokens = 1000,
                Temperature = 0.7f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };

            var response = await _client.GetCompletionsAsync(deploymentName, completionsOptions);
            return response.Value.Choices[0].Text.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating completion for prompt: {Prompt}", prompt);
            throw;
        }
    }
}
```

## JavaScript - Frontend AI Integration

Client-side AI integration for real-time assistance:

```javascript
class AIAssistant {
    constructor(apiEndpoint, apiKey) {
        this.apiEndpoint = apiEndpoint;
        this.apiKey = apiKey;
        this.isLoading = false;
    }

    async generateSuggestion(userInput, context = {}) {
        if (this.isLoading) {
            console.warn('AI request already in progress');
            return null;
        }

        this.isLoading = true;
        this.updateLoadingState(true);

        try {
            const requestBody = {
                prompt: this.buildPrompt(userInput, context),
                max_tokens: 150,
                temperature: 0.5,
                stream: false
            };

            const response = await fetch(`${this.apiEndpoint}/completions`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.apiKey}`
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            return data.choices[0].text.trim();

        } catch (error) {
            console.error('AI suggestion failed:', error);
            this.showErrorMessage('Failed to get AI suggestion. Please try again.');
            return null;
        } finally {
            this.isLoading = false;
            this.updateLoadingState(false);
        }
    }

    buildPrompt(userInput, context) {
        const { projectType = 'general', difficulty = 'beginner' } = context;
        return `
            Context: ${projectType} development project (${difficulty} level)
            User request: ${userInput}
            
            Provide a helpful, specific suggestion:
        `.trim();
    }

    updateLoadingState(isLoading) {
        const button = document.getElementById('ai-suggest-btn');
        const spinner = document.getElementById('loading-spinner');
        
        if (button) {
            button.disabled = isLoading;
            button.textContent = isLoading ? 'Generating...' : 'Get AI Suggestion';
        }
        
        if (spinner) {
            spinner.style.display = isLoading ? 'inline-block' : 'none';
        }
    }

    showErrorMessage(message) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger';
        errorDiv.textContent = message;
        
        const container = document.getElementById('ai-responses');
        if (container) {
            container.appendChild(errorDiv);
            setTimeout(() => errorDiv.remove(), 5000);
        }
    }
}

// Initialize AI assistant when page loads
document.addEventListener('DOMContentLoaded', () => {
    const assistant = new AIAssistant('/api/ai', 'your-api-key-here');
    
    // Example usage
    const suggestButton = document.getElementById('ai-suggest-btn');
    if (suggestButton) {
        suggestButton.addEventListener('click', async () => {
            const userInput = document.getElementById('user-input').value;
            const suggestion = await assistant.generateSuggestion(userInput);
            
            if (suggestion) {
                displaySuggestion(suggestion);
            }
        });
    }
});
```

## PowerShell - Azure Deployment Automation

Automate your AI application deployments with PowerShell:

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

## JSON Configuration

Here's a comprehensive configuration file for AI services:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-openai-service.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "ApiVersion": "2023-12-01-preview",
    "Models": {
      "Gpt35Turbo": "gpt-35-turbo",
      "Gpt4": "gpt-4",
      "TextEmbedding": "text-embedding-ada-002"
    },
    "DefaultSettings": {
      "MaxTokens": 1000,
      "Temperature": 0.7,
      "TopP": 0.95,
      "FrequencyPenalty": 0,
      "PresencePenalty": 0
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Azure.AI.OpenAI": "Debug"
    }
  },
  "ContentSettings": {
    "CacheExpiryMinutes": 5,
    "MaxContentLength": 10000,
    "SupportedLanguages": ["csharp", "javascript", "python", "powershell", "json", "yaml"],
    "SyntaxHighlighting": {
      "Theme": "dark",
      "ShowLineNumbers": true,
      "CopyButtonEnabled": true
    }
  },
  "RateLimiting": {
    "RequestsPerMinute": 60,
    "RequestsPerHour": 1000,
    "BurstLimit": 10
  }
}
```

## Best Practices Summary

1. **Language-Specific Patterns**: Each language has its own conventions and best practices
2. **Error Handling**: Always implement comprehensive error handling across all languages
3. **Configuration Management**: Use proper configuration patterns for each platform
4. **Security**: Never hard-code API keys or sensitive information
5. **Performance**: Consider caching, rate limiting, and async patterns
6. **Testing**: Write tests for your AI integrations to ensure reliability

This multi-language approach demonstrates how syntax highlighting enhances code readability and helps developers quickly understand patterns across different technologies in the AI development ecosystem.
