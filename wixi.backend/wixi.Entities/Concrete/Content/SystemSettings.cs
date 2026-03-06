using System;

namespace wixi.Entities.Concrete.Content
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string SiteUrl { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}

