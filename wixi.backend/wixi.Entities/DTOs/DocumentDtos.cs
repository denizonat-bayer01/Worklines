namespace wixi.Entities.DTOs
{
    // Document Upload Request
    public class DocumentUploadDto
    {
        public int ClientId { get; set; }
        public int DocumentTypeId { get; set; }
    }

    // Document Response
    public class DocumentResponseDto
    {
        public long Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string FileSizeFormatted { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime UploadedAt { get; set; }
        
        // Review info (if exists)
        public bool HasReview { get; set; }
        public string? ReviewDecision { get; set; }
        public string? FeedbackMessage { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }

    // Document List for Client
    public class ClientDocumentListDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int TotalDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public int AcceptedDocuments { get; set; }
        public int RejectedDocuments { get; set; }
        public List<DocumentResponseDto> Documents { get; set; } = new();
    }

    // Document Type Info
    public class DocumentTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public string? AllowedFileTypes { get; set; }
        public long? MaxFileSizeBytes { get; set; }
        public string? MaxFileSizeFormatted { get; set; }
        public bool RequiresApproval { get; set; }
        public string? Note { get; set; }
        public int? EducationTypeId { get; set; }
        public string? EducationTypeName { get; set; }
    }

    // Document Review DTO
    public class DocumentReviewDto
    {
        public long DocumentId { get; set; }
        public string Decision { get; set; } = string.Empty; // Accepted, Rejected, MissingInfo
        public string? ReviewNote { get; set; }
        public string? FeedbackMessage { get; set; }
    }
}

