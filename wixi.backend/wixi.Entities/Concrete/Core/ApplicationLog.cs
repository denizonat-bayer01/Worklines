using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wixi.Entities.Concrete.Core
{
    /// <summary>
    /// Uygulama log kayıtlarını tutan entity
    /// Serilog tarafından otomatik olarak doldurulur
    /// </summary>
    [Table("ApplicationLogs")]
    public class ApplicationLog
    {
        /// <summary>
        /// Log kaydının benzersiz ID'si
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Log mesajı (max 4000 karakter)
        /// </summary>
        [Required]
        [MaxLength(4000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Log seviyesi (Information, Warning, Error, Debug, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// Log kaydının oluşturulma zamanı
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Hata mesajı varsa exception detayları (null olabilir)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Exception { get; set; }

        /// <summary>
        /// Ek property'ler (JSON formatında)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Properties { get; set; }

        /// <summary>
        /// HTTP Request path (varsa)
        /// </summary>
        [MaxLength(500)]
        public string? RequestPath { get; set; }

        /// <summary>
        /// HTTP Request method (GET, POST, etc.)
        /// </summary>
        [MaxLength(10)]
        public string? RequestMethod { get; set; }

        /// <summary>
        /// HTTP Status code (200, 404, 500, etc.)
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Request süresi (milisaniye)
        /// </summary>
        public double? ElapsedMs { get; set; }

        /// <summary>
        /// Kullanıcı ID'si (authenticated user varsa)
        /// </summary>
        [MaxLength(100)]
        public string? UserId { get; set; }

        /// <summary>
        /// Kullanıcı adı (authenticated user varsa)
        /// </summary>
        [MaxLength(200)]
        public string? UserName { get; set; }

        /// <summary>
        /// Sunucu adı / Machine name
        /// </summary>
        [MaxLength(200)]
        public string? MachineName { get; set; }

        /// <summary>
        /// Remote IP adresi
        /// </summary>
        [MaxLength(50)]
        public string? RemoteIP { get; set; }

        /// <summary>
        /// User Agent
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Source Context (logger class/namespace)
        /// </summary>
        [MaxLength(500)]
        public string? SourceContext { get; set; }

        /// <summary>
        /// Request ID / Correlation ID
        /// </summary>
        [MaxLength(100)]
        public string? RequestId { get; set; }

        /// <summary>
        /// Application name
        /// </summary>
        [MaxLength(200)]
        public string? Application { get; set; }

        /// <summary>
        /// Environment (Development, Production, etc.)
        /// </summary>
        [MaxLength(50)]
        public string? Environment { get; set; }
    }
}



