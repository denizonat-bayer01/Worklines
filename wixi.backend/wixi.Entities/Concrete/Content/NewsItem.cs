namespace wixi.Entities.Concrete.Content
{
    public class NewsItem
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Multi-language Title
        public string TitleDe { get; set; }
        public string TitleTr { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        
        // Multi-language Excerpt
        public string ExcerptDe { get; set; }
        public string ExcerptTr { get; set; }
        public string? ExcerptEn { get; set; }
        public string? ExcerptAr { get; set; }
        
        // Multi-language Content (optional - full article content)
        public string? ContentDe { get; set; }
        public string? ContentTr { get; set; }
        public string? ContentEn { get; set; }
        public string? ContentAr { get; set; }
        
        // Image
        public string ImageUrl { get; set; }
        
        // Category
        public string Category { get; set; }
        
        // Featured/Öne Çıkan
        public bool Featured { get; set; } = false;
        
        // Publishing
        public DateTime? PublishedAt { get; set; }
        
        // Slug for detail page (optional)
        public string? Slug { get; set; }
        
        // Ordering
        public int DisplayOrder { get; set; } = 0;
        
        // Status
        public bool IsActive { get; set; } = true;
    }
}

