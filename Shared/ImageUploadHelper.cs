using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;

namespace Shared;

public static class ImageUploadHelper
{
    public static async Task<ImageInfo> ProcessAndUploadImageAsync(string imagePath, BlobServiceClient blobServiceClient)
    {
        var fileName = Path.GetFileName(imagePath);
        var imageId = GenerateImageId(imagePath);
        var containerName = "content-images";
        
        // Get or create container
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // Process image and get metadata
        using var image = await Image.LoadAsync(imagePath);        var imageInfo = new ImageInfo
        {
            FileName = fileName,
            ImageId = imageId,
            BlobPath = $"{imageId}/original{Path.GetExtension(fileName)}",
            OriginalPath = imagePath,
            AltText = Path.GetFileNameWithoutExtension(fileName), // Default alt text
            Width = image.Width,
            Height = image.Height,
            ContentType = GetContentType(fileName),
            UploadedAt = DateTime.UtcNow,
            ContentHash = await CalculateFileHashAsync(imagePath)
        };

        // Upload original
        await UploadImageAsync(containerClient, imagePath, imageInfo.BlobPath, imageInfo);

        // Generate and upload thumbnails
        var sizes = new Dictionary<string, (int width, int height)>
        {
            ["thumbnail"] = (150, 150),
            ["medium"] = (400, 400),
            ["large"] = (800, 800)
        };

        foreach (var size in sizes)
        {
            using var resized = image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(size.Value.width, size.Value.height)
            }));

            var thumbnailPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(fileName)}");
            try
            {
                await resized.SaveAsync(thumbnailPath);
                var blobPath = $"{imageId}/{size.Key}{Path.GetExtension(fileName)}";
                await UploadImageAsync(containerClient, thumbnailPath, blobPath, imageInfo);
            }
            finally
            {
                if (File.Exists(thumbnailPath))
                    File.Delete(thumbnailPath);
            }
        }

        return imageInfo;
    }

    private static async Task UploadImageAsync(BlobContainerClient containerClient, string filePath, string blobPath, ImageInfo imageInfo)
    {
        var blobClient = containerClient.GetBlobClient(blobPath);
        
        await using var fileStream = File.OpenRead(filePath);
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions
        {
            Metadata = new Dictionary<string, string>
            {
                ["ImageId"] = imageInfo.ImageId,
                ["FileName"] = imageInfo.FileName,
                ["AltText"] = imageInfo.AltText,
                ["ContentHash"] = imageInfo.ContentHash
            }
        });
    }

    private static string GenerateImageId(string filePath)
    {
        // Generate a deterministic ID based on the file path
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(filePath.ToLowerInvariant()));
        return Convert.ToHexString(hash).Substring(0, 16).ToLowerInvariant();
    }

    private static async Task<string> CalculateFileHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToBase64String(hash);
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
