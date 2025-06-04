using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

public class TipsController : Controller
{
    private readonly IContentService _contentService;
    private readonly ILogger<TipsController> _logger;

    public TipsController(IContentService contentService, ILogger<TipsController> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    /// <summary>
    /// API endpoint for searching tips (for AJAX calls)
    /// </summary>
    [HttpGet]
    [Route("api/tips/search")]
    public async Task<IActionResult> SearchApi([FromQuery] TipSearchRequest request)
    {
        try
        {
            var tips = await _contentService.SearchTipsAsync(request);
            
            var dtos = tips.Select(t => new TipDto
            {
                Title = t.Title,
                Slug = t.Slug,
                Category = t.Category,
                Tags = t.Tags,
                Difficulty = t.Difficulty,
                Author = t.Author,
                PublishedDate = t.PublishedDate,
                Description = t.Description,
                ReadingTimeMinutes = t.ReadingTimeMinutes,
                UrlSlug = t.UrlSlug
            }).ToList();

            return Json(new { tips = dtos, totalCount = dtos.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in search API");
            return BadRequest(new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Get all categories (for filter dropdowns)
    /// </summary>
    [HttpGet]
    [Route("api/tips/categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _contentService.GetCategoriesAsync();
            return Json(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            return BadRequest(new { error = "Failed to load categories" });
        }
    }

    /// <summary>
    /// Get all tags (for filter dropdowns)
    /// </summary>
    [HttpGet]
    [Route("api/tips/tags")]
    public async Task<IActionResult> GetTags()
    {
        try
        {
            var tags = await _contentService.GetTagsAsync();
            return Json(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tags");
            return BadRequest(new { error = "Failed to load tags" });
        }
    }

    /// <summary>
    /// Refresh content cache (for admin use)
    /// </summary>
    [HttpPost]
    [Route("api/tips/refresh")]
    public async Task<IActionResult> RefreshContent()
    {
        try
        {
            await _contentService.RefreshContentAsync();
            return Json(new { success = true, message = "Content refreshed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing content");
            return BadRequest(new { error = "Failed to refresh content" });
        }
    }
}
