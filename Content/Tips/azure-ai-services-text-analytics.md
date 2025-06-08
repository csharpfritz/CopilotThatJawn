---
title: "Azure AI Text Analytics Integration Guide"
category: "Azure AI"
tags: ["azure", "ai-services", "text-analytics", "sentiment-analysis", "nlp"]
difficulty: "Intermediate"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Learn how to integrate Azure Text Analytics services into your applications for sentiment analysis, key phrase extraction, and language detection."
---

Want to add some natural language understanding to your app? Azure Text Analytics is the jawn you need! Let's integrate sentiment analysis, key phrases extraction, and language detection into your ASP.NET Core application.

## Prerequisites

Before we dive in, make sure you have:

- Completed our [Azure AI Services Setup Guide](azure-ai-services-setup.md)
- Basic understanding of ASP.NET Core dependency injection
- Azure Text Analytics resource created and configured

## Service Implementation

First, let's create a strongly-typed service for Text Analytics operations:

```csharp
public record SentimentAnalysisResult
{
    public string Sentiment { get; init; }
    public double ConfidenceScore { get; init; }
    public bool IsPositive { get; init; }
    public string Text { get; init; }
    public DateTime AnalyzedAt { get; init; } = DateTime.UtcNow;
}

public interface ITextAnalyticsService
{
    Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text);
    Task<List<string>> ExtractKeyPhrasesAsync(string text);
    Task<string> DetectLanguageAsync(string text);
    Task<List<string>> DetectEntitiesAsync(string text);
}

public class TextAnalyticsService : ITextAnalyticsService
{
    private readonly TextAnalyticsClient _client;
    private readonly ILogger<TextAnalyticsService> _logger;

    public TextAnalyticsService(
        TextAnalyticsClient client,
        ILogger<TextAnalyticsService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text)
    {
        try
        {
            _logger.LogInformation("Analyzing sentiment for text: {TextPreview}", 
                text.Length > 50 ? text[..50] + "..." : text);

            var response = await _client.AnalyzeSentimentAsync(text);
            
            return new SentimentAnalysisResult
            {
                Sentiment = response.Value.Sentiment.ToString(),
                ConfidenceScore = response.Value.ConfidenceScores.Positive,
                IsPositive = response.Value.Sentiment == TextSentiment.Positive,
                Text = text
            };
        }
        catch (Exception ex) when (ex is RequestFailedException)
        {
            _logger.LogError(ex, "Error analyzing sentiment");
            throw;
        }
    }

    public async Task<List<string>> ExtractKeyPhrasesAsync(string text)
    {
        try
        {
            _logger.LogInformation("Extracting key phrases from text: {TextPreview}",
                text.Length > 50 ? text[..50] + "..." : text);

            var response = await _client.ExtractKeyPhrasesAsync(text);
            return response.Value.ToList();
        }
        catch (Exception ex) when (ex is RequestFailedException)
        {
            _logger.LogError(ex, "Error extracting key phrases");
            throw;
        }
    }

    public async Task<string> DetectLanguageAsync(string text)
    {
        try
        {
            _logger.LogInformation("Detecting language for text: {TextPreview}",
                text.Length > 50 ? text[..50] + "..." : text);

            var response = await _client.DetectLanguageAsync(text);
            return response.Value.Name;
        }
        catch (Exception ex) when (ex is RequestFailedException)
        {
            _logger.LogError(ex, "Error detecting language");
            throw;
        }
    }

    public async Task<List<string>> DetectEntitiesAsync(string text)
    {
        try
        {
            _logger.LogInformation("Detecting entities in text: {TextPreview}",
                text.Length > 50 ? text[..50] + "..." : text);

            var response = await _client.RecognizeEntitiesAsync(text);
            return response.Value.Select(e => e.Text).ToList();
        }
        catch (Exception ex) when (ex is RequestFailedException)
        {
            _logger.LogError(ex, "Error detecting entities");
            throw;
        }
    }
}
```

## API Controller Implementation

Create an API controller to expose these capabilities:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TextAnalyticsController : ControllerBase
{
    private readonly ITextAnalyticsService _textAnalytics;
    private readonly ILogger<TextAnalyticsController> _logger;

    public TextAnalyticsController(
        ITextAnalyticsService textAnalytics,
        ILogger<TextAnalyticsController> logger)
    {
        _textAnalytics = textAnalytics;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeText([FromBody] TextAnalysisRequest request)
    {
        try
        {
            var analysis = new TextAnalysis
            {
                Sentiment = await _textAnalytics.AnalyzeSentimentAsync(request.Text),
                KeyPhrases = await _textAnalytics.ExtractKeyPhrasesAsync(request.Text),
                Language = await _textAnalytics.DetectLanguageAsync(request.Text),
                Entities = await _textAnalytics.DetectEntitiesAsync(request.Text)
            };

            return Ok(analysis);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Error analyzing text");
            return StatusCode(ex.Status, new { error = ex.Message });
        }
    }
}

public record TextAnalysisRequest(string Text);

public record TextAnalysis
{
    public SentimentAnalysisResult Sentiment { get; init; }
    public List<string> KeyPhrases { get; init; }
    public string Language { get; init; }
    public List<string> Entities { get; init; }
    public DateTime AnalyzedAt { get; init; } = DateTime.UtcNow;
}
```

## Performance Optimization

Implement caching to improve performance:

```csharp
public class CachedTextAnalyticsService : ITextAnalyticsService
{
    private readonly ITextAnalyticsService _inner;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedTextAnalyticsService> _logger;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(30);

    public CachedTextAnalyticsService(
        ITextAnalyticsService inner,
        IMemoryCache cache,
        ILogger<CachedTextAnalyticsService> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<SentimentAnalysisResult> AnalyzeSentimentAsync(string text)
    {
        var cacheKey = $"sentiment_{text.GetHashCode()}";
        
        if (_cache.TryGetValue(cacheKey, out SentimentAnalysisResult cachedResult))
        {
            _logger.LogInformation("Cache hit for sentiment analysis");
            return cachedResult;
        }

        var result = await _inner.AnalyzeSentimentAsync(text);
        _cache.Set(cacheKey, result, _cacheExpiry);
        return result;
    }

    // Implement other methods similarly...
}
```

## Error Handling

Add a specialized exception filter:

```csharp
public class TextAnalyticsExceptionFilter : IExceptionFilter
{
    private readonly ILogger<TextAnalyticsExceptionFilter> _logger;

    public TextAnalyticsExceptionFilter(ILogger<TextAnalyticsExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is RequestFailedException rex)
        {
            var error = new
            {
                Message = GetUserFriendlyMessage(rex),
                Details = rex.Message,
                ErrorCode = rex.ErrorCode
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = rex.Status
            };

            _logger.LogError(rex, "Text Analytics error: {ErrorCode}", rex.ErrorCode);
            context.ExceptionHandled = true;
        }
    }

    private string GetUserFriendlyMessage(RequestFailedException ex) => ex.Status switch
    {
        429 => "We're processing too many requests right now. Please try again in a minute.",
        401 => "Authentication failed. Please check your credentials.",
        400 => "Invalid request. Please check your input.",
        _ => "An unexpected error occurred. Please try again later."
    };
}
```

## Testing

Here's how to test your Text Analytics implementation:

```csharp
public class TextAnalyticsServiceTests
{
    [Fact]
    public async Task AnalyzeSentiment_ShouldReturnPositive_ForPositiveText()
    {
        // Arrange
        var mockClient = new Mock<TextAnalyticsClient>();
        var logger = new Mock<ILogger<TextAnalyticsService>>();
        var service = new TextAnalyticsService(mockClient.Object, logger.Object);

        mockClient.Setup(x => x.AnalyzeSentimentAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(
                new DocumentSentiment(
                    TextSentiment.Positive,
                    new SentimentConfidenceScores(0.9, 0.1, 0.0),
                    new[]
                    {
                        new SentenceSentiment(
                            TextSentiment.Positive,
                            new SentimentConfidenceScores(0.9, 0.1, 0.0))
                    }),
                new Mock<Response>().Object));

        // Act
        var result = await service.AnalyzeSentimentAsync("I love this jawn!");

        // Assert
        Assert.True(result.IsPositive);
        Assert.Equal("Positive", result.Sentiment);
        Assert.True(result.ConfidenceScore > 0.8);
    }
}
```

## Best Practices

1. **Input Validation**
   - Validate text length (API limits)
   - Handle empty or whitespace input
   - Consider text encoding issues

2. **Error Handling**
   - Implement retry logic for transient failures
   - Log errors with proper context
   - Return user-friendly error messages

3. **Performance**
   - Use batch operations when possible
   - Implement caching strategically
   - Monitor response times

4. **Cost Management**
   - Track API usage
   - Implement rate limiting
   - Cache frequently analyzed content

5. **Security**
   - Sanitize input text
   - Validate output before storage
   - Implement proper access controls

## Implementation Checklist

- [ ] Set up Text Analytics client configuration
- [ ] Implement service layer with error handling
- [ ] Add caching for appropriate operations
- [ ] Configure logging and monitoring
- [ ] Set up unit tests
- [ ] Add rate limiting
- [ ] Implement health checks
- [ ] Add performance metrics
- [ ] Configure alerting

## Next Steps

1. Explore advanced Text Analytics features
2. Implement batch processing
3. Add more comprehensive error handling
4. Set up monitoring dashboards
5. Configure cost alerts

Remember, Text Analytics can help you understand what your users are saying, but always validate and sanitize both input and output to ensure reliable and secure operation.

---

*Got Text Analytics tips to share? Submit a PR to our repo!*
