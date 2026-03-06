namespace wixi.Entities.Concrete.Client
{
    /// <summary>
    /// Müşteri kodları için pending (bekleyen) kayıtlar
    /// Client profili oluşturulmadan önce müşteri kodunu saklamak için
    /// </summary>
    public class PendingClientCode
    {
        public int Id { get; set; }
        
        // Client Code Info
        public string ClientCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        
        // Validity
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        
        // Source
        public long? EmployeeSubmissionId { get; set; }  // Hangi employee submission'dan geldi
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Computed
        public bool IsExpired => DateTime.UtcNow > ExpirationDate;
        public bool IsValid => !IsUsed && !IsExpired;
    }
}

