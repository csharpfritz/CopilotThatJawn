using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (frontMatterData.TryGetValue("publishedDate", out object publishedDateObj) && publishedDateObj != null)
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

        public static async Task UploadToTableStorage(TipModel tip, string connectionString)
        {
            var serviceClient = new TableServiceClient(connectionString);
            var tableName = "Content";
            await serviceClient.CreateTableIfNotExistsAsync(tableName);
            var tableClient = serviceClient.GetTableClient(tableName);
            var entity = new ContentEntity
            {
                PartitionKey = tip.Category.ToLowerInvariant(),
                RowKey = !string.IsNullOrWhiteSpace(tip.UrlSlug) ? tip.UrlSlug : tip.FileName,
                Slug = tip.UrlSlug,
                Title = tip.Title,
                Category = tip.Category,
                Tags = string.Join(",", tip.Tags),
                Difficulty = tip.Difficulty,
                Author = tip.Author,
                PublishedDate = DateTime.SpecifyKind(tip.PublishedDate, DateTimeKind.Utc),
                Description = tip.Description,
                Content = tip.Content,
                FileName = tip.FileName
            };
            await tableClient.UpsertEntityAsync(entity);
        }
    }
}
