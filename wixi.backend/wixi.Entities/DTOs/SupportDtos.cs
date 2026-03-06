namespace wixi.Entities.DTOs
{
    // Support Ticket DTOs
    public class SupportTicketCreateDto
    {
        public int ClientId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
        public string Category { get; set; } = "General"; // General, Technical, Billing, Other
    }

    public class SupportTicketUpdateDto
    {
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Open";
    }

    public class SupportTicketResponseDto
    {
        public long Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int MessageCount { get; set; }
        public List<SupportMessageDto> Messages { get; set; } = new();
    }

    // Support Message DTOs
    public class SupportMessageCreateDto
    {
        public long TicketId { get; set; }
        public int SenderId { get; set; }
        public string MessageText { get; set; } = string.Empty;
    }

    public class SupportMessageDto
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty; // Client, Admin, Support
        public string MessageText { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }

    // FAQ DTOs
    public class FAQDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string QuestionEn { get; set; } = string.Empty;
        public string AnswerEn { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class FAQCreateDto
    {
        public string Category { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string QuestionEn { get; set; } = string.Empty;
        public string AnswerEn { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }

    public class FAQUpdateDto
    {
        public string Category { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string QuestionEn { get; set; } = string.Empty;
        public string AnswerEn { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    // Ticket Assignment
    public class TicketAssignDto
    {
        public int AssignedToUserId { get; set; }
    }

    // Ticket Status Update
    public class TicketStatusUpdateDto
    {
        public string Status { get; set; } = string.Empty; // Open, InProgress, Resolved, Closed
    }
}

