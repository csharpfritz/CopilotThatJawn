using Markdig;
using System.Text.RegularExpressions;
using Web.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Web.Services;

/// <summary>
/// Service for managing markdown-based content
/// </summary>
public class ContentService : IContentService
{
    private readonly ILogger<ContentService> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly MarkdownPipeline _markdownPipeline;
    private readonly IDeserializer _yamlDeserializer;
    private List<TipModel> _cachedTips = new();
    private DateTime _lastRefresh = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);    public ContentService(ILogger<ContentService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;        // Configure Markdig with extensions for syntax highlighting
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseEmojiAndSmiley()
            .UseGenericAttributes() // Enables CSS class attributes on code blocks
            .Build();

        // Configure YAML deserializer
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public async Task<List<TipModel>> GetAllTipsAsync()
    {
        await EnsureCacheIsCurrentAsync();
        return _cachedTips.OrderByDescending(t => t.PublishedDate).ToList();
    }

    public async Task<TipModel?> GetTipBySlugAsync(string slug)
    {
        await EnsureCacheIsCurrentAsync();
        return _cachedTips.FirstOrDefault(t => 
            t.UrlSlug.Equals(slug, StringComparison.OrdinalIgnoreCase) ||
            t.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<TipModel>> SearchTipsAsync(TipSearchRequest request)
    {
        await EnsureCacheIsCurrentAsync();
        
        var query = _cachedTips.AsQueryable();

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

        // Apply pagination
        var totalCount = query.Count();
        var tips = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return tips;
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        await EnsureCacheIsCurrentAsync();
        return _cachedTips
            .Select(t => t.Category)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public async Task<List<string>> GetTagsAsync()
    {
        await EnsureCacheIsCurrentAsync();
        return _cachedTips
            .SelectMany(t => t.Tags)
            .Where(tag => !string.IsNullOrEmpty(tag))
            .Distinct()
            .OrderBy(tag => tag)
            .ToList();
    }

    public async Task<List<TipModel>> GetRelatedTipsAsync(TipModel tip, int count = 3)
    {
        await EnsureCacheIsCurrentAsync();
        
        var relatedTips = _cachedTips
            .Where(t => t.Slug != tip.Slug)
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
    }

    public async Task RefreshContentAsync()
    {
        _logger.LogInformation("Refreshing content cache...");
        
        try
        {
            var contentPath = GetContentPath();
            var tipFiles = Directory.GetFiles(contentPath, "*.md", SearchOption.AllDirectories);
            var tips = new List<TipModel>();

            foreach (var filePath in tipFiles)
            {
                try
                {
                    var tip = await ParseMarkdownFileAsync(filePath);
                    if (tip != null)
                    {
                        tips.Add(tip);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing file {FilePath}", filePath);
                }
            }

            _cachedTips = tips;
            _lastRefresh = DateTime.UtcNow;
            
            _logger.LogInformation("Content cache refreshed. Loaded {Count} tips", tips.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing content cache");
            throw;
        }
    }

    private async Task EnsureCacheIsCurrentAsync()
    {
        if (_lastRefresh == DateTime.MinValue || DateTime.UtcNow - _lastRefresh > _cacheExpiry)
        {
            await RefreshContentAsync();
        }
    }

    private string GetContentPath()
    {
        // Look for Content directory in the solution root
        var webRoot = _environment.ContentRootPath;
        var solutionRoot = Directory.GetParent(webRoot)?.FullName;
        
        if (solutionRoot != null)
        {
            var contentPath = Path.Combine(solutionRoot, "Content", "Tips");
            if (Directory.Exists(contentPath))
            {
                return contentPath;
            }
        }

        // Fallback to relative path
        var fallbackPath = Path.Combine(webRoot, "..", "Content", "Tips");
        return Path.GetFullPath(fallbackPath);
    }    private async Task<TipModel?> ParseMarkdownFileAsync(string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        // Extract frontmatter and content
        var frontmatterMatch = Regex.Match(content, @"^---\s*\n(.*?)\n---\s*\n(.*)", 
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        if (!frontmatterMatch.Success)
        {
            _logger.LogWarning("No frontmatter found in file {FileName}", fileName);
            return null;
        }

        var yamlContent = frontmatterMatch.Groups[1].Value;
        var markdownContent = frontmatterMatch.Groups[2].Value;

        try
        {
            // Parse YAML frontmatter
            var metadata = _yamlDeserializer.Deserialize<Dictionary<string, object>>(yamlContent);
              // Convert markdown to HTML with syntax highlighting support
            var htmlContent = Markdown.ToHtml(markdownContent, _markdownPipeline);
            
            // Post-process to ensure proper CSS classes for syntax highlighting
            htmlContent = PostProcessCodeBlocks(htmlContent);
            
            // Create tip model
            var tip = new TipModel
            {
                Title = GetMetadataValue<string>(metadata, "title") ?? "Untitled",
                Slug = GetMetadataValue<string>(metadata, "slug") ?? fileName,
                Category = GetMetadataValue<string>(metadata, "category") ?? "General",
                Tags = ParseTags(GetMetadataValue<object>(metadata, "tags")),
                Difficulty = GetMetadataValue<string>(metadata, "difficulty") ?? "Beginner",
                Author = GetMetadataValue<string>(metadata, "author") ?? "Unknown",
                PublishedDate = ParseDate(GetMetadataValue<object>(metadata, "publishedDate")),
                LastModified = ParseDate(GetMetadataValue<object>(metadata, "lastModified")),
                Description = GetMetadataValue<string>(metadata, "description") ?? "",
                Content = htmlContent,
                FileName = fileName
            };

            return tip;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing metadata for file {FileName}", fileName);
            return null;
        }
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
        var pattern = @"<pre><code class=""language-(\w+)"">";
        var replacement = @"<pre class=""language-$1""><code class=""language-$1"">";
        htmlContent = Regex.Replace(htmlContent, pattern, replacement);
        
        return htmlContent;
    }
}
