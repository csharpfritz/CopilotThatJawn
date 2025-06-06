using System;
using Azure;
using Azure.Data.Tables;

namespace Shared;

    public class ContentEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get { return Category; } set { } }
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
