using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;
using Shared;
using Web.Services;

namespace Web.Pages.Tips;

[OutputCache(PolicyName = "SearchResults")]
public class IndexModel : BasePageModel
{
    private readonly IContentService _contentService;
    private readonly ILogger<IndexModel> _logger;      // Use default cache duration for tips list (6 hours) since new tips are added infrequently
    // protected override int CacheDurationSeconds => base.CacheDurationSeconds; // 6 hours default

    public IndexModel(IContentService contentService, ILogger<IndexModel> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    public TipListViewModel ViewModel { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Tag { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Difficulty { get; set; }    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync()
    {
        // For search results, prevent aggressive browser caching
        if (!string.IsNullOrEmpty(Search) || !string.IsNullOrEmpty(Category) || 
            !string.IsNullOrEmpty(Tag) || !string.IsNullOrEmpty(Difficulty) || PageNumber > 1)
        {
            Response.Headers.CacheControl = "no-cache, must-revalidate";
            Response.Headers.Pragma = "no-cache";
        }

        try
        {
            var request = new TipSearchRequest
            {
                Category = Category,
                Tag = Tag,
                Search = Search,
                Difficulty = Difficulty,
                Page = Math.Max(1, PageNumber),
                PageSize = 12
            };

            var searchResult = await _contentService.SearchTipsAsync(request);

            // Get filter options
            var categories = await _contentService.GetCategoriesAsync();
            var tags = await _contentService.GetTagsAsync();

            ViewModel = new TipListViewModel
            {
                Tips = searchResult.Tips,
                Categories = categories,
                Tags = tags,
                SelectedCategory = Category,
                SelectedTag = Tag,
                SearchTerm = Search,
                Page = PageNumber,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount
            };

            ViewData["Title"] = "AI Tips & Tricks";
            ViewData["Description"] = "Discover the latest tips and tricks for Microsoft Copilot, GitHub Copilot, Azure AI, and more AI productivity tools.";

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tips list");
            return RedirectToPage("/Error");
        }
    }
}
