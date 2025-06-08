---
title: "Azure OpenAI Service Integration Guide"
category: "Azure AI"
tags: ["azure", "openai", "ai-services", "gpt", "nlp"]
difficulty: "Intermediate"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Complete guide to integrating Azure OpenAI service into your applications. Learn how to implement chat completions, text summarization, and tip generation."
---

# Azure OpenAI Service Integration Guide

Want to add some GPT-4 magic to your app? Azure OpenAI Service is the jawn for you! This guide will show you how to integrate conversational AI, text generation, and more into your ASP.NET Core application.

## Prerequisites

Make sure you've completed our [Azure AI Services Setup Guide](azure-ai-services-setup.md) first. For OpenAI specifically, you'll need:
- Azure OpenAI Service resource
- Model deployment (e.g., GPT-4, GPT-3.5 Turbo)
- Endpoint and API key

## OpenAI Service Implementation

Let's implement a service to handle OpenAI interactions:

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
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(
        OpenAIClient client,
        IConfiguration config,
        ILogger<OpenAIService> logger)
    {
        _client = client;
        _deploymentName = config["OpenAI:DeploymentName"];
        _logger = logger;
    }

    public async Task<string> GenerateResponseAsync(string prompt, int maxTokens = 150)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating completion for prompt: {Prompt}", prompt);
            throw;
        }
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

## API Controller Implementation

Here's how to expose the OpenAI functionality through an API:

```csharp
[ApiController]
[Route("api/[controller]")]
public class OpenAIController : ControllerBase
{
    private readonly IOpenAIService _openAI;
    private readonly ILogger<OpenAIController> _logger;

    public OpenAIController(
        IOpenAIService openAI,
        ILogger<OpenAIController> logger)
    {
        _openAI = openAI;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerationRequest request)
    {
        try
        {
            var response = await _openAI.GenerateResponseAsync(
                request.Prompt, 
                request.MaxTokens ?? 150);
                
            return Ok(new { Response = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response");
            return BadRequest("Failed to generate response");
        }
    }

    [HttpPost("summarize")]
    public async Task<IActionResult> Summarize([FromBody] TextRequest request)
    {
        try
        {
            var summary = await _openAI.SummarizeTextAsync(request.Text);
            return Ok(new { Summary = summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error summarizing text");
            return BadRequest("Failed to summarize text");
        }
    }

    [HttpPost("tips")]
    public async Task<IActionResult> GenerateTips([FromBody] TipsRequest request)
    {
        try
        {
            var tips = await _openAI.GenerateTipsAsync(request.Topic);
            return Ok(new { Tips = tips });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating tips");
            return BadRequest("Failed to generate tips");
        }
    }
}
```

## Request Rate Limiting

Implement rate limiting to manage API usage:

```csharp
public class OpenAIRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OpenAIRateLimitMiddleware> _logger;
    private readonly IMemoryCache _cache;
    private const int MaxRequestsPerMinute = 60;

    public OpenAIRateLimitMiddleware(
        RequestDelegate next,
        ILogger<OpenAIRateLimitMiddleware> logger,
        IMemoryCache cache)
    {
        _next = next;
        _logger = logger;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var cacheKey = $"rate_limit_{clientId}";

        if (!_cache.TryGetValue(cacheKey, out int requestCount))
        {
            requestCount = 0;
        }

        if (requestCount >= MaxRequestsPerMinute)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded. Please try again later."
            });
            return;
        }

        _cache.Set(cacheKey, requestCount + 1, TimeSpan.FromMinutes(1));
        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Implement your client identification logic here
        return context.User?.Identity?.Name ?? 
               context.Connection.RemoteIpAddress?.ToString() ?? 
               "anonymous";
    }
}
```

## Cost Management

Implement a service to track token usage and costs:

```csharp
public class OpenAICostTracker
{
    private readonly ILogger<OpenAICostTracker> _logger;
    private readonly TelemetryClient _telemetry;

    public OpenAICostTracker(
        ILogger<OpenAICostTracker> logger,
        TelemetryClient telemetry)
    {
        _logger = logger;
        _telemetry = telemetry;
    }

    public void TrackTokenUsage(
        string operation,
        int promptTokens,
        int completionTokens,
        string model)
    {
        var totalTokens = promptTokens + completionTokens;
        
        _telemetry.TrackMetric("OpenAI.TokenUsage", totalTokens, new Dictionary<string, string>
        {
            { "Operation", operation },
            { "Model", model }
        });

        _logger.LogInformation(
            "OpenAI usage - Operation: {Operation}, Model: {Model}, " + 
            "Prompt Tokens: {PromptTokens}, Completion Tokens: {CompletionTokens}",
            operation, model, promptTokens, completionTokens);
    }
}
```

## Testing

Here's how to test your OpenAI integration:

```csharp
public class OpenAIServiceTests
{
    [Fact]
    public async Task GenerateTips_ShouldReturnFiveItems()
    {
        // Arrange
        var mockClient = new Mock<OpenAIClient>();
        var mockConfig = new Mock<IConfiguration>();
        var mockLogger = new Mock<ILogger<OpenAIService>>();
        
        mockConfig.Setup(x => x["OpenAI:DeploymentName"])
                 .Returns("gpt-4");

        var chatResponse = new ChatCompletions("", "", "", 0, new[] 
        {
            new ChatChoice(0, new ChatMessage(ChatRole.Assistant, 
                "- Tip 1\n- Tip 2\n- Tip 3\n- Tip 4\n- Tip 5"))
        });

        mockClient.Setup(x => x.GetChatCompletionsAsync(
            It.IsAny<string>(), 
            It.IsAny<ChatCompletionsOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(chatResponse, new MockHttpResponse()));

        var service = new OpenAIService(mockClient.Object, mockConfig.Object, mockLogger.Object);

        // Act
        var tips = await service.GenerateTipsAsync("testing");

        // Assert
        Assert.Equal(5, tips.Count);
    }
}
```

## Best Practices

1. **Prompt Engineering**: Craft clear, specific prompts for better results
2. **Error Handling**: Implement comprehensive error handling
3. **Rate Limiting**: Control API usage to manage costs
4. **Monitoring**: Track token usage and response times
5. **Cost Management**: Set up budgets and alerts
6. **Content Filtering**: Implement content filtering for generated text
7. **Caching**: Cache responses for identical prompts

## Implementation Tips

1. Start with a lower-cost model like GPT-3.5 Turbo for testing
2. Use streaming for long-form content generation
3. Implement retry logic with exponential backoff
4. Set up proper timeout handling
5. Consider implementing a prompt template system

## Next Steps

1. Explore other Azure OpenAI models like embeddings and DALL-E
2. Implement streaming responses for long-form content
3. Add semantic search capabilities
4. Set up monitoring and alerting
5. Explore fine-tuning for your specific use case

Remember, with great AI power comes great responsibility. Monitor your usage, implement proper error handling, and always validate the output before showing it to users.

---

*Got some cool OpenAI implementation tips to share? Submit a PR to our repo!*
