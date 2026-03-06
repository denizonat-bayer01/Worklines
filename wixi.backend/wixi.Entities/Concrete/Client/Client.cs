using wixi.Entities.Concrete.Identity;

namespace wixi.Entities.Concrete.Client
{
    public class Client
    {
        public int Id { get; set; }
        
        // User relationship
        public int UserId { get; set; }
        public virtual AppUser User { get; set; } = null!;
        
        // Personal Info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        
        // Client-specific
        public string ClientCode { get; set; } = string.Empty;  // WP-84321
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public ClientStatus Status { get; set; } = ClientStatus.Active;
        
        // Education
        public int? EducationTypeId { get; set; }
        public virtual EducationType? EducationType { get; set; }
        
        // Relations
        public virtual ICollection<Document.Document> Documents { get; set; } = new List<Document.Document>();
        public virtual ICollection<Application.Application> Applications { get; set; } = new List<Application.Application>();
        public virtual ICollection<EducationInfo> EducationHistory { get; set; } = new List<EducationInfo>();
        public virtual ICollection<Support.SupportTicket> SupportTickets { get; set; } = new List<Support.SupportTicket>();
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }  // Soft delete
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
        public bool IsActive => Status == ClientStatus.Active && DeletedAt == null;
    }
    
    public enum ClientStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Completed = 4
    }
}

