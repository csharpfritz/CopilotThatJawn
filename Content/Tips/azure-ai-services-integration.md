---
title: "Azure AI Services Integration Quick Start"
slug: "azure-ai-services-integration"
category: "Azure AI"
tags: ["azure", "ai-services", "integration", "apis", "cloud"]
difficulty: "Advanced"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-04"
lastModified: "2025-06-04"
description: "Get up and running with Azure AI Services in your applications. From setup to deployment, this guide covers the essentials."
---

# Azure AI Services Integration Quick Start

Ready to add some serious AI power to your applications? Azure AI Services is where it's at for building intelligent apps that can see, hear, speak, and understand. Let's get this jawn up and running!

## Prerequisites

Before we dive in, make sure you have:
- An active Azure subscription
- Visual Studio or VS Code
- .NET 8.0+ SDK
- Basic understanding of REST APIs

## Setting Up Azure AI Services

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

## Core AI Services Implementation

### Text Analytics Service

```csharp
public interface ITextAnalyticsService
{
    Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text);
    Task<List<string>> ExtractKeyPhrasesAsync(string text);
    Task<string> DetectLanguageAsync(string text);
}

public class TextAnalyticsService : ITextAnalyticsService
{
    private readonly TextAnalyticsClient _client;

    public TextAnalyticsService(TextAnalyticsClient client)
    {
        _client = client;
    }

    public async Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text)
    {
        var response = await _client.AnalyzeSentimentAsync(text);
        return new SentimentAnalysisResult
        {
            Sentiment = response.Value.Sentiment.ToString(),
            ConfidenceScore = response.Value.ConfidenceScores.Positive,
            IsPositive = response.Value.Sentiment == TextSentiment.Positive
        };
    }

    public async Task<List<string>> ExtractKeyPhrasesAsync(string text)
    {
        var response = await _client.ExtractKeyPhrasesAsync(text);
        return response.Value.ToList();
    }

    public async Task<string> DetectLanguageAsync(string text)
    {
        var response = await _client.DetectLanguageAsync(text);
        return response.Value.Name;
    }
}
```

### OpenAI Integration Service

```csharp
public interface IOpenAIService
{
    Task<string> GenerateResponseAsync(string prompt, int maxTokens = 150);
    Task<string> SummarizeTextAsync(string text);
    Task<List<string>> GenerateTipsAsync(string topic);
}

public class OpenAIService : IOpenAIService
{
    private readonly OpenAIClient _client;
    private readonly string _deploymentName;

    public OpenAIService(OpenAIClient client, IConfiguration config)
    {
        _client = client;
        _deploymentName = config["OpenAI:DeploymentName"];
    }

    public async Task<string> GenerateResponseAsync(string prompt, int maxTokens = 150)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = _deploymentName,
            Messages = { new ChatRequestUserMessage(prompt) },
            MaxTokens = maxTokens,
            Temperature = 0.7f
        };

        var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions);
        return response.Value.Choices[0].Message.Content;
    }

    public async Task<string> SummarizeTextAsync(string text)
    {
        var prompt = $"Summarize this text in 2-3 sentences:\n\n{text}";
        return await GenerateResponseAsync(prompt, 100);
    }

    public async Task<List<string>> GenerateTipsAsync(string topic)
    {
        var prompt = $"Generate 5 practical tips for {topic}. Format as a bulleted list.";
        var response = await GenerateResponseAsync(prompt, 200);
        
        return response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                      .Where(line => line.StartsWith("•") || line.StartsWith("-"))
                      .ToList();
    }
}
```

## Controller Implementation

```csharp
[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly ITextAnalyticsService _textAnalytics;
    private readonly IOpenAIService _openAI;

    public AIController(ITextAnalyticsService textAnalytics, IOpenAIService openAI)
    {
        _textAnalytics = textAnalytics;
        _openAI = openAI;
    }

    [HttpPost("analyze-sentiment")]
    public async Task<IActionResult> AnalyzeSentiment([FromBody] TextAnalysisRequest request)
    {
        try
        {
            var result = await _textAnalytics.AnalyzeSentimentAsync(request.Text);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error analyzing sentiment: {ex.Message}");
        }
    }

    [HttpPost("generate-tips")]
    public async Task<IActionResult> GenerateTips([FromBody] TipGenerationRequest request)
    {
        try
        {
            var tips = await _openAI.GenerateTipsAsync(request.Topic);
            return Ok(new { Tips = tips });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating tips: {ex.Message}");
        }
    }
}
```

## Error Handling Best Practices

### Global Exception Handler
```csharp
public class AzureAIExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AzureAIExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            _logger.LogWarning("Rate limit exceeded for Azure AI service");
            await HandleRateLimitAsync(context);
        }
        catch (RequestFailedException ex) when (ex.Status == 401)
        {
            _logger.LogError("Authentication failed for Azure AI service");
            await HandleAuthErrorAsync(context);
        }
    }
}
```

## Performance Optimization

### Caching Strategy
```csharp
services.AddMemoryCache();
services.Decorate<ITextAnalyticsService, CachedTextAnalyticsService>();

public class CachedTextAnalyticsService : ITextAnalyticsService
{
    private readonly ITextAnalyticsService _inner;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(30);

    public async Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text)
    {
        var cacheKey = $"sentiment_{text.GetHashCode()}";
        
        if (_cache.TryGetValue(cacheKey, out SentimentAnalysisResult cachedResult))
            return cachedResult;

        var result = await _inner.AnalyzeSentimentAsync(text);
        _cache.Set(cacheKey, result, _cacheExpiry);
        return result;
    }
}
```

## Monitoring and Logging

### Application Insights Integration
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();

// Custom telemetry
public class AIServiceTelemetry
{
    private readonly TelemetryClient _telemetryClient;

    public void TrackAIServiceCall(string serviceName, double duration, bool success)
    {
        _telemetryClient.TrackDependency("AzureAI", serviceName, 
            DateTime.UtcNow.AddMilliseconds(-duration), 
            TimeSpan.FromMilliseconds(duration), success);
    }
}
```

## Security Considerations

### Key Management
```csharp
// Use Azure Key Vault for production
services.AddAzureKeyVault(builder.Configuration["KeyVault:Vault"]);

// Rotate keys regularly
services.Configure<AzureAIOptions>(options =>
{
    options.ApiKey = builder.Configuration["AzureAI:ApiKey"];
    options.RotationDate = DateTime.UtcNow.AddDays(30);
});
```

## Testing Your Integration

### Unit Test Example
```csharp
[Test]
public async Task SentimentAnalysis_ShouldReturnPositive_ForPositiveText()
{
    // Arrange
    var mockClient = new Mock<TextAnalyticsClient>();
    var service = new TextAnalyticsService(mockClient.Object);
    
    // Act
    var result = await service.AnalyzeSentimentAsync("I love this jawn!");
    
    // Assert
    Assert.IsTrue(result.IsPositive);
}
```

## Deployment Checklist

- [ ] Set up Application Insights monitoring
- [ ] Configure auto-scaling for high traffic
- [ ] Set up alerts for rate limit warnings
- [ ] Implement circuit breaker pattern
- [ ] Test failover scenarios
- [ ] Document API rate limits
- [ ] Set up cost alerts

## Next Steps

1. **Explore Computer Vision**: Add image analysis capabilities
2. **Implement Speech Services**: Add voice interaction
3. **Custom Models**: Train domain-specific AI models
4. **Multi-region Deployment**: Set up global distribution

Remember, Azure AI Services is powerful jawn, but with great power comes great responsibility. Monitor your usage, implement proper error handling, and always consider the cost implications of your AI operations.

---

*Ready to build the next big AI-powered app in Philly? These foundations will get you there! Questions? Submit an issue or PR to our repo.*
