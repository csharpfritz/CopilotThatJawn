using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Shared;
using Web.Extensions;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureTableClient("tables");

// Add WebOptimizer services
builder.Services.AddWebOptimizer(pipeline =>
{
    // Bundle and minify CSS files
    pipeline.MinifyCssFiles();
    pipeline.AddCssBundle("/css/bundle.min.css", 
        "css/site.css",
        "css/layout.css");

    // Bundle and minify JavaScript files
    pipeline.MinifyJsFiles();
    pipeline.AddJavaScriptBundle("/js/bundle.min.js",
        "js/site.js",
        "js/analytics.js",
        "js/theme-switcher.js");
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
    options.AddBasePolicy(builder => 
        builder.Cache()
               .SetVaryByHost(true)
               .SetVaryByQuery("*"));
    
    // Cache static assets for longer
    options.AddPolicy("StaticFiles", builder => 
        builder.Cache()
               .Expire(TimeSpan.FromDays(30))
               .SetVaryByHost(true));
               
    // Add policies for sitemap and RSS feed
    options.AddPolicy("SitemapPolicy", builder =>
        builder.Cache()
               .Expire(TimeSpan.FromHours(12)));
               
    options.AddPolicy("RssFeedPolicy", builder =>
        builder.Cache()
               .Expire(TimeSpan.FromHours(1)));
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Enable compression and caching early in the pipeline
app.UseResponseCompression();
app.UseResponseCaching();
app.UseOutputCache();

app.UseHttpsRedirection();
app.UseWebOptimizer(); // Add WebOptimizer middleware before static files
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 30 days
        ctx.Context.Response.Headers.CacheControl = "public,max-age=2592000";
        ctx.Context.Response.Headers.Vary = "Accept-Encoding";
    }
});

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Map sitemap and RSS feed endpoints with caching
app.MapSitemapEndpoint();
app.MapRssFeedEndpoint();

app.Run();
