using System.ComponentModel.DataAnnotations;

namespace wixi.Entities.DTOs
{
    public class UserForRegisterDto
    {
        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad alanı zorunludur")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Müşteri kodu (optional - eğer varsa validasyon yapılır ve client profili oluşturulur)
        /// </summary>
        public string? ClientCode { get; set; }

        // Additional client information
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; } // Place of birth can be used as nationality
        
        [Required(ErrorMessage = "Eğitim türü seçimi zorunludur")]
        public int EducationTypeId { get; set; }

        // Education information
        public List<EducationInfoCreateDto>? EducationHistory { get; set; }
    }
}

