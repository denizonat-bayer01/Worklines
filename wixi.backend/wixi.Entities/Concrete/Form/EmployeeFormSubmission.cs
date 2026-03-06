namespace wixi.Entities.Concrete.Form
{
    public class EmployeeFormSubmission
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Personal Information
        public string? Salutation { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        
        // Qualifications
        public string? Profession { get; set; }
        public int? Experience { get; set; } // Years
        public string? Education { get; set; }
        public string? GermanLevel { get; set; }
        public string? AdditionalInfo { get; set; }
        
        // CV Upload
        public string? CvFileName { get; set; }
        public string? CvFilePath { get; set; }
        public long? CvFileSize { get; set; }
        
        // Additional
        public string? SpecialRequests { get; set; }
        
        // Meta
        public string? RequestIp { get; set; }
        public string? UserAgent { get; set; }
        public string? Language { get; set; }
        
        // Email Log Reference
        public long? EmailLogId { get; set; }
    }
}

