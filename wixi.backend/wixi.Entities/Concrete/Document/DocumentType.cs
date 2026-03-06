namespace wixi.Entities.Concrete.Document
{
    public class DocumentType
    {
        public int Id { get; set; }
        
        // Identification
        public string Code { get; set; } = string.Empty;  // "passport", "diploma", "cv"
        public string Name { get; set; } = string.Empty;  // "Pasaport (Renkli Fotokopi - PDF)"
        public string NameEn { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionEn { get; set; }
        
        // Requirements
        public bool IsRequired { get; set; } = true;
        public bool IsActive { get; set; } = true;
        
        // Education type relationship
        public int? EducationTypeId { get; set; }
        public virtual Client.EducationType? EducationType { get; set; }
        
        // Validation rules
        public string? AllowedFileTypes { get; set; }  // ".pdf,.jpg,.png"
        public long? MaxFileSizeBytes { get; set; }  // 10485760 (10MB)
        public bool RequiresApproval { get; set; } = true;
        
        // Display
        public int DisplayOrder { get; set; }
        public string? IconName { get; set; }
        public string? Note { get; set; }  // "20€ ücret karşılığında Almanca CV hazırlama hizmeti"
        public string? NoteEn { get; set; }
        
        // Relations
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

