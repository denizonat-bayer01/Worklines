namespace wixi.Entities.DTOs
{
    // Client Create/Update
    public class ClientCreateDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public int? EducationTypeId { get; set; }
        public string? ClientCode { get; set; } // Optional - if provided, use this instead of generating
    }

    public class ClientUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public int? EducationTypeId { get; set; }
    }

    // Client Response
    public class ClientResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? EducationTypeId { get; set; }
        public string? EducationTypeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Related data
        public List<EducationInfoDto> EducationHistory { get; set; } = new();
    }

    // Education Info DTOs
    public class EducationInfoDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string? Country { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
        public decimal? GPA { get; set; }
    }

    public class EducationInfoCreateDto
    {
        public int ClientId { get; set; }
        public string Level { get; set; } = string.Empty; // HighSchool, Bachelor, Master, PhD, etc.
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string? Country { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
        public decimal? GPA { get; set; }
    }

    public class EducationInfoUpdateDto
    {
        public string Level { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string? Country { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
        public decimal? GPA { get; set; }
    }

    // Education Type
    public class EducationTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Pending Client Code DTOs
    public class PendingClientCodeResponseDto
    {
        public int Id { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public long? EmployeeSubmissionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsExpired { get; set; }
        public bool IsValid { get; set; }
        public int? DaysRemaining { get; set; }  // Kaç gün kaldı (negatif ise geçmiş)
        public string Status { get; set; } = string.Empty;  // "Valid", "Used", "Expired"
    }
}
