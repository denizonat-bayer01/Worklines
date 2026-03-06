namespace wixi.Entities.Concrete.Application
{
    public class ApplicationTemplate
    {
        public int Id { get; set; }
        
        // Template info
        public string Name { get; set; } = string.Empty;  // "Denklik İşlem Süreci"
        public string NameEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionEn { get; set; }
        
        // Template type
        public ApplicationType Type { get; set; }
        
        // Configuration
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public int DisplayOrder { get; set; }
        public string? IconName { get; set; }
        
        // Duration estimates (in days)
        public int? EstimatedDurationDays { get; set; }
        public int? MinDurationDays { get; set; }
        public int? MaxDurationDays { get; set; }
        
        // Relations
        public virtual ICollection<ApplicationStepTemplate> StepTemplates { get; set; } = new List<ApplicationStepTemplate>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

