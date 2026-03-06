namespace wixi.Entities.Concrete.Form
{
    public class EmployerFormSubmission
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Company Information
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Industry { get; set; }
        public string? CompanySize { get; set; }
        
        // Position Requirements
        public string Positions { get; set; }
        public string Requirements { get; set; }
        
        // Additional
        public string? Message { get; set; }
        public string? SpecialRequests { get; set; }
        
        // Meta
        public string? RequestIp { get; set; }
        public string? UserAgent { get; set; }
        public string? Language { get; set; }
        
        // Email Log Reference
        public long? EmailLogId { get; set; }
    }
}

