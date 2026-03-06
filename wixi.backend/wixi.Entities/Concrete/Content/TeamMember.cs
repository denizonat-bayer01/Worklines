namespace wixi.Entities.Concrete.Content
{
    public class TeamMember
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Basic Information
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Experience { get; set; } // e.g., "15+"
        
        // Position (Multi-language)
        public string PositionDe { get; set; }
        public string PositionTr { get; set; }
        public string? PositionEn { get; set; }
        
        // Location (Multi-language)
        public string LocationDe { get; set; }
        public string LocationTr { get; set; }
        public string? LocationEn { get; set; }
        
        // Education (Multi-language)
        public string EducationDe { get; set; }
        public string EducationTr { get; set; }
        public string? EducationEn { get; set; }
        
        // Bio (Multi-language)
        public string BioDe { get; set; }
        public string BioTr { get; set; }
        public string? BioEn { get; set; }
        
        // Philosophy (Multi-language, optional)
        public string? PhilosophyDe { get; set; }
        public string? PhilosophyTr { get; set; }
        public string? PhilosophyEn { get; set; }
        
        // Specializations (JSON array stored as string)
        public string? SpecializationsDe { get; set; } // JSON array
        public string? SpecializationsTr { get; set; }
        public string? SpecializationsEn { get; set; }
        
        // Languages (JSON array stored as string)
        public string? LanguagesDe { get; set; } // JSON array
        public string? LanguagesTr { get; set; }
        public string? LanguagesEn { get; set; }
        
        // Achievements (JSON array stored as string)
        public string? AchievementsDe { get; set; } // JSON array
        public string? AchievementsTr { get; set; }
        public string? AchievementsEn { get; set; }
        
        // Ordering
        public int DisplayOrder { get; set; } = 0;
        
        // Status
        public bool IsActive { get; set; } = true;
        
        // Consultation fields
        public bool CanProvideConsultation { get; set; } = false;
        public decimal? ConsultationPrice { get; set; }
        public string? ConsultationCurrency { get; set; }
    }
}

