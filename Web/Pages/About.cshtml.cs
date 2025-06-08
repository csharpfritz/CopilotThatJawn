using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class AboutModel : BasePageModel
{
    // Override cache duration for static pages - cache for 1 hour
    protected override int CacheDurationSeconds => 3600;
    
    public void OnGet()
    {
    }
}
