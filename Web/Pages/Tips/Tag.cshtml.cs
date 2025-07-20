using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;
using Shared;
using Web.Services;

namespace Web.Pages.Tips;

[OutputCache(Duration = 21600, Tags = new[] { "tips", "content", "tag" })]
public class TagModel : BasePageModel
{
    private readonly IContentService _contentService;
    private readonly ILogger<TagModel> _logger;
      // Use default cache duration for tag pages (6 hours) - tags change infrequently
    // protected override int CacheDurationSeconds => base.CacheDurationSeconds; // 6 hours default

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

        // Always redirect to lowercase version of the tag for SEO consistency
        var lowercaseTag = Tag.ToLowerInvariant();
        if (lowercaseTag != Tag)
        {
            return RedirectToPage("/Tips/Tag", new { tag = lowercaseTag, page = PageNumber });
        }

        try
        {
            var request = new TipSearchRequest
            {
                Tag = Tag,
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
                SelectedTag = Tag,
                Page = PageNumber,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount
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
