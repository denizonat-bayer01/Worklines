using System;

namespace wixi.Entities.Concrete.Content
{
    public class UserPreference
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty; // from NameIdentifier
        public string Language { get; set; } = "de"; // de|tr|en|ar
        public string Theme { get; set; } = "light"; // light|dark
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}


