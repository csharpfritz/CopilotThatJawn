﻿using System.Text;
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
    {
        // Recursively get all markdown files in the directory
        var mdFiles = Directory.GetFiles(filePath, "*.md", SearchOption.AllDirectories);
        if (mdFiles.Length == 0)
        {
            Console.WriteLine($"No markdown files found in directory: {filePath}");
            return 1;
        }        int addedCount = 0;
        int updatedCount = 0;
        int unchangedCount = 0;
        int failedCount = 0;

        foreach (var mdFile in mdFiles)
        {
            try
            {
                TipModel tip = Shared.ContentUploadHelper.ParseMarkdownFile(mdFile);
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
    }
    else
    {
        // Parse the markdown file
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return 1;
        }
        TipModel tip = Shared.ContentUploadHelper.ParseMarkdownFile(filePath);
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
