using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class AboutModel : BasePageModel
{
    // Override cache duration for static pages - cache for 3 days
    protected override int CacheDurationSeconds => 259200; // 3 days
    
    public void OnGet()
    {
    }
}
