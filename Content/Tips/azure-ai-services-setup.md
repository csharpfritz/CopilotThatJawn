---
title: "Azure AI Services Setup Guide"
category: "Azure AI"
tags: ["azure", "ai-services", "setup", "configuration", "cloud"]
difficulty: "Beginner"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Step-by-step guide to set up Azure AI Services for your applications, from prerequisites to configuration. Perfect for getting started with Azure AI."
---

# Azure AI Services Setup Guide

Ready to add some serious AI power to your applications? Let's set up Azure AI Services - your gateway to building intelligent apps that can see, hear, speak, and understand. This guide will get your jawn up and running with the basics!

## Prerequisites

Before we dive in, make sure you have:
- An active Azure subscription
- Visual Studio or VS Code
- .NET 8.0+ SDK
- Basic understanding of REST APIs

## Setting Up Azure AI Resources

### 1. Create Your Azure AI Resource

```bash
# Using Azure CLI (the fast way)
az cognitiveservices account create \
  --name "copilot-that-jawn-ai" \
  --resource-group "your-resource-group" \
  --kind "CognitiveServices" \
  --sku "S0" \
  --location "eastus"
```

### 2. Get Your Keys and Endpoint

```bash
# Get your subscription key
az cognitiveservices account keys list \
  --name "copilot-that-jawn-ai" \
  --resource-group "your-resource-group"

# Get your endpoint
az cognitiveservices account show \
  --name "copilot-that-jawn-ai" \
  --resource-group "your-resource-group" \
  --query "properties.endpoint"
```

## Essential NuGet Packages

Add these to your ASP.NET Core project:

```xml
<PackageReference Include="Azure.AI.TextAnalytics" Version="5.3.0" />
<PackageReference Include="Azure.AI.Vision.ImageAnalysis" Version="1.0.0" />
<PackageReference Include="Azure.AI.OpenAI" Version="1.0.0" />
<PackageReference Include="Microsoft.Extensions.Azure" Version="1.7.0" />
```

## Configuration Setup

### appsettings.json

```json
{
  "AzureAI": {
    "Endpoint": "https://your-resource.cognitiveservices.azure.com/",
    "ApiKey": "your-api-key-here",
    "Region": "eastus"
  },
  "OpenAI": {
    "Endpoint": "https://your-openai-resource.openai.azure.com/",
    "ApiKey": "your-openai-key",
    "DeploymentName": "gpt-4"
  }
}
```

### Program.cs Configuration

```csharp
// Add Azure AI services to DI container
builder.Services.AddAzureClients(clientBuilder =>
{
    // Text Analytics
    clientBuilder.AddTextAnalyticsClient(
        new Uri(builder.Configuration["AzureAI:Endpoint"]),
        new AzureKeyCredential(builder.Configuration["AzureAI:ApiKey"]));
    
    // OpenAI
    clientBuilder.AddOpenAIClient(
        new Uri(builder.Configuration["OpenAI:Endpoint"]),
        new AzureKeyCredential(builder.Configuration["OpenAI:ApiKey"]));
});
```

## Security Best Practices

1. **Never Commit Secrets**: Keep your API keys and sensitive configuration in Azure Key Vault or user secrets
2. **Use Managed Identity**: When possible, use Azure Managed Identity for authentication
3. **Implement RBAC**: Set up proper role-based access control for your resources
4. **Regular Key Rotation**: Set up a process for regular key rotation
5. **Monitor Usage**: Keep an eye on your resource usage and set up alerts

## Next Steps

1. Check out our other tips for implementing specific Azure AI services
2. Consider setting up Application Insights for monitoring
3. Explore the Azure AI Studio for model customization
4. Join the Philly tech community discussions on AI integration

Remember, this is just the beginning of your Azure AI journey. Once you've got this setup locked down, you'll be ready to implement specific services like Text Analytics, Computer Vision, or OpenAI. Check out our other tips for detailed implementation guides!

---

*Got questions about setting up Azure AI Services? Submit an issue or PR to our repo - we're here to help!*
