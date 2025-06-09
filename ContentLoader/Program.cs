using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Markdig;
using Shared;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// Check for the command line argument
if (args.Length == 0)
{
    Console.WriteLine("Please provide a path to a markdown file as a command-line argument.");
    return 1;
}

string filePath = args[0];

// Check if the file exists
if (!File.Exists(filePath) && !Directory.Exists(filePath))
{
    Console.WriteLine($"File or directory not found: {filePath}");
    return 1;
}

// Get connection string from environment variable
string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Error: AZURE_STORAGE_CONNECTION_STRING environment variable is not set.");
    return 1;
}

try
{
    // Check if the path is a directory or a file
    if (Directory.Exists(filePath))
    {        // Recursively get all markdown files
        var mdFiles = Directory.GetFiles(filePath, "*.md", SearchOption.AllDirectories);
        
        // For each markdown file, look for images in its adjacent images folder
        var imageFiles = mdFiles
            .SelectMany(mdFile =>
            {
                var mdDirectory = Path.GetDirectoryName(mdFile);
                var imagesPath = Path.Combine(mdDirectory!, "images");
                return Directory.Exists(imagesPath)
                    ? Directory.GetFiles(imagesPath, "*.*", SearchOption.AllDirectories)
                        .Where(f => new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
                            .Contains(Path.GetExtension(f).ToLowerInvariant()))
                    : Array.Empty<string>();
            })
            .Distinct()
            .ToArray();

        if (mdFiles.Length == 0)
        {
            Console.WriteLine($"No markdown files found in directory: {filePath}");
            return 1;
        }        // Create blob service client for image uploads
        var blobServiceClient = new BlobServiceClient(connectionString);

        // Process images first
        Console.WriteLine("\nProcessing images...");
        var imageUploadResults = new List<ImageInfo>();
        foreach (var imageFile in imageFiles)
        {
            try
            {
                var imageInfo = await ImageUploadHelper.ProcessAndUploadImageAsync(imageFile, blobServiceClient);
                imageUploadResults.Add(imageInfo);
                Console.WriteLine($"Processed image: {imageFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image {imageFile}: {ex.Message}");
            }
        }

        // Now process markdown files
        Console.WriteLine("\nProcessing markdown files...");
        int addedCount = 0;
        int updatedCount = 0;
        int unchangedCount = 0;
        int failedCount = 0;

        foreach (var mdFile in mdFiles)
        {        try
            {
                // Parse markdown and look for image references
                TipModel tip = Shared.ContentUploadHelper.ParseMarkdownFile(mdFile);
                  // Replace image references in content with proper image IDs
                foreach (var image in imageUploadResults)
                {
                    // Get the markdown file's directory
                    var mdDirectory = Path.GetDirectoryName(mdFile)!;
                    // Get the image's directory
                    var imageDirectory = Path.GetDirectoryName(image.OriginalPath)!;
                    // Calculate relative path from markdown to image
                    var relativePath = Path.GetRelativePath(mdDirectory, imageDirectory);
                    var localImagePath = Path.Combine(relativePath, image.FileName).Replace("\\", "/");
                    
                    tip.Content = tip.Content.Replace(
                        $"]({localImagePath}",
                        $"](/images/{image.ImageId}/original"
                    );
                }

                // Add the image info to the content entity
                tip.Images = JsonSerializer.Serialize(imageUploadResults);
                
                var status = await Shared.ContentUploadHelper.UploadToTableStorage(tip, connectionString);
                
                switch (status)
                {
                    case UploadStatus.Added:
                        addedCount++;
                        break;
                    case UploadStatus.Updated:
                        updatedCount++;
                        break;
                    case UploadStatus.Unchanged:
                        unchangedCount++;
                        break;
                    case UploadStatus.Failed:
                        failedCount++;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading {mdFile}: {ex.Message}");
                failedCount++;
            }
        }

        Console.WriteLine("\nSync Summary:");
        Console.WriteLine($"Added:     {addedCount}");
        Console.WriteLine($"Updated:   {updatedCount}");
        Console.WriteLine($"Unchanged: {unchangedCount}");
        Console.WriteLine($"Failed:    {failedCount}");
        Console.WriteLine($"Total:     {mdFiles.Length}");

        return failedCount == 0 ? 0 : 1;
    }    else
    {
        // Parse the markdown file
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return 1;
        }

        // Create BlobServiceClient for single file
        var blobServiceClient = new BlobServiceClient(connectionString);
        var imageUploadResults = new List<ImageInfo>();

        // Check if there are any images in the images directory
        var imageDir = Path.Combine(Path.GetDirectoryName(filePath)!, "images");
        if (Directory.Exists(imageDir))
        {
            var imageFiles = Directory.GetFiles(imageDir, "*.*")
                .Where(f => new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
                    .Contains(Path.GetExtension(f).ToLowerInvariant()))
                .ToArray();

            foreach (var imageFile in imageFiles)
            {
                try
                {
                    var imageInfo = await ImageUploadHelper.ProcessAndUploadImageAsync(imageFile, blobServiceClient);
                    imageUploadResults.Add(imageInfo);
                    Console.WriteLine($"Processed image: {imageFile}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image {imageFile}: {ex.Message}");
                }
            }
        }

        // Parse and process markdown
        TipModel tip = Shared.ContentUploadHelper.ParseMarkdownFile(filePath);

        // Replace image references
        foreach (var image in imageUploadResults)
        {
            var localImagePath = $"images/{image.FileName}";
            tip.Content = tip.Content.Replace(
                $"]({localImagePath}",
                $"](/images/{image.ImageId}/original"
            );
        }

        // Add image info to content
        tip.Images = System.Text.Json.JsonSerializer.Serialize(imageUploadResults);

        // Upload to Azure Table Storage
        await Shared.ContentUploadHelper.UploadToTableStorage(tip, connectionString);
        return 0;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    return 1;
}

// Table storage entity class now moved to Shared/ContentEntity.cs
