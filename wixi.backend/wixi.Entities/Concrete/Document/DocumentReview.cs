using wixi.Entities.Concrete.Identity;

namespace wixi.Entities.Concrete.Document
{
    public class DocumentReview
    {
        public long Id { get; set; }
        
        // Document relationship
        public long DocumentId { get; set; }
        public virtual Document Document { get; set; } = null!;
        
        // Reviewer relationship
        public int ReviewerId { get; set; }
        public virtual AppUser Reviewer { get; set; } = null!;
        
        // Review decision
        public DocumentStatus Decision { get; set; }
        public string? ReviewNote { get; set; }  // Admin internal note
        public string? FeedbackMessage { get; set; }  // Message to client
        
        // Review details
        public int ReviewDurationMinutes { get; set; }  // How long review took
        public string? RejectionReason { get; set; }  // Categorized rejection reason
        
        // Timestamps
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

