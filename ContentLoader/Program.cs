using System.Text;
using System.Text.RegularExpressions;
using Azure;
using Azure.Data.Tables;
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
if (!File.Exists(filePath))
{
    Console.WriteLine($"File not found: {filePath}");
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
    // Parse the markdown file
    TipModel tip = Shared.ContentUploadHelper.ParseMarkdownFile(filePath);
    // Upload to Azure Table Storage
    await Shared.ContentUploadHelper.UploadToTableStorage(tip, connectionString);
    Console.WriteLine($"Successfully uploaded {Path.GetFileName(filePath)} to Azure Table Storage.");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    return 1;
}

// Table storage entity class now moved to Shared/ContentEntity.cs
