namespace wixi.Entities.Concrete.Application
{
    public class ApplicationStepTemplate
    {
        public int Id { get; set; }
        
        // Application template relationship
        public int ApplicationTemplateId { get; set; }
        public virtual ApplicationTemplate Template { get; set; } = null!;
        
        // Step info
        public string Title { get; set; } = string.Empty;  // "Denklik İşlemleri"
        public string TitleEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionEn { get; set; }
        
        // Configuration
        public int StepOrder { get; set; }
        public string? IconName { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsActive { get; set; } = true;
        
        // Duration estimate (in days)
        public int? EstimatedDurationDays { get; set; }
        
        // Relations
        public virtual ICollection<ApplicationSubStepTemplate> SubStepTemplates { get; set; } = new List<ApplicationSubStepTemplate>();
        public virtual ICollection<ApplicationStep> Steps { get; set; } = new List<ApplicationStep>();
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

