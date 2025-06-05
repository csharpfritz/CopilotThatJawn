using System.Net;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Web.Models;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Add controller support for API endpoints
builder.Services.AddMvc().AddViewComponentsAsServices(); // Register view components

// Register content service
builder.Services.AddScoped<IContentService, ContentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers(); // Map controller routes for API endpoints

// Sitemap.xml endpoint - dynamically generates a sitemap based on content
app.MapGet("/sitemap.xml", async (HttpContext context, IContentService contentService) =>
{
    // Define the XML namespace for the sitemap
    XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
    
    // Create the XML document with the urlset as the root element
    var sitemap = new XDocument(
        new XDeclaration("1.0", "utf-8", null),
        new XElement(ns + "urlset")
    );
    
    // Get the base URL of the site
    var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
    
    // Add the home page URL
    sitemap.Root!.Add(
        new XElement(ns + "url",
            new XElement(ns + "loc", baseUrl),
            new XElement(ns + "changefreq", "weekly"),
            new XElement(ns + "priority", "1.0"),
            new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
        )
    );
    
    // Add URLs for static pages
    var staticPages = new[] { "/Privacy" };
    foreach (var page in staticPages)
    {
        sitemap.Root.Add(
            new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}{page}"),
                new XElement(ns + "changefreq", "monthly"),
                new XElement(ns + "priority", "0.8"),
                new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            )
        );
    }
    
    // Add main tips listing page
    sitemap.Root.Add(
        new XElement(ns + "url",
            new XElement(ns + "loc", $"{baseUrl}/Tips"),
            new XElement(ns + "changefreq", "daily"),
            new XElement(ns + "priority", "0.9"),
            new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
        )
    );
    
    // Add all tips pages
    var tips = await contentService.GetAllTipsAsync();
    foreach (var tip in tips)
    {
        sitemap.Root.Add(
            new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}/tips/{tip.UrlSlug}"),
                new XElement(ns + "lastmod", tip.LastModified.ToString("yyyy-MM-dd")),
                new XElement(ns + "changefreq", "monthly"),
                new XElement(ns + "priority", "0.7")
            )
        );
    }
    
    // Add category pages
    var categories = await contentService.GetCategoriesAsync();
    foreach (var category in categories)
    {
        sitemap.Root.Add(
            new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}/tips/category/{WebUtility.UrlEncode(category)}"),
                new XElement(ns + "changefreq", "weekly"),
                new XElement(ns + "priority", "0.6"),
                new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            )
        );
    }
    
    // Add tag pages
    var tags = await contentService.GetTagsAsync();
    foreach (var tag in tags)
    {
        sitemap.Root.Add(
            new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}/tips/tag/{WebUtility.UrlEncode(tag)}"),
                new XElement(ns + "changefreq", "weekly"),
                new XElement(ns + "priority", "0.6"),
                new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            )
        );
    }
    
    // Set the content type to XML
    context.Response.ContentType = MediaTypeNames.Application.Xml;
    
    // Return the XML document as a string
    return sitemap.ToString();
});

app.Run();
