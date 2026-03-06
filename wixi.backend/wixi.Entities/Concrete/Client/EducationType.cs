namespace wixi.Entities.Concrete.Client
{
    public class EducationType
    {
        public int Id { get; set; }
        
        // Identification
        public string Code { get; set; } = string.Empty;  // "university", "vocational", "masterCraftsman"
        public string Name { get; set; } = string.Empty;  // "Üniversite Mezunu"
        public string NameEn { get; set; } = string.Empty;  // "University Graduate"
        public string? Description { get; set; }
        public string? DescriptionEn { get; set; }
        
        // Display
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
        public string? IconName { get; set; }
        
        // Relations
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
        public virtual ICollection<Document.DocumentType> RequiredDocuments { get; set; } = new List<Document.DocumentType>();
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

