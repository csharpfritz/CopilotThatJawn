# Technical Architecture

## Technology Stack

"Copilot That Jawn" is built using modern web technologies with a focus on performance, maintainability, and scalability:

- **Backend Framework**: ASP.NET Core 9.0+ with .NET Aspire
- **Service Orchestration**: .NET Aspire AppHost for service management
- **Caching**: Redis distributed caching with StackExchange.Redis
- **Frontend**: Bootstrap 5 with custom CSS using Grid/Flexbox
- **Content Processing**: Markdown (using Markdig library) with YAML frontmatter (using YamlDotNet)
- **JavaScript**: Vanilla JS with modern ES6+ features
- **Build/Development**: .NET CLI with hot reload via `dotnet watch` through Aspire

## Application Architecture

The application follows a clean architecture pattern with separation of concerns:

### Web Layer

- **Razor Pages**: Used for content-focused pages with minimal logic
- **MVC Controllers**: Used for API endpoints and more complex interactions
- **View Components**: Reusable UI components across pages

### Business Layer

- **Services**: Business logic, content processing, and external integrations
- **Repositories**: Data access patterns for any database interactions

### Data Layer

- **Content Files**: Markdown files with YAML frontmatter for structured content
- **Database** (if implemented): Entity Framework Core with SQL Server or SQLite

## Content Management

The content management approach prioritizes developer-friendly workflows:

1. **Markdown Files**: All content is stored as Markdown files in the `Content/` directory
2. **YAML Frontmatter**: Each file contains metadata like title, tags, categories, and publication date
3. **Content Service**: The `ContentService` class processes these files and provides them to the application
4. **Caching**: Content is cached for performance while allowing for easy updates

## Site Generation

The site combines dynamic content processing with static asset optimization:

1. **Content Processing**: Markdown files are parsed at runtime by the ContentService
2. **Dynamic Routes**: Content is served through dynamic routes like `/tips/{slug}`
3. **SEO Features**: Sitemap.xml generation, meta tags, and OpenGraph support
4. **Performance Optimization**: Response compression, caching strategies, and optimized assets

## Deployment and Hosting

The application is designed to be deployed to:

- **Azure App Service**: Primary hosting option
- **Docker Containers**: Containerization support
- **Local Development**: Full support for local development with hot reload

## Diagrams

### Application Flow

```
User Request → ASP.NET Core Pipeline → Razor Pages/Controllers → ContentService → Markdown Processing → HTML Response
```

### Content Processing Flow

```
Markdown Files → YAML Frontmatter Parsing → Markdown Processing → HTML Generation → View Rendering → User Response
```

### Component Relationships

```
Razor Pages ↔ ContentService ↔ Markdown Files
Controllers ↔ ContentService ↔ Markdown Files
```

## .NET Aspire Architecture

The application is built using .NET Aspire, Microsoft's cloud-native application stack, which provides:

- **Service Orchestration**: AppHost manages all services including Redis and Azure Storage
- **Service Discovery**: Automatic service discovery and configuration
- **Resource Management**: Simplified provisioning of external dependencies
- **Observability**: Built-in logging, metrics, and distributed tracing
- **Development Experience**: Aspire dashboard for real-time monitoring

### Service Dependencies

The AppHost configures the following services:

```csharp
// Redis cache with persistent storage and RedisInsight
var redis = builder.AddRedis("redis")
    .WithRedisInsight()
    .WithLifetime(ContainerLifetime.Persistent)
    .PublishAsConnectionString();

// Azure Storage emulator for development
var storage = builder.AddAzureStorage("azure-storage")
    .RunAsEmulator();

var tables = storage.AddTables("tables");

// Web application with dependencies
var web = builder.AddProject<Web>("web")
    .WithReference(tables)
    .WithReference(redis)
    .WaitFor(tables)
    .WaitFor(redis);
```

## Caching Architecture

The application implements a multi-layered caching strategy using Redis as the primary distributed cache:

### Cache Layers

1. **Local Memory Cache (L1)**: Short-term local caching (5 minutes) for frequently accessed data
2. **Redis Distributed Cache (L2)**: Primary cache layer (6 hours) for shared data across instances
3. **Output Cache**: Redis-backed output caching for rendered pages and responses
4. **Response Cache**: HTTP response caching with appropriate cache headers

### Redis Configuration

#### Aspire Integration
```csharp
// Add Redis distributed caching through Aspire
builder.AddRedisDistributedCache("redis");
builder.AddRedisOutputCache("redis");

// Configure Redis key prefixes for better organization
builder.Services.PostConfigure<RedisCacheOptions>(options =>
{
    options.InstanceName = "CopilotThatJawn:";
});
```

#### Cache Policies
The application defines several output cache policies:

- **Default Policy**: 6-hour cache with host and query string variation
- **Static Content**: 3-day cache for static pages
- **Tips Content**: 3-day cache with slug-based variation
- **Dynamic Content**: 6-hour cache for frequently updated content

### Cache Implementation

#### ContentService Caching Strategy
```csharp
private async Task<List<TipModel>> GetTipsFromCacheAsync()
{
    // L1: Check local memory cache first (5 minutes)
    if (_cache.TryGetValue(TIPS_CACHE_KEY, out List<TipModel>? localTips))
        return localTips ?? new List<TipModel>();

    // L2: Check Redis distributed cache (6 hours)
    var distributedTipsJson = await _distributedCache.GetStringAsync(TIPS_CACHE_KEY);
    if (!string.IsNullOrEmpty(distributedTipsJson))
    {
        var tips = JsonSerializer.Deserialize<List<TipModel>>(distributedTipsJson);
        _cache.Set(TIPS_CACHE_KEY, tips, _localCacheExpiry); // Populate L1
        return tips;
    }

    // Cache miss: Load from database and populate both cache layers
    tips = await GetTipsFromAzureTableAsync();
    
    // Store in Redis (L2)
    var serializedTips = JsonSerializer.Serialize(tips);
    await _distributedCache.SetStringAsync(TIPS_CACHE_KEY, serializedTips, 
        new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(6) });
    
    // Store in local cache (L1)
    _cache.Set(TIPS_CACHE_KEY, tips, TimeSpan.FromMinutes(5));
    
    return tips;
}
```

#### Cache Invalidation
The application provides multiple cache invalidation mechanisms:

1. **Manual Invalidation**: Through the ContentService
2. **API Endpoint**: `/api/cache/refresh` with API key authentication
3. **Output Cache Tags**: Tag-based invalidation for page-level caching

### Redis Development Setup

#### Local Development
- Redis runs as a Docker container managed by Aspire
- RedisInsight dashboard available for cache inspection
- Persistent storage ensures data survives container restarts

#### Connection Configuration
- Connection strings managed through Aspire service discovery
- Environment-specific configuration through `appsettings.json`
- Secure parameter handling for production deployments
