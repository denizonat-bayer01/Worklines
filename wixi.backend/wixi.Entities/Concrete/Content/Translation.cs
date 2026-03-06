using System;

namespace wixi.Entities.Concrete.Content
{
    public class Translation
    {
        public long Id { get; set; }
        public string Key { get; set; } = string.Empty; // e.g. "contact.title"

        public string? De { get; set; }
        public string? Tr { get; set; }
        public string? En { get; set; }
        public string? Ar { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}


