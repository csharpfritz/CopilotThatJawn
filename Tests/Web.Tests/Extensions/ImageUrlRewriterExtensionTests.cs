using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Moq;
using Web.Extensions;
using Web.Services;
using Xunit;

namespace Web.Tests.Extensions;

public class ImageUrlRewriterExtensionTests
{
    [Fact]
    public void Setup_ReplacesDefaultImageRenderer()
    {
        // Arrange

        var extension = new ImageUrlRewriterExtension();
        var pipeline = new MarkdownPipelineBuilder().Build();
        var renderer = new HtmlRenderer(new StringWriter());

        // Act
        extension.Setup(pipeline, renderer);

        // Assert
        var imageRenderer = renderer.ObjectRenderers.FindExact<Markdig.Renderers.Html.Inlines.LinkInlineRenderer>();
        Assert.Null(imageRenderer); // Original renderer should be removed
        
        var customRenderer = renderer.ObjectRenderers.Find(r => r.GetType().Name == "CustomImageRenderer");
        Assert.NotNull(customRenderer); // Custom renderer should be added
    }

    [Fact]
    public void Render_AddsResponsiveImageClasses_WhenRenderingImage()
    {
        // Arrange
        var extension = new ImageUrlRewriterExtension();

        // Create a markdown pipeline with our extension
        var pipeline = new MarkdownPipelineBuilder()
            .Use(extension)
            .Build();
        
        // Create a markdown document with an image
        var markdown = "![Alt text](image.jpg \"Image title\")";
        
        // Act
        var result = Markdown.ToHtml(markdown, pipeline);
        
        // Assert
        Assert.Contains("class=\"img-fluid\"", result);
        Assert.Contains("loading=\"lazy\"", result);
        Assert.Contains("title=\"Image title\"", result);
    }
}
