# Redis Configuration and Caching Strategy

This document provides detailed information about the Redis implementation and caching strategy used in "Copilot That Jawn".

## Overview

The application uses Redis as a distributed cache to improve performance and scalability. Redis is integrated through .NET Aspire's orchestration system and provides multiple caching layers.

## Architecture

### Multi-Layer Caching Strategy

The application implements a sophisticated caching hierarchy:

1. **Local Memory Cache (L1)** - 5-minute in-memory cache for hot data
2. **Redis Distributed Cache (L2)** - 6-hour distributed cache for shared data
3. **Output Cache** - Redis-backed page-level caching
4. **HTTP Response Cache** - Client-side caching with appropriate headers

### Cache Flow Diagram

```text
Request → Check L1 (Memory) → Check L2 (Redis) → Database → Cache Population
    ↓           ↓                    ↓              ↓            ↑
  Cache Hit   Cache Hit           Cache Hit    Cache Miss      Populate All Layers
```

## .NET Aspire Integration

### AppHost Configuration

Redis is configured in the `AppHost` project with the following setup:

```csharp
// AppHost/AppHost.cs
var redis = builder.AddRedis("redis")
    .WithRedisInsight()                    // Enable RedisInsight dashboard
    .WithLifetime(ContainerLifetime.Persistent)  // Persist data across restarts
    .PublishAsConnectionString();          // Make connection available to services

var web = builder.AddProject<Web>("web")
    .WithReference(redis)                  // Inject Redis connection
    .WaitFor(redis);                      // Ensure Redis starts first
```

### Service Registration

In the Web project, Redis services are registered:

```csharp
// Web/Program.cs
builder.AddRedisDistributedCache("redis");  // Distributed caching
builder.AddRedisOutputCache("redis");       // Output caching

// Configure Redis options
builder.Services.PostConfigure<RedisCacheOptions>(options =>
{
    options.InstanceName = "CopilotThatJawn:";  // Key prefix for organization
});
```

## Implementation Details

### ContentService Caching

The primary caching implementation is in the `ContentService` class:

```csharp
public class ContentService : IContentService
{
    private const string TIPS_CACHE_KEY = "content_tips";
    private static readonly TimeSpan _distributedCacheExpiry = TimeSpan.FromHours(6);
    private static readonly TimeSpan _localCacheExpiry = TimeSpan.FromMinutes(5);

    private async Task<List<TipModel>> GetTipsFromCacheAsync()
    {
        // L1: Check local memory cache first
        if (_cache.TryGetValue(TIPS_CACHE_KEY, out List<TipModel>? localTips))
        {
            return localTips ?? new List<TipModel>();
        }

        // L2: Check Redis distributed cache
        var distributedTipsJson = await _distributedCache.GetStringAsync(TIPS_CACHE_KEY);
        List<TipModel> tips;

        if (!string.IsNullOrEmpty(distributedTipsJson))
        {
            // Cache hit in Redis
            tips = JsonSerializer.Deserialize<List<TipModel>>(distributedTipsJson) ?? new List<TipModel>();
            _logger.LogDebug("Loaded {Count} tips from Redis cache", tips.Count);
        }
        else
        {
            // Cache miss - load from database
            tips = await GetTipsFromAzureTableAsync();
            
            // Store in Redis
            var serializedTips = JsonSerializer.Serialize(tips);
            var distributedCacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = _distributedCacheExpiry
            };
            await _distributedCache.SetStringAsync(TIPS_CACHE_KEY, serializedTips, distributedCacheOptions);
            _logger.LogInformation("Cached {Count} tips in Redis", tips.Count);
        }

        // Store in local memory cache
        _cache.Set(TIPS_CACHE_KEY, tips, _localCacheExpiry);
        return tips;
    }
}
```

### Output Caching Policies

The application defines several output cache policies:

```csharp
// Configure output cache policies
builder.Services.Configure<OutputCacheOptions>(options =>
{
    // Default policy: 6-hour cache
    options.AddBasePolicy(builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByQuery("*")
               .Expire(TimeSpan.FromHours(6))
               .Tag("outputcache", "site"));
                 
    // Static content: 3-day cache
    options.AddPolicy("StaticContent", builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .Expire(TimeSpan.FromDays(3))
               .Tag("outputcache", "static"));
               
    // Tips content: 3-day cache with slug variation
    options.AddPolicy("TipsContent", builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByRouteValue("slug")
               .Expire(TimeSpan.FromDays(3))
               .Tag("outputcache", "tips", "content"));
});
```

## Development Experience

### Local Development Setup

When running the application locally:

1. **Automatic Redis Setup**: Aspire automatically starts a Redis container
2. **RedisInsight Dashboard**: Available through the Aspire dashboard for cache inspection
3. **Persistent Storage**: Redis data persists across application restarts
4. **Connection Management**: Connection strings are automatically configured

### Aspire Dashboard Features

The Aspire dashboard provides:

- **Service Status**: Monitor Redis container health
- **Connection Strings**: View Redis connection details
- **Logs**: Redis server logs and application cache activity
- **Metrics**: Cache hit/miss ratios and performance metrics

### RedisInsight Integration

RedisInsight is automatically available and provides:

- **Key Browser**: Explore cached keys and their values
- **Performance Monitoring**: Monitor Redis performance metrics  
- **Memory Usage**: Track Redis memory consumption
- **Command Analysis**: Analyze Redis commands and their performance

## Cache Management

### Manual Cache Invalidation

The application provides several methods for cache invalidation:

#### Through ContentService

```csharp
public async Task InvalidateTipsCacheAsync()
{
    // Clear distributed cache
    await _distributedCache.RemoveAsync(TIPS_CACHE_KEY);
    
    // Clear local memory cache
    _cache.Remove(TIPS_CACHE_KEY);
    
    // Clear output cache for tips pages
    await _outputCacheStore.EvictByTagAsync("tips", default);
    await _outputCacheStore.EvictByTagAsync("content", default);
}
```

#### Via API Endpoint

```bash
# Refresh cache via secured API endpoint
curl -X POST https://localhost:5001/api/cache/refresh \
  -H "X-API-Key: your-api-key"
```

The API endpoint provides comprehensive cache refresh:

- Refreshes content from the database
- Updates both local and distributed caches
- Invalidates output cache entries
- Returns detailed refresh status

### Cache Refresh Process

When content is refreshed:

1. **Load Fresh Data**: Retrieve latest content from Azure Table Storage
2. **Update Redis**: Store serialized content in Redis with 6-hour expiration
3. **Update Local Cache**: Store in memory cache with 5-minute expiration
4. **Invalidate Output Cache**: Clear page-level caches using tags
5. **Log Activity**: Record cache refresh operations for monitoring

## Production Considerations

### Connection Configuration

In production, Redis connection is configured through:

- **Environment Variables**: `ConnectionStrings__redis` contains the Redis connection string
- **Azure Configuration**: Connection strings managed through Azure App Configuration
- **Secure Parameters**: Sensitive connection details secured through Azure Key Vault

### Performance Optimization

- **Connection Pooling**: StackExchange.Redis manages connection pooling automatically
- **Serialization**: JSON serialization for compatibility and performance
- **Key Expiration**: Sliding expiration policies to balance freshness and performance
- **Memory Management**: Appropriate eviction policies for memory-constrained environments

### Monitoring and Alerting

Production monitoring should include:

- **Cache Hit Ratios**: Monitor effectiveness of caching strategy
- **Redis Memory Usage**: Ensure Redis doesn't exceed memory limits
- **Connection Health**: Monitor Redis connectivity and latency
- **Cache Refresh Frequency**: Track how often content is being refreshed

## Troubleshooting

### Common Issues

#### Redis Connection Failed
```bash
# Check if Redis is running through Aspire
dotnet run --project AppHost --verbose

# Verify Redis container status in Aspire dashboard
```

#### Cache Not Updating
```bash
# Force cache refresh via API
curl -X POST https://localhost:5001/api/cache/refresh

# Check logs for cache invalidation
```

#### Performance Issues
- Monitor cache hit ratios in application logs
- Use RedisInsight to analyze key usage patterns
- Consider adjusting cache expiration times

### Debugging Cache Behavior

Enable verbose logging for cache operations:

```json
{
  "Logging": {
    "LogLevel": {
      "Web.Services.ContentService": "Debug",
      "Microsoft.Extensions.Caching": "Debug"
    }
  }
}
```

## Best Practices

### Cache Key Design

- **Consistent Prefixes**: Use "content_" prefix for content-related keys
- **Descriptive Names**: Keys should clearly indicate their content
- **Version Considerations**: Include version information if cache format changes

### Expiration Strategy

- **Sliding Expiration**: Use sliding expiration for frequently accessed data
- **Absolute Expiration**: Use absolute expiration for time-sensitive content
- **Layered TTL**: Different TTL values for different cache layers

### Error Handling

- **Graceful Degradation**: Application should work even if Redis is unavailable
- **Fallback Strategies**: Always have a fallback to database queries
- **Retry Logic**: Implement appropriate retry logic for Redis operations

## Configuration Reference

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__redis` | Redis connection string | Configured by Aspire |
| `CacheRefresh__ApiKey` | API key for cache refresh endpoint | Development: none, Production: required |

### Cache Configuration

| Setting | Value | Purpose |
|---------|-------|---------|
| Local Cache TTL | 5 minutes | Hot data caching |
| Redis Cache TTL | 6 hours | Distributed caching |
| Output Cache TTL | 3 days (content), 6 hours (dynamic) | Page-level caching |
| Redis Instance Name | "CopilotThatJawn:" | Key organization |

This configuration provides optimal performance while ensuring content freshness and system reliability.
