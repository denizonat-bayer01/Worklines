namespace wixi.Entities.Concrete.Application
{
    public class ApplicationSubStepTemplate
    {
        public int Id { get; set; }
        
        // Step template relationship
        public int StepTemplateId { get; set; }
        public virtual ApplicationStepTemplate StepTemplate { get; set; } = null!;
        
        // Sub-step info
        public string Name { get; set; } = string.Empty;  // "Belgeler Yüklendi"
        public string NameEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionEn { get; set; }
        
        // Configuration
        public int SubStepOrder { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsActive { get; set; } = true;
        
        // Duration estimate (in days)
        public int? EstimatedDurationDays { get; set; }
        
        // Relations
        public virtual ICollection<ApplicationSubStep> SubSteps { get; set; } = new List<ApplicationSubStep>();
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

