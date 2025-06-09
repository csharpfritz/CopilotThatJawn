using System.Text.Json.Serialization;

namespace Shared;

/// <summary>
/// Represents metadata for an image in content
/// </summary>
public class ImageInfo
{
    /// <summary>
    /// Original filename of the image
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Generated unique identifier for the image
    /// </summary>
    public string ImageId { get; set; } = string.Empty;

    /// <summary>
    /// The blob storage path for the image
    /// </summary>
    public string BlobPath { get; set; } = string.Empty;

    /// <summary>
    /// The public URL for the image (CDN or direct blob URL)
    /// </summary>
    public string PublicUrl { get; set; } = string.Empty;

    /// <summary>
    /// Alt text for accessibility
    /// </summary>
    public string AltText { get; set; } = string.Empty;

    /// <summary>
    /// Caption text (optional)
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Original width of the image in pixels
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Original height of the image in pixels
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Size of the image in bytes
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// MIME type of the image
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// When the image was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Hash of the image content for change detection
    /// </summary>
    public string ContentHash { get; set; } = string.Empty;

    /// <summary>
    /// The original file path of the image
    /// </summary>
    public string OriginalPath { get; set; } = string.Empty;
}
