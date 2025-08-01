using Markdig;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.OutputCaching;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Azure.Data.Tables;
using Shared;
using Web.Extensions;
using System.Text.Json;

namespace Web.Services;

/// <summary>
/// Service for managing markdown-based content with Redis distributed caching
/// </summary>
public class ContentService : IContentService
{    private readonly ILogger<ContentService> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IMemoryCache _cache; // Keep for very short-term local caching
    private readonly IDistributedCache _distributedCache; // Redis cache
    private readonly IOutputCacheStore _outputCacheStore; // Output cache store for invalidation
    private readonly MarkdownPipeline _markdownPipeline;
    private readonly IDeserializer _yamlDeserializer;
    private readonly TableClient _tableClient;
    private const string TIPS_CACHE_KEY = "content_tips";
    private static readonly TimeSpan _distributedCacheExpiry = TimeSpan.FromHours(6);
    private static readonly TimeSpan _localCacheExpiry = TimeSpan.FromMinutes(5); // Short local cache for frequently accessed data
      public ContentService(
        ILogger<ContentService> logger, 
        IWebHostEnvironment environment,
        IMemoryCache cache,
        IDistributedCache distributedCache,
        IOutputCacheStore outputCacheStore,
        TableServiceClient tableServiceClient)
    {
        _logger = logger;
        _environment = environment;
        _cache = cache;
        _distributedCache = distributedCache;
        _outputCacheStore = outputCacheStore;

        // Configure Markdig with image processing
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAutoLinks()
            .UseEmphasisExtras()
            .UseDefinitionLists()
            .UseFooters()
            .UseFootnotes()
            .UseCitations()
            .UseGridTables()
            .UsePipeTables()
            .UseListExtras()
            .UseMathematics()
            .UseMediaLinks()
            .UseTaskLists()
            .UseAutoIdentifiers()
            .UseAbbreviations()
            .UseCustomContainers()
            .UseFigures()
            .UseEmojiAndSmiley()
            .UseGenericAttributes()
            .Use(new ImageUrlRewriterExtension()) // Custom extension for image processing
            .Build();

        // Configure YAML deserializer
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        // Initialize Azure Table Client
        _tableClient = tableServiceClient.GetTableClient("Content");
    }

    public async Task<List<TipModel>> GetAllTipsAsync()
    {
        var tips = await GetTipsFromCacheAsync();
        return tips.OrderByDescending(t => t.PublishedDate).ToList();
    }

    public async Task<TipModel?> GetTipBySlugAsync(string slug)
    {
        var tips = await GetTipsFromCacheAsync();
        return tips.FirstOrDefault(t => 
            t.UrlSlug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<TipSearchResult> SearchTipsAsync(TipSearchRequest request)
    {
        var tips = await GetTipsFromCacheAsync();
        var query = tips.AsQueryable();

        // Filter by category
        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(t => t.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by tag
        if (!string.IsNullOrEmpty(request.Tag))
        {
            query = query.Where(t => t.Tags.Any(tag => tag.Equals(request.Tag, StringComparison.OrdinalIgnoreCase)));
        }

        // Filter by difficulty
        if (!string.IsNullOrEmpty(request.Difficulty))
        {
            query = query.Where(t => t.Difficulty.Equals(request.Difficulty, StringComparison.OrdinalIgnoreCase));
        }

        // Search in title, description, and content
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchTerm = request.Search.ToLowerInvariant();
            query = query.Where(t => 
                t.Title.ToLowerInvariant().Contains(searchTerm) ||
                t.Description.ToLowerInvariant().Contains(searchTerm) ||
                t.Content.ToLowerInvariant().Contains(searchTerm) ||
                t.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchTerm)));
        }

        // Order by published date (newest first)
        query = query.OrderByDescending(t => t.PublishedDate);

        // Get total count before pagination
        var totalCount = query.Count();
        
        // Apply pagination
        var results = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new TipSearchResult
        {
            Tips = results,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        var tips = await GetTipsFromCacheAsync();
        return tips
            .Select(t => t.Category)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public async Task<List<string>> GetTagsAsync()
    {
        var tips = await GetTipsFromCacheAsync();
        return tips
            .SelectMany(t => t.Tags)
            .Where(tag => !string.IsNullOrEmpty(tag))
            .Distinct()
            .OrderBy(tag => tag)
            .ToList();
    }

    public async Task<List<TipModel>> GetRelatedTipsAsync(TipModel tip, int count = 3)
    {
        var tips = await GetTipsFromCacheAsync();
        
        var relatedTips = tips
            .Where(t => t.UrlSlug != tip.UrlSlug)
            .Select(t => new
            {
                Tip = t,
                Score = CalculateRelatednessScore(tip, t)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(count)
            .Select(x => x.Tip)
            .ToList();

        return relatedTips;
    }    public async Task RefreshContentAsync()
    {
        _logger.LogInformation("Refreshing content cache...");
        
        try
        {
            var tips = await GetTipsFromAzureTableAsync();

            // Update Redis cache
            var serializedTips = JsonSerializer.Serialize(tips);
            var distributedCacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = _distributedCacheExpiry
            };
            await _distributedCache.SetStringAsync(TIPS_CACHE_KEY, serializedTips, distributedCacheOptions);

            // Update local cache
            var localCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_localCacheExpiry)
                .SetPriority(CacheItemPriority.Normal)
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _logger.LogDebug("Local content cache evicted. Reason: {Reason}", reason);
                });

            _cache.Set(TIPS_CACHE_KEY, tips, localCacheEntryOptions);
            _logger.LogInformation("Content cache refreshed. Loaded {Count} tips", tips.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing content cache");
            throw;
        }
    }

    private async Task<List<TipModel>> GetTipsFromAzureTableAsync()
    {
        var tips = new List<TipModel>();

        await foreach (var entity in _tableClient.QueryAsync<ContentEntity>())
        {
            try
            {
                // Convert Markdown content to HTML
                var htmlContent = Markdown.ToHtml(entity.Content, _markdownPipeline);
                htmlContent = PostProcessCodeBlocks(htmlContent);

                var tip = new TipModel
                {
                    Title = entity.Title,
                    Description = entity.Description,
                    Content = htmlContent, // Store the converted HTML
                    Category = entity.Category,
                    Tags = entity.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                    PublishedDate = entity.PublishedDate,
                    Author = entity.Author,
                    Difficulty = entity.Difficulty,
                    FileName = entity.FileName,
                    LastModified = entity.Timestamp?.UtcDateTime ?? entity.PublishedDate,
                    UrlSlug = !string.IsNullOrWhiteSpace(entity.Slug) ? entity.Slug : entity.RowKey // Set UrlSlug
                };
                tips.Add(tip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing entity from Azure Table");
            }
        }

        return tips;
    }    private async Task<List<TipModel>> GetTipsFromCacheAsync()
    {
        // First, check local memory cache for very recent data
        if (_cache.TryGetValue(TIPS_CACHE_KEY, out List<TipModel>? localTips))
        {
            return localTips ?? new List<TipModel>();
        }

        // Check Redis distributed cache
        var distributedTipsJson = await _distributedCache.GetStringAsync(TIPS_CACHE_KEY);
        List<TipModel> tips;

        if (!string.IsNullOrEmpty(distributedTipsJson))
        {
            try
            {
                tips = JsonSerializer.Deserialize<List<TipModel>>(distributedTipsJson) ?? new List<TipModel>();
                _logger.LogDebug("Loaded {Count} tips from Redis cache", tips.Count);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize tips from Redis cache, falling back to database");
                tips = await GetTipsFromAzureTableAsync();
            }
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

        // Store in local memory cache for very short term
        _cache.Set(TIPS_CACHE_KEY, tips, _localCacheExpiry);

        return tips;
    }

    private T? GetMetadataValue<T>(Dictionary<string, object> metadata, string key)
    {
        if (metadata.TryGetValue(key, out var value) && value != null)
        {
            if (value is T directValue)
                return directValue;
            
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
        return default(T);
    }

    private List<string> ParseTags(object? tagsValue)
    {
        if (tagsValue == null) return new List<string>();

        if (tagsValue is List<object> objectList)
        {
            return objectList.Select(t => t?.ToString() ?? "").Where(t => !string.IsNullOrEmpty(t)).ToList();
        }

        if (tagsValue is string tagsString)
        {
            return tagsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .Where(t => !string.IsNullOrEmpty(t))
                            .ToList();
        }

        return new List<string>();
    }

    private DateTime ParseDate(object? dateValue)
    {
        if (dateValue == null) return DateTime.UtcNow;

        if (dateValue is DateTime dateTime)
            return dateTime;

        if (dateValue is string dateString && DateTime.TryParse(dateString, out var parsedDate))
            return parsedDate;

        return DateTime.UtcNow;
    }

    private int CalculateRelatednessScore(TipModel tip1, TipModel tip2)
    {
        var score = 0;

        // Same category = +10 points
        if (tip1.Category.Equals(tip2.Category, StringComparison.OrdinalIgnoreCase))
            score += 10;

        // Shared tags = +5 points per tag
        var sharedTags = tip1.Tags.Intersect(tip2.Tags, StringComparer.OrdinalIgnoreCase);
        score += sharedTags.Count() * 5;

        // Same difficulty = +3 points
        if (tip1.Difficulty.Equals(tip2.Difficulty, StringComparison.OrdinalIgnoreCase))
            score += 3;

        // Same author = +2 points
        if (tip1.Author.Equals(tip2.Author, StringComparison.OrdinalIgnoreCase))
            score += 2;

        return score;
    }

	private string PostProcessCodeBlocks(string htmlContent)
	{
		// Ensure proper CSS classes for Prism.js syntax highlighting
		// Handle fenced code blocks with language specification
		// Pattern 1: <pre><code class="language-xxx"> -> <pre class="language-xxx"><code class="language-xxx">
		var pattern1 = @"<pre><code class=""language-(\w+)"">";
		var replacement1 = @"<pre class=""language-$1""><code class=""language-$1"">";
		htmlContent = Regex.Replace(htmlContent, pattern1, replacement1);
		return htmlContent;
	}

    private string ProcessImagesInContent(string content, List<ImageInfo> images)
    {
        // Process markdown image syntax: ![alt](url "caption")
        var imagePattern = new Regex(@"!\[([^\]]*)\]\(([^)\s]+)(?:\s+""([^""]*)"")?\)");
        return imagePattern.Replace(content, match =>
        {
            var altText = match.Groups[1].Value;
            var url = match.Groups[2].Value;
            var caption = match.Groups[3].Success ? match.Groups[3].Value : null;

            // If it's already a full URL, leave it as is
            if (Uri.TryCreate(url, UriKind.Absolute, out _))
                return match.Value;

            // Find matching image in our processed list
            var image = images.FirstOrDefault(i => i.FileName == url);
            if (image == null)
                return match.Value; // Keep original if not found

            // Build new markdown with processed URL
            var captionPart = !string.IsNullOrEmpty(caption) ? $" \"{caption}\"" : "";
            return $"![{altText}]({image.PublicUrl}{captionPart})";
        });
    }

    /// <summary>
    /// Invalidate all tips-related cache entries
    /// </summary>
    public async Task InvalidateTipsCacheAsync()
    {
        try
        {
            // Clear distributed cache
            await _distributedCache.RemoveAsync(TIPS_CACHE_KEY);
            
            // Clear local memory cache
            _cache.Remove(TIPS_CACHE_KEY);
            
            // Clear output cache for tips pages
            await _outputCacheStore.EvictByTagAsync("tips", default);
            await _outputCacheStore.EvictByTagAsync("content", default);
            
            _logger.LogInformation("Tips cache invalidated successfully");
        }        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating tips cache");
        }
    }
}
