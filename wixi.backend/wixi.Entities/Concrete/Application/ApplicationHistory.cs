using wixi.Entities.Concrete.Identity;

namespace wixi.Entities.Concrete.Application
{
    public class ApplicationHistory
    {
        public long Id { get; set; }
        
        // Application relationship
        public long ApplicationId { get; set; }
        public virtual Application Application { get; set; } = null!;
        
        // Action info
        public string Action { get; set; } = string.Empty;  // "StatusChanged", "StepCompleted", "DocumentApproved"
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Description { get; set; }
        
        // User info
        public int? UserId { get; set; }  // Who made the change (admin/system)
        public virtual AppUser? User { get; set; }
        public string? UserType { get; set; }  // "Admin", "System", "Client"
        
        // Additional metadata
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        
        // Metadata as JSON string (will be serialized/deserialized)
        public string? MetadataJson { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

