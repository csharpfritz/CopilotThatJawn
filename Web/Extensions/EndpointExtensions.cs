using System.Net;
using System.Net.Mime;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Web.Services;

namespace Web.Extensions;

public static class EndpointExtensions
{
    public static void MapSitemapEndpoint(this WebApplication app)
    {
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
            var staticPages = new[] { "/Privacy", "/Contribute" };
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
    }

    public static void MapRssFeedEndpoint(this WebApplication app)
    {
        app.MapGet("/feed.rss", async (HttpContext context, IContentService contentService) =>
        {            // Create the RSS feed XML document
            var rss = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"/xsl/rss.xsl\""),
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XElement("channel",
                        // Feed metadata
                        new XElement("title", "Copilot That Jawn - Latest Tips"),
                        new XElement("link", $"{context.Request.Scheme}://{context.Request.Host}"),
                        new XElement("description", "Where Philly innovation meets AI excellence. Master Microsoft Copilot and GitHub Copilot with our expert-curated tips and tricks."),
                        new XElement("language", "en-us"),
                        new XElement("copyright", $"Copyright {DateTime.Now.Year}, Copilot That Jawn"),
                        new XElement("lastBuildDate", DateTime.UtcNow.ToString("r")),
                        new XElement("generator", "Copilot That Jawn"),
                        // Add RSS feed icon
                        new XElement("image",
                            new XElement("url", $"{context.Request.Scheme}://{context.Request.Host}/img/icon-with-bg.webp"),
                            new XElement("title", "Copilot That Jawn"),
                            new XElement("link", $"{context.Request.Scheme}://{context.Request.Host}")
                        )
                    )
                )
            );

            // Get the latest 15 tips
            var tips = (await contentService.GetAllTipsAsync())
                .OrderByDescending(t => t.LastModified)
                .Take(15);

            // Get base URL
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

            // Add each tip as an item
            foreach (var tip in tips)
            {
                rss.Root!.Element("channel")!.Add(
                    new XElement("item",
                        new XElement("title", tip.Title),
                        new XElement("link", $"{baseUrl}/tips/{tip.UrlSlug}"),
                        new XElement("guid", $"{baseUrl}/tips/{tip.UrlSlug}"),
                        new XElement("description", tip.Description),
                        new XElement("pubDate", tip.LastModified.ToString("r")),
                        tip.Tags.Select(tag => new XElement("category", tag))
                    )
                );
            }            // Check if the request accepts HTML content
            var acceptHeader = context.Request.Headers.Accept.ToString().ToLower();
            if (acceptHeader.Contains("text/html") || acceptHeader.Contains("*/*"))
            {
                // For browser requests, use text/xml to allow XSL transformation
                context.Response.ContentType = "text/xml";
            }
            else
            {
                // For RSS readers, use the standard RSS content type
                context.Response.ContentType = "application/rss+xml";
            }
            
            // Return the XML document as a string
            return rss.ToString();
        });
    }
}
