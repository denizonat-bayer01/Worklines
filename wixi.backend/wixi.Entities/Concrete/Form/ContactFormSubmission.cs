namespace wixi.Entities.Concrete.Form
{
    public class ContactFormSubmission
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? Age { get; set; }
        public string? Nationality { get; set; }
        
        // Education
        public string? Education { get; set; }
        public string? FieldOfStudy { get; set; }
        public string? WorkExperience { get; set; }
        
        // Languages
        public string? GermanLevel { get; set; }
        public string? EnglishLevel { get; set; }
        
        // Interest
        public string? Interest { get; set; }
        public string? PreferredCity { get; set; }
        public string? Timeline { get; set; }
        
        // Additional
        public string? Message { get; set; }
        public bool PrivacyConsent { get; set; }
        public bool Newsletter { get; set; }
        
        // Meta
        public string? RequestIp { get; set; }
        public string? UserAgent { get; set; }
        public string? Language { get; set; }
        
        // Email Log Reference
        public long? EmailLogId { get; set; }
    }
}

