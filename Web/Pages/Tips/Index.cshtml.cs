using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Models;
using Web.Services;

namespace Web.Pages.Tips;

public class IndexModel : PageModel
{
    private readonly IContentService _contentService;
    private readonly ILogger<IndexModel> _logger;

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

            var tips = await _contentService.SearchTipsAsync(request);
            var totalCount = tips.Count;

            // Get filter options
            var categories = await _contentService.GetCategoriesAsync();
            var tags = await _contentService.GetTagsAsync();

            ViewModel = new TipListViewModel
            {
                Tips = tips,
                Categories = categories,
                Tags = tags,
                SelectedCategory = Category,
                SelectedTag = Tag,
                SearchTerm = Search,
                Page = PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
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
