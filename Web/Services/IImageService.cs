using Shared;

namespace Web.Services;

/// <summary>
/// Interface for managing content images
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Upload an image file to blob storage
    /// </summary>
    Task<ImageInfo> UploadImageAsync(Stream imageStream, string fileName, string? altText = null, string? caption = null);

    /// <summary>
    /// Get an image by its ID
    /// </summary>
    Task<ImageInfo?> GetImageAsync(string imageId);

    /// <summary>
    /// Delete an image from storage
    /// </summary>
    Task DeleteImageAsync(string imageId);

    /// <summary>
    /// Update image metadata (alt text, caption)
    /// </summary>
    Task UpdateImageMetadataAsync(string imageId, string? altText = null, string? caption = null);

    /// <summary>
    /// Get the public URL for an image
    /// </summary>
    string GetImageUrl(string imageId, ImageSize size = ImageSize.Original);

    /// <summary>
    /// Get the image ID for a given filename
    /// </summary>
    /// <param name="filename">The original filename of the image</param>
    /// <returns>The image ID if found, null otherwise</returns>
    string? GetImageIdByFilename(string filename);
}

public enum ImageSize
{
    Original,
    Thumbnail,
    Medium,
    Large
}
