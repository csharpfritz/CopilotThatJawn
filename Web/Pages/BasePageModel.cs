using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OutputCaching;

namespace Web.Pages;

[OutputCache]
public abstract class BasePageModel : PageModel
{    /// <summary>
    /// The default output cache duration for dynamic pages - extended to 6 hours
    /// </summary>
    protected virtual int CacheDurationSeconds => 21600; // 6 hours by default

    /// <summary>
    /// Whether the page allows caching by default
    /// </summary>
    protected virtual bool AllowCaching => true;

    /// <summary>
    /// Configure output caching for the page
    /// </summary>
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        base.OnPageHandlerExecuting(context);

        if (AllowCaching && HttpContext.Request.Method == "GET")
        {
            // Set cache headers
            var headers = HttpContext.Response.GetTypedHeaders();
            headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(CacheDurationSeconds)
            };
        }
    }

    /// <summary>
    /// Clear the output cache for this page
    /// </summary>
    protected async Task InvalidateCache()
    {
        var outputCacheStore = HttpContext.RequestServices.GetRequiredService<IOutputCacheStore>();
        await outputCacheStore.EvictByTagAsync(GetType().FullName!, default);
    }
}
