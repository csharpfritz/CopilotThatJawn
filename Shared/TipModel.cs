using System.ComponentModel.DataAnnotations;

namespace Shared;

/// <summary>
/// Represents a tip/article with metadata and content
/// </summary>
public class TipModel
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Difficulty { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public DateTime LastModified { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the reading time estimate in minutes
    /// </summary>
    public int ReadingTimeMinutes => Math.Max(1, Content.Split(' ').Length / 200);

    public string UrlSlug { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for displaying tips in lists
/// </summary>
public class TipListViewModel
{
    public List<TipModel> Tips { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string? SelectedCategory { get; set; }
    public string? SelectedTag { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// ViewModel for displaying a single tip
/// </summary>
public class TipDetailViewModel
{
    public TipModel Tip { get; set; } = new();
    public List<TipModel> RelatedTips { get; set; } = new();
    public TipModel? PreviousTip { get; set; }
    public TipModel? NextTip { get; set; }
}

/// <summary>
/// Request model for searching/filtering tips
/// </summary>
public class TipSearchRequest
{
    public string? Category { get; set; }
    public string? Tag { get; set; }
    public string? Search { get; set; }
    public string? Difficulty { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// DTO for API responses
/// </summary>
public class TipDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Difficulty { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ReadingTimeMinutes { get; set; }
}
