
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
string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Error: AZURE_STORAGE_CONNECTION_STRING environment variable is not set.");
    return 1;
}

try
{
    // Parse the markdown file
    TipModel tip = ParseMarkdownFile(filePath);
    
    // Upload to Azure Table Storage
    await UploadToTableStorage(tip, connectionString);
    
    Console.WriteLine($"Successfully uploaded {Path.GetFileName(filePath)} to Azure Table Storage.");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    return 1;
}

// Method to parse markdown file with front matter
TipModel ParseMarkdownFile(string filePath)
{
    Console.WriteLine($"Parsing file: {filePath}");
    
    string content = File.ReadAllText(filePath);
    
    // Extract front matter (YAML between --- delimiters)
    var frontMatterMatch = Regex.Match(content, @"^---\s*\n(.*?)\n---\s*\n", 
        RegexOptions.Singleline);
    
    if (!frontMatterMatch.Success)
    {
        throw new FormatException("Front matter not found in the markdown file.");
    }
    
    string frontMatter = frontMatterMatch.Groups[1].Value;
    string markdownContent = content.Substring(frontMatterMatch.Length).Trim();
    
    // Parse YAML front matter
    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
        
    var frontMatterData = deserializer.Deserialize<Dictionary<string, object>>(frontMatter);
    
    // Create TipModel and fill it with front matter data
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
    
    // Parse dates
    if (frontMatterData.TryGetValue("publishedDate", out object publishedDateObj) && 
        publishedDateObj != null)
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
    
    if (frontMatterData.TryGetValue("lastModified", out object lastModifiedObj) && 
        lastModifiedObj != null)
    {
        if (DateTime.TryParse(lastModifiedObj.ToString(), out DateTime lastModified))
        {
            tip.LastModified = lastModified;
        }
    }
    else
    {
        tip.LastModified = DateTime.UtcNow;
    }
    
    return tip;
}

// Helper method to get string value from front matter
string GetStringValue(Dictionary<string, object> data, string key)
{
    if (data.TryGetValue(key, out object value) && value != null)
    {
        return value.ToString() ?? string.Empty;
    }
    return string.Empty;
}

// Helper method to get list of strings from front matter
List<string> GetListValue(Dictionary<string, object> data, string key)
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
            // Try to parse as a comma-separated string
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

// Method to upload tip to Azure Table Storage
async Task UploadToTableStorage(TipModel tip, string connectionString)
{
    // Create table client
    var serviceClient = new TableServiceClient(connectionString);
    
    // Get a reference to the table (create if it doesn't exist)
    var tableName = "Content";
    
    // Create the table if it doesn't exist
    await serviceClient.CreateTableIfNotExistsAsync(tableName);
    
    // Get a reference to the table
    var tableClient = serviceClient.GetTableClient(tableName);
    
    // Create a table entity from the tip
    var entity = new ContentEntity
    {
        PartitionKey = tip.Category.ToLowerInvariant(),
        RowKey = tip.UrlSlug,
        Title = tip.Title,
        Category = tip.Category,
        Tags = string.Join(",", tip.Tags),
        Difficulty = tip.Difficulty,
        Author = tip.Author,
        PublishedDate = tip.PublishedDate,
        LastModified = tip.LastModified,
        Description = tip.Description,
        Content = tip.Content,
        FileName = tip.FileName
    };
    
    // Upload to table storage
    await tableClient.UpsertEntityAsync(entity);
}

// Table storage entity class
public class ContentEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Tips";
	public string RowKey {get { return Category; } set { } }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public DateTime LastModified { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
