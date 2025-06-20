using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;
using Shared;
using Web.Services;

namespace Web.Pages;

[OutputCache(Duration = 21600, Tags = new[] { "home", "content", "tips" })]
public class IndexModel : BasePageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IContentService _contentService;
      // Use default cache duration for home page (6 hours) - good balance for fresh content
    // protected override int CacheDurationSeconds => base.CacheDurationSeconds; // 6 hours default
    
    public List<TipModel> RecentTips { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> PopularTags { get; set; } = new();
    public TipModel? FeaturedTip { get; set; }
    public int TotalTipsCount { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IContentService contentService)
    {
        _logger = logger;
        _contentService = contentService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            // Get all tips
            var allTips = await _contentService.GetAllTipsAsync();
            TotalTipsCount = allTips.Count;
            
            // Get the most recent tips
            RecentTips = allTips.OrderByDescending(t => t.PublishedDate).Take(6).ToList();
            
            // Select a featured tip - either one specifically tagged as featured, or the most recent GitHub Copilot related one
            FeaturedTip = allTips.FirstOrDefault(t => t.Tags.Contains("featured")) ?? 
                        allTips.FirstOrDefault(t => t.Category.Equals("GitHub Copilot", StringComparison.OrdinalIgnoreCase) || 
                                             t.Tags.Contains("github-copilot"));
            
            // Get all categories
            Categories = await _contentService.GetCategoriesAsync();
            
            // Get all tags and take the most used ones
            PopularTags = await _contentService.GetTagsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page content");
        }
    }
}
