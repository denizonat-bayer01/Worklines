using wixi.Entities.Concrete.Identity;

namespace wixi.Entities.Concrete.Support
{
    public class SupportTicket
    {
        public long Id { get; set; }
        
        // Client relationship
        public int ClientId { get; set; }
        public virtual Client.Client Client { get; set; } = null!;
        
        // Ticket info
        public string TicketNumber { get; set; } = string.Empty;  // "TKT-2023-12345"
        public string Subject { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public TicketPriority Priority { get; set; } = TicketPriority.Normal;
        public TicketCategory Category { get; set; }
        
        // Assignment
        public int? AssignedToId { get; set; }
        public virtual AppUser? AssignedTo { get; set; }
        
        // SLA tracking
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? DueDate { get; set; }
        
        // Additional info
        public string? Resolution { get; set; }
        public string? CloseReason { get; set; }
        public int? Rating { get; set; }  // 1-5 customer rating
        public string? RatingComment { get; set; }
        
        // Relations
        public virtual ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Computed properties
        public bool IsOpen => Status == TicketStatus.Open || Status == TicketStatus.InProgress;
        public bool IsClosed => Status == TicketStatus.Closed;
        public TimeSpan? ResponseTime => FirstResponseAt.HasValue 
            ? FirstResponseAt.Value - CreatedAt 
            : null;
        public TimeSpan? ResolutionTime => ResolvedAt.HasValue 
            ? ResolvedAt.Value - CreatedAt 
            : null;
    }
    
    public enum TicketStatus
    {
        Open = 1,
        InProgress = 2,
        WaitingForCustomer = 3,
        Resolved = 4,
        Closed = 5,
        Reopened = 6
    }
    
    public enum TicketPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Urgent = 4,
        Critical = 5
    }
    
    public enum TicketCategory
    {
        General = 1,
        Documents = 2,
        Application = 3,
        Technical = 4,
        Billing = 5,
        Complaint = 6,
        Other = 99
    }
}

