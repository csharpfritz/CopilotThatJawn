using System.Web;
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
				htmlRenderer.ObjectRenderers.Add(new CustomImageRenderer());
			}
		}
	}

	private class CustomImageRenderer : HtmlObjectRenderer<LinkInline>
	{

		protected override void Write(HtmlRenderer renderer, LinkInline link)
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

				renderer.Write("<img src=\"");
				renderer.WriteEscapeUrl(url);
				renderer.Write("\"");

				if (link.Title != null)
				{
					renderer.Write(" title=\"");
					renderer.Write(HttpUtility.UrlDecode(link.Title));
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

	}
}
