using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class ContributeModel : BasePageModel
{
    // Override cache duration for static content - cache for 3 days
    protected override int CacheDurationSeconds => 259200; // 3 days
    
    public void OnGet()
    {
    }
}