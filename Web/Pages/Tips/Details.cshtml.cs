using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;
using Shared;
using Web.Services;

namespace Web.Pages.Tips;

[OutputCache(PolicyName = "TipsContent")]
public class DetailsModel : BasePageModel
{
    private readonly IContentService _contentService;
    private readonly ILogger<DetailsModel> _logger;      // Override cache duration for individual tips - cache for 3 days since tips are static content
    protected override int CacheDurationSeconds => 259200; // 3 days

    public DetailsModel(IContentService contentService, ILogger<DetailsModel> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    public TipDetailViewModel ViewModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        try
        {
            var tip = await _contentService.GetTipBySlugAsync(slug);
            
            if (tip == null)
            {
                _logger.LogWarning("Tip not found: {Slug}", slug);
                return NotFound();
            }

            var relatedTips = await _contentService.GetRelatedTipsAsync(tip, 3);

            ViewModel = new TipDetailViewModel
            {
                Tip = tip,
                RelatedTips = relatedTips
            };

            ViewData["Title"] = tip.Title;
            ViewData["Description"] = tip.Description;
            ViewData["Keywords"] = string.Join(", ", tip.Tags);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tip: {Slug}", slug);
            return RedirectToPage("/Error");
        }
    }
}
