---
title: "Azure AI Services Security & Monitoring Guide"
category: "Azure AI"
tags: ["azure", "ai-services", "security", "monitoring", "devops"]
difficulty: "Advanced"
author: "Copilot That Jawn Team"
publishedDate: "2025-06-07"
lastModified: "2025-06-07"
description: "Learn how to implement robust security measures and monitoring for your Azure AI services. From error handling to key management and performance tracking."
---

# Azure AI Services Security & Monitoring Guide

Building AI-powered apps is cool, but keeping them secure and well-monitored is crucial. This guide will show you how to lock down your Azure AI services and keep tabs on everything that's happening. Let's make this jawn bulletproof!

## Error Handling Middleware

First, let's implement comprehensive error handling:

```csharp
public class AzureAIExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AzureAIExceptionMiddleware> _logger;
    private readonly TelemetryClient _telemetry;

    public AzureAIExceptionMiddleware(
        RequestDelegate next,
        ILogger<AzureAIExceptionMiddleware> logger,
        TelemetryClient telemetry)
    {
        _next = next;
        _logger = logger;
        _telemetry = telemetry;
    }

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
            TrackException(ex, "RateLimit");
        }
        catch (RequestFailedException ex) when (ex.Status == 401)
        {
            _logger.LogError("Authentication failed for Azure AI service");
            await HandleAuthErrorAsync(context);
            TrackException(ex, "Authentication");
        }
        catch (RequestFailedException ex) when (ex.Status == 400)
        {
            _logger.LogError("Invalid request to Azure AI service");
            await HandleBadRequestAsync(context);
            TrackException(ex, "BadRequest");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Azure AI service");
            await HandleUnexpectedErrorAsync(context);
            TrackException(ex, "Unexpected");
        }
    }

    private void TrackException(Exception ex, string category)
    {
        _telemetry.TrackException(ex, new Dictionary<string, string>
        {
            { "Category", category },
            { "ServiceName", "AzureAI" }
        });
    }

    private async Task HandleRateLimitAsync(HttpContext context)
    {
        context.Response.StatusCode = 429;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Rate limit exceeded. Please try again later.",
            retryAfter = "60"
        });
    }

    private async Task HandleAuthErrorAsync(HttpContext context)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Authentication failed. Please check your credentials."
        });
    }

    private async Task HandleBadRequestAsync(HttpContext context)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Invalid request. Please check your input."
        });
    }

    private async Task HandleUnexpectedErrorAsync(HttpContext context)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred. Please try again later."
        });
    }
}
```

## Application Insights Integration

Set up comprehensive monitoring with Application Insights:

```csharp
public class AIServiceTelemetry
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<AIServiceTelemetry> _logger;

    public AIServiceTelemetry(
        TelemetryClient telemetryClient,
        ILogger<AIServiceTelemetry> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public void TrackAIServiceCall(
        string serviceName,
        string operation,
        double duration,
        bool success,
        int? statusCode = null)
    {
        // Track as dependency
        _telemetryClient.TrackDependency(
            "AzureAI",
            serviceName,
            operation,
            DateTime.UtcNow.AddMilliseconds(-duration),
            TimeSpan.FromMilliseconds(duration),
            success);

        // Track custom metrics
        _telemetryClient.TrackMetric(
            $"AzureAI.{serviceName}.Duration",
            duration,
            new Dictionary<string, string>
            {
                { "Operation", operation },
                { "Success", success.ToString() }
            });

        if (statusCode.HasValue)
        {
            _telemetryClient.TrackMetric(
                $"AzureAI.{serviceName}.StatusCode",
                statusCode.Value,
                new Dictionary<string, string>
                {
                    { "Operation", operation }
                });
        }

        _logger.LogInformation(
            "AI Service call - Service: {Service}, Operation: {Operation}, " +
            "Duration: {Duration}ms, Success: {Success}, StatusCode: {StatusCode}",
            serviceName, operation, duration, success, statusCode);
    }
}
```

## Key Management

Implement secure key management with Azure Key Vault:

```csharp
public class SecureKeyProvider
{
    private readonly SecretClient _secretClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SecureKeyProvider> _logger;
    private const int KeyCacheMinutes = 15;

    public SecureKeyProvider(
        SecretClient secretClient,
        IMemoryCache cache,
        ILogger<SecureKeyProvider> logger)
    {
        _secretClient = secretClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GetApiKeyAsync(string keyName)
    {
        var cacheKey = $"apikey_{keyName}";

        if (_cache.TryGetValue(cacheKey, out string cachedKey))
        {
            return cachedKey;
        }

        try
        {
            var secret = await _secretClient.GetSecretAsync(keyName);
            var key = secret.Value.Value;

            _cache.Set(cacheKey, key, TimeSpan.FromMinutes(KeyCacheMinutes));
            return key;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve API key: {KeyName}", keyName);
            throw;
        }
    }
}
```

## Cost Management & Monitoring

Track usage and costs with custom metrics:

```csharp
public class AICostMonitor
{
    private readonly TelemetryClient _telemetry;
    private readonly ILogger<AICostMonitor> _logger;

    public AICostMonitor(
        TelemetryClient telemetry,
        ILogger<AICostMonitor> logger)
    {
        _telemetry = telemetry;
        _logger = logger;
    }

    public void TrackUsage(
        string service,
        string operation,
        int units,
        decimal estimatedCost)
    {
        _telemetry.TrackMetric("AI.Usage", units, new Dictionary<string, string>
        {
            { "Service", service },
            { "Operation", operation }
        });

        _telemetry.TrackMetric("AI.Cost", (double)estimatedCost, 
            new Dictionary<string, string>
        {
            { "Service", service },
            { "Operation", operation }
        });

        _logger.LogInformation(
            "AI Usage - Service: {Service}, Operation: {Operation}, " +
            "Units: {Units}, Estimated Cost: ${Cost}",
            service, operation, units, estimatedCost);
    }
}
```

## Performance Monitoring

Implement a health check for your AI services:

```csharp
public class AzureAIHealthCheck : IHealthCheck
{
    private readonly TextAnalyticsClient _textAnalytics;
    private readonly OpenAIClient _openAI;
    private readonly ILogger<AzureAIHealthCheck> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        try
        {
            // Check Text Analytics
            await _textAnalytics.DetectLanguageAsync("Test");
        }
        catch (Exception ex)
        {
            errors.Add($"Text Analytics error: {ex.Message}");
        }

        try
        {
            // Check OpenAI
            await _openAI.GetDeploymentsAsync();
        }
        catch (Exception ex)
        {
            errors.Add($"OpenAI error: {ex.Message}");
        }

        if (errors.Any())
        {
            return HealthCheckResult.Unhealthy(
                "One or more AI services are not responding",
                data: new Dictionary<string, object>
                {
                    { "Errors", errors }
                });
        }

        return HealthCheckResult.Healthy("All AI services are operational");
    }
}
```

## Alert Configuration

Set up Azure Monitor alerts:

```json
{
  "name": "AI Service Rate Limit Alert",
  "type": "Microsoft.Insights/metricAlerts",
  "properties": {
    "description": "Triggers when rate limit is exceeded",
    "severity": 2,
    "enabled": true,
    "scopes": ["<your-resource-id>"],
    "evaluationFrequency": "PT1M",
    "windowSize": "PT5M",
    "criteria": {
      "odata.type": "Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria",
      "allOf": [
        {
          "name": "Rate limit exceeded",
          "metricName": "RateLimitExceeded",
          "operator": "GreaterThan",
          "threshold": 5,
          "timeAggregation": "Count"
        }
      ]
    },
    "actions": [
      {
        "actionGroupId": "<your-action-group-id>"
      }
    ]
  }
}
```

## Security Best Practices

1. **Identity & Access Management**
   - Use Managed Identities whenever possible
   - Implement least-privilege access
   - Regularly audit access permissions

2. **Network Security**
   - Use private endpoints for AI services
   - Implement network isolation
   - Configure service endpoints

3. **Key Management**
   - Store keys in Key Vault
   - Implement automatic key rotation
   - Monitor key usage

4. **Monitoring & Logging**
   - Enable diagnostic logging
   - Set up alerts for anomalies
   - Monitor authentication failures

5. **Cost Management**
   - Set up budgets and quotas
   - Monitor usage patterns
   - Implement cost allocation tags

## Implementation Checklist

- [ ] Enable Application Insights
- [ ] Configure Key Vault integration
- [ ] Set up health checks
- [ ] Implement error handling middleware
- [ ] Configure usage monitoring
- [ ] Set up cost tracking
- [ ] Configure alerting
- [ ] Enable diagnostic logging
- [ ] Set up automated testing
- [ ] Configure backup procedures

## Next Steps

1. Set up automated monitoring dashboards
2. Configure advanced alerting rules
3. Implement continuous testing
4. Set up cost optimization strategies
5. Plan for disaster recovery

Remember, security and monitoring aren't one-time setups - they need regular review and updates. Keep an eye on your metrics, adjust your alerts, and stay on top of any security advisories.

---

*Got security or monitoring tips to share? Submit a PR to our repo!*
