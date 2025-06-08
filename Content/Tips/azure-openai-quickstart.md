---
title: "Quick Start: Azure OpenAI in C#"
category: "Azure AI"
tags: ["azure", "openai", "quickstart", "csharp"]
difficulty: "Beginner"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Get started with Azure OpenAI in your C# application in under 5 minutes."
---

# Quick Start: Azure OpenAI in C#

Need to add some AI magic to your C# app? Here's the quickest way to get that jawn up and running with Azure OpenAI!

## 1. Install the Package

```powershell
dotnet add package Azure.AI.OpenAI
```

## 2. Create the Service

```csharp
public class OpenAIService
{
    private readonly OpenAIClient _client;

    public OpenAIService(IConfiguration configuration)
    {
        var endpoint = configuration["AzureOpenAI:Endpoint"];
        var apiKey = configuration["AzureOpenAI:ApiKey"];
        
        _client = new OpenAIClient(
            new Uri(endpoint), 
            new AzureKeyCredential(apiKey)
        );
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        var response = await _client.GetCompletionsAsync(
            "gpt-35-turbo",
            new CompletionsOptions { Prompts = { prompt } }
        );
        
        return response.Value.Choices[0].Text.Trim();
    }
}
```

## 3. Register in Startup

```csharp
builder.Services.AddScoped<OpenAIService>();
```

That's it! Now you can inject `OpenAIService` into your controllers or pages and start generating text.

## Pro Tips
- Use `gpt-35-turbo` for most tasks, it's cost-effective
- Start with a low request count to test your integration
- Keep your API keys in user secrets during development

Need more advanced features? Check out our other Azure OpenAI tips for error handling, configuration, and performance optimization!
