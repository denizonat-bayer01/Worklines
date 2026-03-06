using wixi.Entities.Concrete.Identity;

namespace wixi.Entities.Concrete.Support
{
    public class FAQ
    {
        public int Id { get; set; }
        
        // Content
        public string Question { get; set; } = string.Empty;
        public string QuestionEn { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string AnswerEn { get; set; } = string.Empty;
        
        // Categorization
        public FAQCategory Category { get; set; }
        public string? Tags { get; set; }  // Comma-separated tags for search
        
        // Display
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        
        // Statistics
        public int ViewCount { get; set; } = 0;
        public int HelpfulCount { get; set; } = 0;
        public int NotHelpfulCount { get; set; } = 0;
        
        // Additional info
        public string? RelatedLink { get; set; }
        public string? VideoUrl { get; set; }
        
        // Author
        public int? AuthorId { get; set; }
        public virtual AppUser? Author { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        
        // Computed properties
        public decimal HelpfulRatio => (HelpfulCount + NotHelpfulCount) > 0 
            ? (decimal)HelpfulCount / (HelpfulCount + NotHelpfulCount) * 100 
            : 0;
        public bool IsPublished => PublishedAt.HasValue && PublishedAt.Value <= DateTime.UtcNow;
    }
    
    public enum FAQCategory
    {
        General = 1,
        Documents = 2,
        Process = 3,
        Communication = 4,
        Technical = 5,
        Billing = 6,
        Visa = 7,
        WorkPermit = 8,
        Recognition = 9
    }
}

