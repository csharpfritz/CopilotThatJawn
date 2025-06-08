---
title: "Configuration Patterns for Azure OpenAI"
category: "Azure AI"
tags: ["azure", "openai", "configuration", "security"]
difficulty: "Intermediate"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Learn secure configuration patterns for Azure OpenAI in your ASP.NET Core applications."
---

# Configuration Patterns for Azure OpenAI

Keep your Azure OpenAI configuration secure and maintainable with these patterns. Don't let your API keys be exposed like a tourist at the Liberty Bell!

## Configuration Structure

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-key-here",
    "DefaultModel": "gpt-35-turbo",
    "RetryCount": 3,
    "Deployments": {
      "Completion": "completion-deployment",
      "Chat": "chat-deployment",
      "Embedding": "embedding-deployment"
    }
  }
}
```

## Strongly-Typed Settings

```csharp
public class AzureOpenAISettings
{
    public string Endpoint { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string DefaultModel { get; init; } = "gpt-35-turbo";
    public int RetryCount { get; init; } = 3;
    public DeploymentSettings Deployments { get; init; } = new();

    public class DeploymentSettings
    {
        public string Completion { get; init; } = string.Empty;
        public string Chat { get; init; } = string.Empty;
        public string Embedding { get; init; } = string.Empty;
    }
}
```

## Secure Registration

```csharp
// Program.cs
builder.Services.Configure<AzureOpenAISettings>(
    builder.Configuration.GetSection("AzureOpenAI")
);

builder.Services.AddScoped<OpenAIService>();
```

## Service Implementation

```csharp
public class OpenAIService
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAISettings _settings;

    public OpenAIService(
        IOptions<AzureOpenAISettings> settings)
    {
        _settings = settings.Value;
        _client = new OpenAIClient(
            new Uri(_settings.Endpoint),
            new AzureKeyCredential(_settings.ApiKey)
        );
    }
}
```

## Security Best Practices

1. **Development**: Use User Secrets
   ```powershell
   dotnet user-secrets set "AzureOpenAI:ApiKey" "your-key-here"
   ```

2. **Production**: Use Key Vault
   ```csharp
   builder.Configuration.AddAzureKeyVault(
       new Uri(keyVaultUri),
       new DefaultAzureCredential()
   );
   ```

3. **Environment-Specific Settings**:
   ```json
   // appsettings.Development.json
   {
     "AzureOpenAI": {
       "RetryCount": 1,
       "Endpoint": "https://dev-resource.openai.azure.com/"
     }
   }
   ```

## Pro Tips

- Never commit API keys to source control
- Use different deployments for dev/prod
- Rotate keys regularly (90-day max)
- Monitor key usage in Azure Portal

Remember: Configuration might not be the most exciting part of AI development, but getting it right will save you from some major headaches down the road!
