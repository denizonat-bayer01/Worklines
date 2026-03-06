using System;
using System.ComponentModel.DataAnnotations;

namespace wixi.Entities.Concrete.Email
{
    /// <summary>
    /// Email template entity for storing email templates that can be edited via UI
    /// Template keys: ContactForm, EmployerForm, EmployeeForm
    /// </summary>
    public class EmailTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty; // ContactForm, EmployerForm, EmployeeForm

        [Required]
        [StringLength(500)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string BodyHtml { get; set; } = string.Empty; // HTML template with placeholders like {{FirstName}}

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string? UpdatedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; } // Optimistic concurrency
    }
}

