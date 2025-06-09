using System;
using Azure;
using Azure.Data.Tables;

namespace Shared;

    public class ContentEntity : ITableEntity
    {
        // PartitionKey: Use Category to group related content
        public string PartitionKey { get; set; } = string.Empty;
        // RowKey: Use Slug (or FileName if Slug is not available) for uniqueness and searchability
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        // Removed LastModified; use Timestamp for last modification tracking
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        // Optional: Add Slug property for RowKey assignment if not already present
        public string Slug { get; set; } = string.Empty;
        // Content hash for change detection
        public string ContentHash { get; set; } = string.Empty;
        
        // Store as JSON string of ImageInfo objects
        public string Images { get; set; } = string.Empty;
    }
