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

    public async Task<Shared.ImageInfo?> GetImageAsync(string imageId)
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

                    return new Shared.ImageInfo
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
