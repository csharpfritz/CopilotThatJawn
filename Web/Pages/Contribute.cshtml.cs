using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class ContributeModel : BasePageModel
{
    // Override cache duration for static content - cache for 1 hour
    protected override int CacheDurationSeconds => 3600;
    
    public void OnGet()
    {
    }
}