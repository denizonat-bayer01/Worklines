namespace wixi.Entities.DTOs
{
    // Application Create/Update
    public class ApplicationCreateDto
    {
        public int ClientId { get; set; }
        public int TemplateId { get; set; }
        public string? Notes { get; set; }
    }

    public class ApplicationUpdateDto
    {
        public string? Notes { get; set; }
    }

    // Application Response
    public class ApplicationResponseDto
    {
        public long Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CurrentStepOrder { get; set; }
        public string? CurrentStepName { get; set; }
        public int TotalSteps { get; set; }
        public int CompletedSteps { get; set; }
        public double ProgressPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
        public string? Notes { get; set; }
        public List<ApplicationStepResponseDto> Steps { get; set; } = new();
    }

    // Application Step Response
    public class ApplicationStepResponseDto
    {
        public long Id { get; set; }
        public long ApplicationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int StepOrder { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public List<ApplicationSubStepResponseDto> SubSteps { get; set; } = new();
    }

    // Application Sub-Step Response
    public class ApplicationSubStepResponseDto
    {
        public long Id { get; set; }
        public long StepId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SubStepOrder { get; set; }
        public bool IsCompleted { get; set; }
        public string Status { get; set; } = string.Empty; // NotStarted, InProgress, Completed, etc.
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
    }

    // Step Update
    public class StepUpdateDto
    {
        public string Status { get; set; } = string.Empty; // NotStarted, InProgress, Completed, Blocked
        public string? Notes { get; set; }
        public int? AssignedToUserId { get; set; }
    }

    // Sub-Step Update
    public class SubStepUpdateDto
    {
        public bool IsCompleted { get; set; }
        public string? Notes { get; set; }
    }

    // Application History
    public class ApplicationHistoryDto
    {
        public long Id { get; set; }
        public long ApplicationId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }

    // Application Template Info
    public class ApplicationTemplateDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EstimatedDurationDays { get; set; }
        public bool IsActive { get; set; }
        public List<ApplicationStepTemplateDto> Steps { get; set; } = new();
    }

    public class ApplicationStepTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int StepOrder { get; set; }
        public int EstimatedDurationDays { get; set; }
        public bool IsRequired { get; set; }
        public List<ApplicationSubStepTemplateDto> SubSteps { get; set; } = new();
    }

    public class ApplicationSubStepTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SubStepOrder { get; set; }
        public bool IsRequired { get; set; }
    }
}

