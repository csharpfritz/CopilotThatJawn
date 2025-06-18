using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using Shared;
using Web.Extensions;
using Web.Services;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

// Add .NET Aspire service defaults
builder.AddServiceDefaults();

// Register HttpContextAccessor for accessing HttpContext in Razor Pages
builder.Services.AddHttpContextAccessor();

// Add route configuration to enforce lowercase URLs for better SEO
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = false;
});

builder.AddAzureTableClient("tables");

// Add Redis distributed caching - manual configuration since extension doesn't exist

builder.AddRedisDistributedCache("redis");
builder.AddRedisOutputCache("redis");

// Configure Redis key prefixes for better organization
builder.Services.PostConfigure<Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions>(options =>
{
    options.InstanceName = "CopilotThatJawn:";
});

// Add WebOptimizer services
builder.Services.AddWebOptimizer(pipeline =>
{
    if (!builder.Environment.IsDevelopment())
    {
        // Bundle and minify CSS files in production only
        pipeline.AddCssBundle("/css/bundle.min.css", 
            "css/site.css",
            "css/layout.css");

        // Bundle and minify JavaScript files in production only
        pipeline.AddJavaScriptBundle("/js/bundle.min.js",
            "js/site.js",
            "js/analytics.js",
            "js/theme-switcher.js");
            
        // Enable minification for all CSS files
        pipeline.MinifyCssFiles();
        
        // Enable minification for all JavaScript files  
        pipeline.MinifyJsFiles();
    }
    else
    {
        // In development, still enable basic minification for testing
        pipeline.MinifyCssFiles("css/*.css");
        pipeline.MinifyJsFiles("js/*.js");
    }
});

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Pages";
    });

builder.Services.AddControllers(); // Add controller support for API endpoints
builder.Services.AddMvc().AddViewComponentsAsServices(); // Register view components

// Add caching services
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

// Configure output cache policies (Redis is already configured above via AddRedisOutputCache)
builder.Services.Configure<Microsoft.AspNetCore.OutputCaching.OutputCacheOptions>(options =>
{
    // Default site-wide caching policy - extended to 6 hours for better performance
    options.AddBasePolicy(builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByQuery("*")
               .SetVaryByHeader("Accept-Language")  // Vary by language
               .Expire(TimeSpan.FromHours(6))       // Cache for 6 hours by default
               .Tag("outputcache", "site")); // Add tags for better organization
                 
    // Special policy for static content pages - extended to 3 days
    options.AddPolicy("StaticContent", builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .Expire(TimeSpan.FromDays(3))        // Cache static content for 3 days
               .Tag("outputcache", "static")); // Add tags for better organization
               
    // Special policy for tips and content pages - extended to 3 days since they're static
    options.AddPolicy("TipsContent", builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByRouteValue("slug")         // Vary by tip slug
               .Expire(TimeSpan.FromDays(3))        // Cache tips for 3 days
               .Tag("outputcache", "tips", "content")); // Add tags for better organization
               
    // Policy for frequently updated content - extended to 6 hours minimum
    options.AddPolicy("DynamicContent", builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByQuery("*")
               .Expire(TimeSpan.FromHours(6))       // Cache dynamic content for 6 hours
               .Tag("outputcache", "dynamic")); // Add tags for better organization
});

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "image/svg+xml", "application/javascript", "text/css" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

// Register content service
builder.Services.AddScoped<IContentService, ContentService>();

var app = builder.Build();

var options = new RewriteOptions()
	.AddRedirectToNonWwwPermanent()
	.AddRedirect("^tips/tag$", "tips", 301); // Redirect /tips/tag to /tips with 301 (permanent) redirect
	//.AddRedirectToHttpsPermanent();
app.UseRewriter(options);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();

	// Enable compression and caching only in production
	app.UseResponseCompression();
	app.UseResponseCaching();
	app.UseOutputCache();
}
else
{
	app.UseDeveloperExceptionPage();
}

// Enable output cache in all environments to test caching functionality
//app.UseOutputCache();

// Enable compression and caching early in the pipeline
app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseWebOptimizer(); // Use WebOptimizer in production
    
    // Add middleware to handle cache headers for WebOptimizer files
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/css/bundle.min.css") || 
            context.Request.Path.StartsWithSegments("/js/bundle.min.js"))
        {
            // Set headers to ensure proper cache behavior for bundled files
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.CacheControl = "public,max-age=31536000,immutable";
                context.Response.Headers.Vary = "Accept-Encoding";
                return Task.CompletedTask;
            });
        }
        await next();
    });
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (!app.Environment.IsDevelopment())
        {
            var path = ctx.Context.Request.Path.Value?.ToLowerInvariant();
            
            // Different caching strategies based on file type and path
            if (path != null)
            {
                // WebOptimizer bundles and files with version query strings - cache aggressively
                if (path.Contains("bundle.min.") || ctx.Context.Request.Query.ContainsKey("v"))
                {
                    ctx.Context.Response.Headers.CacheControl = "public,max-age=31536000,immutable"; // 1 year
                }
                // Regular CSS/JS files - shorter cache with validation
                else if (path.EndsWith(".css") || path.EndsWith(".js"))
                {
                    ctx.Context.Response.Headers.CacheControl = "public,max-age=3600,must-revalidate"; // 1 hour
                }
                // Images and fonts - medium cache
                else if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg") || 
                        path.EndsWith(".gif") || path.EndsWith(".svg") || path.EndsWith(".webp") ||
                        path.EndsWith(".woff") || path.EndsWith(".woff2") || path.EndsWith(".ttf"))
                {
                    ctx.Context.Response.Headers.CacheControl = "public,max-age=2592000"; // 30 days
                }
                // Other static files - short cache
                else
                {
                    ctx.Context.Response.Headers.CacheControl = "public,max-age=3600"; // 1 hour
                }
            }
            
            ctx.Context.Response.Headers.Vary = "Accept-Encoding";
        }
        else
        {
            // Disable caching in development
            ctx.Context.Response.Headers.CacheControl = "no-cache, no-store";
            ctx.Context.Response.Headers.Pragma = "no-cache";
            ctx.Context.Response.Headers.Expires = "-1";
        }
    }
});

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Map sitemap and RSS feed endpoints with caching
app.MapSitemapEndpoint();
app.MapRssFeedEndpoint();

// Map cache refresh endpoint
app.MapCacheRefreshEndpoint();

app.MapDefaultEndpoints();

app.Run();
