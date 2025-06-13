using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Web.Services;

namespace Web.Extensions;

/// <summary>
/// Markdown extension for processing images in content
/// </summary>
public class ImageUrlRewriterExtension : IMarkdownExtension
{
    private readonly IImageService _imageService;

    public ImageUrlRewriterExtension(IImageService imageService)
    {
        _imageService = imageService;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Nothing to do on the pipeline
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            // Replace the default image renderer
            var originalRenderer = htmlRenderer.ObjectRenderers
                .FindExact<Markdig.Renderers.Html.Inlines.LinkInlineRenderer>();
            
            if (originalRenderer != null)
            {
                htmlRenderer.ObjectRenderers.Remove(originalRenderer);
                htmlRenderer.ObjectRenderers.Add(new CustomImageRenderer(_imageService));
            }
        }
    }

    private class CustomImageRenderer : HtmlObjectRenderer<LinkInline>
    {
        private readonly IImageService _imageService;

        public CustomImageRenderer(IImageService imageService)
        {
            _imageService = imageService;
        }        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            if (!link.IsImage)
            {
                renderer.Write('[');
                renderer.WriteChildren(link);
                renderer.Write("](");
                renderer.Write(link.Url);
                renderer.Write(')');
                return;
            }

            if (renderer.EnableHtmlForInline)
            {
                var url = link.Url;
                
                // Only process relative URLs
                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    // Try to get image ID - first check if it's already in our format
                    var imageId = ExtractImageId(url);
                    
                    if (string.IsNullOrEmpty(imageId))
                    {
                        // If not in our format, try to get the original filename
                        var filename = ExtractFilename(url);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            // Look up the image ID from the filename using IImageService
                            imageId = _imageService.GetImageIdByFilename(filename);
                        }
                    }

                    if (!string.IsNullOrEmpty(imageId))
                    {
                        url = _imageService.GetImageUrl(imageId);
                    }
                }

                renderer.Write("<img src=\"");
                renderer.WriteEscapeUrl(url);
                renderer.Write("\"");

                if (link.Title != null)
                {
                    renderer.Write(" title=\"");
                    renderer.WriteEscapeUrl(link.Title);
                    renderer.Write("\"");
                }

                if (!string.IsNullOrEmpty(link.Label))
                {
                    renderer.Write(" alt=\"");
                    renderer.WriteEscape(link.Label);
                    renderer.Write("\"");
                }

                // Add responsive image classes
                renderer.Write(" class=\"img-fluid\"");

                // Add loading="lazy" for better performance
                renderer.Write(" loading=\"lazy\"");

                renderer.Write(" />");
            }
        }

        private string? ExtractImageId(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            
            // Expected format: /images/{imageId}/size
            var parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && parts[0] == "images")
            {
                return parts[1];
            }
            return null;
        }

        private string? ExtractFilename(string? url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            // Handle relative path format: images/filename.png
            var parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && parts[0] == "images")
            {
                return parts[1];
            }
            
            // If it's just a filename
            if (parts.Length == 1)
            {
                return parts[0];
            }
            
            return null;
        }
    }
}
