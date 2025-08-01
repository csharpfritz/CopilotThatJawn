using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;
using Shared;
using Web.Services;

namespace Web.Pages.Tips;

[OutputCache(Duration = 21600, Tags = new[] { "tips", "content", "category" }, VaryByQueryKeys = new[] { "category", "page" })]
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
	public string Category { get; set; } = string.Empty;

	[BindProperty(SupportsGet = true)]
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

			// Always redirect to lowercase version of the category for SEO consistency
			if (matchingCategory.ToLowerInvariant() != Category.ToLowerInvariant())
			{
				return RedirectToPage("/Tips/Category", new { category = matchingCategory.ToLowerInvariant() });
			}

			Category = matchingCategory;

			var request = new TipSearchRequest
			{
				Category = Category,
				Page = Math.Max(1, PageNumber),
				PageSize = 12
			};

			var searchResult = await _contentService.SearchTipsAsync(request);

			ViewModel = new TipListViewModel
			{
				Tips = searchResult.Tips,
				Categories = categories,
				Tags = tags,
				SelectedCategory = Category,
				Page = PageNumber,
				PageSize = searchResult.PageSize,
				TotalCount = searchResult.TotalCount
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
