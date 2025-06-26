using System.Net;
using System.Net.Mime;
using System.Xml.Linq;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Web.Services;

namespace Web.Extensions;

public static class EndpointExtensions
{
	public static void MapSitemapEndpoint(this WebApplication app)
	{
		app.MapGet("/sitemap.xml", async (HttpContext context, IContentService contentService) =>
		{            // Enable output caching for sitemap - extended to 6 hours
			context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
			{
				Public = true,
				MaxAge = TimeSpan.FromHours(6)
			};
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
		})
		.CacheOutput(policy => policy
				.Expire(TimeSpan.FromHours(6))
				.SetVaryByHost(true)
				.Tag("sitemap"));
	}

	public static void MapRssFeedEndpoint(this WebApplication app)
	{
		app.MapGet("/feed.rss", async (HttpContext context, IContentService contentService) =>
		{            // Enable output caching for RSS feed - extended to 2 hours for better performance
			context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
			{
				Public = true,
				MaxAge = TimeSpan.FromHours(2)
			};// Create the RSS feed XML document
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
		})
		.CacheOutput(policy => policy
				.Expire(TimeSpan.FromHours(2))
				.SetVaryByHost(true)
				.Tag("rss"));
	}

	public static void MapCacheRefreshEndpoint(this WebApplication app)
	{
		app.MapPost("/api/cache/refresh", async (HttpContext context, IContentService contentService, IOutputCacheStore outputCacheStore, ILogger<Program> logger, IWebHostEnvironment environment, IConfiguration configuration) =>
		{
			try
			{
				logger.LogInformation("Cache refresh requested via API endpoint");

				// Validate API key for security
				var expectedApiKey = configuration["CacheRefresh:ApiKey"];
				if (!string.IsNullOrEmpty(expectedApiKey))
				{
					var providedApiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
					if (string.IsNullOrEmpty(providedApiKey) || providedApiKey != expectedApiKey)
					{
						logger.LogWarning("Cache refresh request rejected: Invalid or missing API key");
						return Results.Unauthorized();
					}
				}
				else if (!environment.IsDevelopment())
				{
					logger.LogWarning("Cache refresh API key not configured in production environment");
					return Results.Problem("API key not configured", statusCode: 500);
				}

				// Only perform cache refresh in non-development environments
				if (environment.IsDevelopment())
				{
					logger.LogInformation("Development environment detected. Skipping cache refresh.");
					return Results.Ok(new
					{
						message = "Cache refresh skipped in development environment",
						timestamp = DateTime.UtcNow,
						environment = "Development"
					});
				}

				// Refresh content cache
				await contentService.RefreshContentAsync();

				// Evict all output cache entries
				await outputCacheStore.EvictByTagAsync("sitemap", default);
				await outputCacheStore.EvictByTagAsync("rss", default);
				await outputCacheStore.EvictByTagAsync("article-images", default);
				await outputCacheStore.EvictByTagAsync("Web.Pages.IndexModel", default);
				await outputCacheStore.EvictByTagAsync("Web.Pages.Tips.IndexModel", default);
				await outputCacheStore.EvictByTagAsync("Web.Pages.Tips.CategoryModel", default);
				await outputCacheStore.EvictByTagAsync("Web.Pages.Tips.TagModel", default);
				await outputCacheStore.EvictByTagAsync("Web.Pages.Tips.DetailsModel", default);

				// Also evict by the base policy
				await outputCacheStore.EvictByTagAsync("", default);

				logger.LogInformation("Cache refresh completed successfully (content and output cache)");
				return Results.Ok(new
				{
					message = "Content and output cache refreshed successfully",
					timestamp = DateTime.UtcNow
				});
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error refreshing cache via API endpoint");
				return Results.Problem("Failed to refresh cache", statusCode: 500);
			}
		})
		.WithName("RefreshCache")
		.WithSummary("Refresh the content cache")
		.WithDescription("Triggers a refresh of the in-memory content cache and output cache for all pages. Requires X-API-Key header for authentication.");
	}

	public static void MapArticleImagesEndpoint(this WebApplication app)
	{
		app.MapGet("/article-images/{*imagePath}", async (HttpContext context, BlobServiceClient blobServiceClient, ILogger<Program> logger, string imagePath) =>
		{
			if (string.IsNullOrEmpty(imagePath))
			{
				return Results.NotFound();
			}

			// Sanitize the path to prevent any potential issues
			imagePath = imagePath.Replace("..", "").TrimStart('/');

			try
			{
				// Get the container client (using "images" as the default container name)
				var containerClient = blobServiceClient.GetBlobContainerClient("content-images");

				// Check if the container exists
				if (!await containerClient.ExistsAsync())
				{
					logger.LogError("Images container not found in blob storage");
					return Results.Problem("Image container not available", statusCode: 500);
				}

				// Get the blob client for the requested image
				var blobClient = containerClient.GetBlobClient(imagePath);

				// Check if the blob exists
				if (!await blobClient.ExistsAsync())
				{
					logger.LogWarning("Article image not found in blob storage: {ImagePath}", imagePath);
					return Results.NotFound();
				}

				// Get the blob properties to determine content type
				var properties = await blobClient.GetPropertiesAsync();
				var contentType = properties.Value.ContentType;

				// If content type is not set in blob properties, determine it from file extension
				if (string.IsNullOrEmpty(contentType))
				{
					contentType = GetContentType(Path.GetExtension(imagePath));
				}

				// Set cache control headers for better performance
				context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
				{
					Public = true,
					MaxAge = TimeSpan.FromDays(30)
				};

				// Download the blob content
				var memoryStream = new MemoryStream();
				await blobClient.DownloadToAsync(memoryStream);
				memoryStream.Position = 0;

				// Return the file content with the appropriate content type
				return Results.File(memoryStream.ToArray(), contentType);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error retrieving article image from blob storage: {ImagePath}", imagePath);
				return Results.Problem("Error retrieving image", statusCode: 500);
			}
		})
		.CacheOutput(policy => policy
			.Expire(TimeSpan.FromDays(30))
			.SetVaryByHost(true)
			.SetVaryByQuery("*")
			.Tag("article-images"));
	}

	private static string GetContentType(string extension)
	{
		return extension.ToLower() switch
		{
			".jpg" or ".jpeg" => "image/jpeg",
			".png" => "image/png",
			".gif" => "image/gif",
			".webp" => "image/webp",
			".svg" => "image/svg+xml",
			".avif" => "image/avif",
			_ => "application/octet-stream"
		};
	}
}
