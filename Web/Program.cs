using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using Shared;
using Web.Extensions;
using Web.Services;
using Microsoft.AspNetCore.Rewrite;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Add .NET Aspire service defaults
builder.AddServiceDefaults();

builder.AddAzureTableClient("tables");

// Add WebOptimizer services
builder.Services.AddWebOptimizer(pipeline =>
{
    if (!builder.Environment.IsDevelopment())
    {
        // Bundle and minify CSS files in production only
        pipeline.MinifyCssFiles();
        pipeline.AddCssBundle("/css/bundle.min.css", 
            "css/site.css",
            "css/layout.css");

        // Bundle and minify JavaScript files in production only
        pipeline.MinifyJsFiles();
        pipeline.AddJavaScriptBundle("/js/bundle.min.js",
            "js/site.js",
            "js/analytics.js",
            "js/theme-switcher.js");
    }
    // In development, no bundling or minification - serve files directly
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
builder.Services.AddOutputCache(options =>
{
    // Default site-wide caching policy
    options.AddBasePolicy(builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByQuery("*")
               .SetVaryByHeader("Accept-Language")  // Vary by language
               .Expire(TimeSpan.FromMinutes(10))); // Cache for 10 minutes by default
               
    // Special policy for static content pages
    options.AddPolicy("StaticContent", builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .Expire(TimeSpan.FromHours(1))); // Cache static content for 1 hour
               
    // Policy for frequently updated content
    options.AddPolicy("DynamicContent", builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByQuery("*")
               .Expire(TimeSpan.FromMinutes(5))); // Cache dynamic content for 5 minutes
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

// Add Azure Blob Storage
builder.Services.AddSingleton(x => 
{
    var connectionString = builder.Configuration.GetConnectionString("AzureStorage");
    return new BlobServiceClient(connectionString);
});

// Add Image Service
builder.Services.AddSingleton<IImageService, ImageService>();

// Add Content Service with image handling
builder.Services.AddSingleton<IContentService, ContentService>();

var app = builder.Build();

var options = new RewriteOptions()
	.AddRedirectToNonWwwPermanent();
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

// Enable compression and caching early in the pipeline
app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseWebOptimizer(); // Only use WebOptimizer in production
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (!app.Environment.IsDevelopment())
        {
            // Cache static files for 30 days in production
            ctx.Context.Response.Headers.CacheControl = "public,max-age=2592000";
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
