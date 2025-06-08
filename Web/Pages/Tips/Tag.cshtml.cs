using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared;
using Web.Services;

namespace Web.Pages.Tips;

public class TagModel : BasePageModel
{
    private readonly IContentService _contentService;
    private readonly ILogger<TagModel> _logger;
    
    // Override cache duration for tag pages - cache for 5 minutes
    protected override int CacheDurationSeconds => 300;

    public TagModel(IContentService contentService, ILogger<TagModel> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    public TipListViewModel ViewModel { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string Tag { get; set; } = string.Empty;    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Tag))
        {
            return RedirectToPage("/Tips/Index");
        }

        try
        {
            var request = new TipSearchRequest
            {
                Tag = Tag,
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
                SelectedTag = Tag,
                Page = PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            ViewData["Title"] = $"#{Tag} Tips & Tricks";
            ViewData["Description"] = $"Discover tips and tricks tagged with #{Tag} for Microsoft Copilot, GitHub Copilot, Azure AI, and more AI productivity tools.";

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tips for tag: {Tag}", Tag);
            return RedirectToPage("/Error");
        }
    }
}
