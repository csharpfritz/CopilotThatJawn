using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Web.Services;

/// <summary>
/// Service for managing content images with Azure Blob Storage
/// </summary>
public class ImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<ImageService> _logger;
    private readonly string _containerName = "content-images";
    private readonly string _cdnEndpoint;

    public ImageService(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration,
        ILogger<ImageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _cdnEndpoint = configuration["Azure:CdnEndpoint"] ?? string.Empty;
    }

    public async Task<ImageInfo> UploadImageAsync(Stream imageStream, string fileName, string? altText = null, string? caption = null)
    {
        try
        {
            // Create container if it doesn't exist
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Generate a unique ID and path for the image
            var imageId = Guid.NewGuid().ToString("N");
            var blobPath = GenerateBlobPath(fileName, imageId);

            // Calculate content hash
            var contentHash = await CalculateContentHashAsync(imageStream);
            imageStream.Position = 0;

            // Get image dimensions and type
            using var image = await Image.LoadAsync(imageStream);
            imageStream.Position = 0;

            var imageInfo = new ImageInfo
            {
                FileName = fileName,
                ImageId = imageId,
                BlobPath = blobPath,
                PublicUrl = GetImageUrl(imageId),
                AltText = altText ?? fileName,
                Caption = caption,
                Width = image.Width,
                Height = image.Height,
                ContentType = GetContentType(fileName),
                UploadedAt = DateTime.UtcNow,
                ContentHash = contentHash
            };

            // Upload original
            var blobClient = container.GetBlobClient(blobPath);
            await blobClient.UploadAsync(imageStream, new BlobUploadOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    ["ImageId"] = imageId,
                    ["FileName"] = fileName,
                    ["AltText"] = altText ?? fileName,
                    ["Caption"] = caption ?? string.Empty,
                    ["ContentHash"] = contentHash
                }
            });

            // Generate and upload thumbnails
            await GenerateThumbnailsAsync(container, imageStream, blobPath);

            return imageInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image {FileName}", fileName);
            throw;
        }
    }

    public async Task<ImageInfo?> GetImageAsync(string imageId)
    {
        try
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobs = container.GetBlobsAsync(prefix: $"{imageId}/");

            await foreach (var blob in blobs)
            {
                if (blob.Name.EndsWith("/original"))
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    var properties = await blobClient.GetPropertiesAsync();

                    return new ImageInfo
                    {
                        ImageId = imageId,
                        FileName = properties.Value.Metadata["FileName"],
                        BlobPath = blob.Name,
                        PublicUrl = GetImageUrl(imageId),
                        AltText = properties.Value.Metadata["AltText"],
                        Caption = properties.Value.Metadata["Caption"],
                        ContentType = properties.Value.ContentType,
                        UploadedAt = properties.Value.CreatedOn.UtcDateTime,
                        ContentHash = properties.Value.Metadata["ContentHash"]
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting image {ImageId}", imageId);
            return null;
        }
    }

    public async Task DeleteImageAsync(string imageId)
    {
        try
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobs = container.GetBlobsAsync(prefix: $"{imageId}/");

            await foreach (var blob in blobs)
            {
                await container.DeleteBlobAsync(blob.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ImageId}", imageId);
            throw;
        }
    }

    public async Task UpdateImageMetadataAsync(string imageId, string? altText = null, string? caption = null)
    {
        try
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobs = container.GetBlobsAsync(prefix: $"{imageId}/");

            await foreach (var blob in blobs)
            {
                var blobClient = container.GetBlobClient(blob.Name);
                var properties = await blobClient.GetPropertiesAsync();
                var metadata = new Dictionary<string, string>(properties.Value.Metadata);

                if (altText != null) metadata["AltText"] = altText;
                if (caption != null) metadata["Caption"] = caption;

                await blobClient.SetMetadataAsync(metadata);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating image metadata {ImageId}", imageId);
            throw;
        }
    }

    public string GetImageUrl(string imageId, ImageSize size = ImageSize.Original)
    {
        var sizeFolder = size switch
        {
            ImageSize.Thumbnail => "thumbnail",
            ImageSize.Medium => "medium",
            ImageSize.Large => "large",
            _ => "original"
        };

        if (!string.IsNullOrEmpty(_cdnEndpoint))
        {
            return $"{_cdnEndpoint}/{_containerName}/{imageId}/{sizeFolder}";
        }

        return $"{_blobServiceClient.Uri}{_containerName}/{imageId}/{sizeFolder}";
    }

    private async Task GenerateThumbnailsAsync(BlobContainerClient container, Stream originalStream, string originalPath)
    {
        originalStream.Position = 0;
        using var image = await Image.LoadAsync(originalStream);

        // Define thumbnail sizes
        var sizes = new Dictionary<string, (int width, int height)>
        {
            ["thumbnail"] = (150, 150),
            ["medium"] = (400, 400),
            ["large"] = (800, 800)
        };

        foreach (var size in sizes)
        {
            var resized = image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(size.Value.width, size.Value.height)
            }));

            var thumbnailPath = originalPath.Replace("/original", $"/{size.Key}");
            var thumbnailBlob = container.GetBlobClient(thumbnailPath);

            using var ms = new MemoryStream();
            await resized.SaveAsync(ms, image.Metadata.DecodedImageFormat);
            ms.Position = 0;

            await thumbnailBlob.UploadAsync(ms, overwrite: true);
        }
    }

    private string GenerateBlobPath(string fileName, string imageId)
    {
        var extension = Path.GetExtension(fileName);
        return $"{imageId}/original{extension}";
    }

    private async Task<string> CalculateContentHashAsync(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToBase64String(hash);
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    public string? GetImageIdByFilename(string filename)
    {
        try
        {
            // Get the container client
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            
            // List blobs and find one matching the filename
            var blobs = container.GetBlobs(prefix: "");
            foreach (var blob in blobs)
            {
                // Extract the image ID from the blob name (format: {imageId}/original/filename)
                var parts = blob.Name.Split('/');
                if (parts.Length >= 2)
                {
                    var imageId = parts[0];
                    var blobFilename = Path.GetFileName(blob.Name);
                    if (string.Equals(blobFilename, filename, StringComparison.OrdinalIgnoreCase))
                    {
                        return imageId;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up image ID for filename: {Filename}", filename);
        }
        
        return null;
    }
}
