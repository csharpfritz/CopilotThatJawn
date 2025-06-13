using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared;
using Web.Services;

namespace Web.Pages.Tips;

public class CategoryModel : BasePageModel
{
    private readonly IContentService _contentService;
    private readonly ILogger<CategoryModel> _logger;
      // Use default cache duration for category pages (6 hours) - categories change infrequently
    // protected override int CacheDurationSeconds => base.CacheDurationSeconds; // 6 hours default

    public CategoryModel(IContentService contentService, ILogger<CategoryModel> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    public TipListViewModel ViewModel { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string Category { get; set; } = string.Empty;    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Category))
        {
            return RedirectToPage("/Tips/Index");
        }

        try
        {
            // Get filter options first to validate category
            var categories = await _contentService.GetCategoriesAsync();
            var tags = await _contentService.GetTagsAsync();

            // Find the matching category with correct casing
            var matchingCategory = categories.FirstOrDefault(c => 
                c.Equals(Category, StringComparison.OrdinalIgnoreCase));

            // If category doesn't exist, redirect to index
            if (matchingCategory == null)
            {
                return RedirectToPage("/Tips/Index");
            }

            // If category exists but with different casing, redirect to correct casing
            if (matchingCategory != Category)
            {
                return RedirectToPage("/Tips/Category", new { category = matchingCategory });
            }

            var request = new TipSearchRequest
            {
                Category = Category,
                Page = Math.Max(1, PageNumber),
                PageSize = 12
            };

            var tips = await _contentService.SearchTipsAsync(request);
            var totalCount = tips.Count;

            ViewModel = new TipListViewModel
            {
                Tips = tips,
                Categories = categories,
                Tags = tags,
                SelectedCategory = Category,
                Page = PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            ViewData["Title"] = $"{Category} Tips & Tricks";
            ViewData["Description"] = $"Discover tips and tricks in the {Category} category for Microsoft Copilot, GitHub Copilot, Azure AI, and more AI productivity tools.";

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tips for category: {Category}", Category);
            return RedirectToPage("/Error");
        }
    }
}
