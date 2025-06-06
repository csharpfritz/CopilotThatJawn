using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;

public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;
    private readonly IConfiguration _configuration;

    public PrivacyModel(ILogger<PrivacyModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void OnGet()
    {
        _logger.LogInformation("Privacy policy page accessed at: {Time}", DateTime.UtcNow);

        // Add the Google Analytics ID to ViewData if we want to display it in the view
        ViewData["GoogleAnalyticsId"] = _configuration["GoogleAnalytics:MeasurementId"];
    }
}

