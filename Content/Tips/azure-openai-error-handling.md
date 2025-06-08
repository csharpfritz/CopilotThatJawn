---
title: "Error Handling in Azure OpenAI Applications"
category: "Azure AI"
tags: ["azure", "openai", "error-handling", "logging"]
difficulty: "Intermediate"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Learn best practices for handling errors and implementing logging in your Azure OpenAI applications."
---

# Error Handling in Azure OpenAI Applications

Keep your AI-powered app running smooth with these error handling patterns. Don't let those API hiccups mess up your jawn!

## Key Error Handling Patterns

```csharp
public class OpenAIService
{
    private readonly OpenAIClient _client;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(
        IConfiguration configuration, 
        ILogger<OpenAIService> logger)
    {
        _client = new OpenAIClient(
            new Uri(configuration["AzureOpenAI:Endpoint"]), 
            new AzureKeyCredential(configuration["AzureOpenAI:ApiKey"])
        );
        _logger = logger;
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        try
        {
            _logger.LogInformation(
                "Generating completion for prompt: {Prompt}", 
                prompt[..Math.Min(100, prompt.Length)]
            );

            var response = await _client.GetCompletionsAsync(
                "gpt-35-turbo",
                new CompletionsOptions { Prompts = { prompt } }
            );

            return response.Value.Choices[0].Text.Trim();
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            _logger.LogWarning("Rate limit hit: {Message}", ex.Message);
            throw new RateLimitException("AI service is busy", ex);
        }
        catch (RequestFailedException ex) when (ex.Status == 401)
        {
            _logger.LogError("Authentication failed: {Message}", ex.Message);
            throw new ConfigurationException("Invalid API key", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error generating completion for prompt: {Prompt}", 
                prompt[..Math.Min(100, prompt.Length)]
            );
            throw;
        }
    }
}
```

## Logging Best Practices

1. **Structured Logging**: Use message templates with parameters
   ```csharp
   _logger.LogInformation("Generated {TokenCount} tokens", response.Usage.TotalTokens);
   ```

2. **Log Levels**:
   - `LogTrace`: API request details
   - `LogInformation`: Successful operations
   - `LogWarning`: Rate limits, retries
   - `LogError`: API errors, configuration issues

3. **Sensitive Data**: Never log full prompts or API keys
   ```csharp
   // Good: Log truncated prompt
   logger.LogInformation("Processing prompt: {Prompt}...", prompt[..50]);
   
   // Bad: Don't do this!
   logger.LogInformation("Using key: {Key}", apiKey);
   ```

## Common Exception Types

Handle these specific Azure OpenAI exceptions:
- `RequestFailedException`: Base exception for Azure SDK errors
- `Status 429`: Rate limiting
- `Status 401`: Authentication issues
- `Status 400`: Invalid requests (bad prompts/parameters)

## Pro Tips

- Add retry policies for transient errors
- Use correlation IDs for request tracking
- Monitor error rates in Application Insights
- Set up alerts for error thresholds

Remember: Good error handling makes your app more reliable and easier to debug. Don't wait for production issues to implement proper logging!
