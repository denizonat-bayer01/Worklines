namespace wixi.Entities.Concrete.Client
{
    public class EducationInfo
    {
        public int Id { get; set; }
        
        // Client relationship
        public int ClientId { get; set; }
        public virtual Client Client { get; set; } = null!;
        
        // Education details
        public EducationLevel Level { get; set; }
        public string Degree { get; set; } = string.Empty;  // "Master of Science in Data Science"
        public string Institution { get; set; } = string.Empty;  // "Stanford University"
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string? Country { get; set; }
        public bool IsCurrent { get; set; } = false;
        
        // Additional info
        public string? Description { get; set; }
        public decimal? GPA { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    
    public enum EducationLevel
    {
        HighSchool = 1,
        VocationalSchool = 2,
        Associate = 3,
        Bachelor = 4,
        Master = 5,
        PhD = 6,
        Apprenticeship = 7,  // Kalfalık
        Mastership = 8       // Ustalık
    }
}

