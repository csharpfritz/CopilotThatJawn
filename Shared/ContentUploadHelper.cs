using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Data.Tables;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Shared
{
    public static class ContentUploadHelper
    {
        public static TipModel ParseMarkdownFile(string filePath)
        {
            Console.WriteLine($"Parsing file: {filePath}");
            string content = File.ReadAllText(filePath);
            var frontMatterMatch = Regex.Match(content, @"^---\s*\n(.*?)\n---\s*\n", RegexOptions.Singleline);
            if (!frontMatterMatch.Success)
            {
                throw new FormatException("Front matter not found in the markdown file.");
            }
            string frontMatter = frontMatterMatch.Groups[1].Value;
            string markdownContent = content.Substring(frontMatterMatch.Length).Trim();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var frontMatterData = deserializer.Deserialize<Dictionary<string, object>>(frontMatter);
            var tip = new TipModel
            {
                Title = GetStringValue(frontMatterData, "title"),
                Category = GetStringValue(frontMatterData, "category"),
                Tags = GetListValue(frontMatterData, "tags"),
                Difficulty = GetStringValue(frontMatterData, "difficulty"),
                Author = GetStringValue(frontMatterData, "author"),
                Description = GetStringValue(frontMatterData, "description"),
                Content = markdownContent,
                FileName = Path.GetFileName(filePath)
            };
            if (frontMatterData.TryGetValue("publishedDate", out object? publishedDateObj) && publishedDateObj != null)
            {
                if (DateTime.TryParse(publishedDateObj.ToString(), out DateTime publishedDate))
                {
                    tip.PublishedDate = publishedDate;
                }
            }
            else
            {
                tip.PublishedDate = DateTime.UtcNow;
            }
            return tip;
        }

        public static string GetStringValue(Dictionary<string, object> data, string key)
        {
            if (data.TryGetValue(key, out object value) && value != null)
            {
                return value.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        public static List<string> GetListValue(Dictionary<string, object> data, string key)
        {
            var list = new List<string>();
            if (data.TryGetValue(key, out object value) && value != null)
            {
                if (value is List<object> objList)
                {
                    foreach (var item in objList)
                    {
                        if (item != null)
                        {
                            list.Add(item.ToString() ?? string.Empty);
                        }
                    }
                }
                else if (value.ToString() != null)
                {
                    string stringValue = value.ToString()!;
                    if (stringValue.Contains(','))
                    {
                        list.AddRange(stringValue.Split(',').Select(s => s.Trim()));
                    }
                    else
                    {
                        list.Add(stringValue);
                    }
                }
            }
            return list;
        }

        private static string CalculateContentHash(TipModel tip)
        {
            // Create a string representation of the content that should be compared for changes
            var contentToHash = $"{tip.Title}|{tip.Category}|{string.Join(",", tip.Tags)}|{tip.Difficulty}|{tip.Author}|{tip.Description}|{tip.Content}";
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contentToHash));
            return Convert.ToBase64String(hashBytes);
        }

        private static async Task<ContentEntity?> GetExistingContent(TableClient tableClient, string partitionKey, string rowKey)
        {
            try
            {
                var response = await tableClient.GetEntityAsync<ContentEntity>(partitionKey, rowKey);
                return response.Value;
            }
            catch (Exception)
            {
                // Entity doesn't exist or other error - treat as not found
                return null;
            }
        }        public static async Task<UploadStatus> UploadToTableStorage(TipModel tip, string connectionString)
        {
            try
            {
                var serviceClient = new TableServiceClient(connectionString);
                var tableName = "Content";
                await serviceClient.CreateTableIfNotExistsAsync(tableName);
                var tableClient = serviceClient.GetTableClient(tableName);

                var partitionKey = tip.Category.ToLowerInvariant();
                var rowKey = !string.IsNullOrWhiteSpace(tip.UrlSlug) ? tip.UrlSlug : tip.FileName;
                
                // Calculate content hash for change detection
                var contentHash = CalculateContentHash(tip);
                
                // Check if entity already exists
                var existingEntity = await GetExistingContent(tableClient, partitionKey, rowKey);
                
                if (existingEntity != null && existingEntity.ContentHash == contentHash)
                {
                    Console.WriteLine($"Skipping {tip.FileName} - no changes detected");
                    return UploadStatus.Unchanged;
                }

                var entity = new ContentEntity
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    Slug = tip.UrlSlug,
                    Title = tip.Title,
                    Category = tip.Category,
                    Tags = string.Join(",", tip.Tags),
                    Difficulty = tip.Difficulty,
                    Author = tip.Author,
                    PublishedDate = DateTime.SpecifyKind(tip.PublishedDate, DateTimeKind.Utc),
                    Description = tip.Description,
                    Content = tip.Content,
                    FileName = tip.FileName,                ContentHash = contentHash,
                Images = tip.Images
                };
                
                await tableClient.UpsertEntityAsync(entity);
                
                if (existingEntity == null)
                {
                    Console.WriteLine($"Uploaded new content: {tip.FileName}");
                    return UploadStatus.Added;
                }
                else
                {
                    Console.WriteLine($"Updated changed content: {tip.FileName}");
                    return UploadStatus.Updated;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload {tip.FileName}: {ex.Message}");
                return UploadStatus.Failed;
            }
        }
    }
}
