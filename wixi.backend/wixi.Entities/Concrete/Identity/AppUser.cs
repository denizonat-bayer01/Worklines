using Microsoft.AspNetCore.Identity;

namespace wixi.Entities.Concrete.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenEndDate { get; set; }
        
        // 2FA için alanlar (IdentityUser'dan gelen TwoFactorEnabled override edildi)
        public new bool TwoFactorEnabled { get; set; } = false;
        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorCodeExpiration { get; set; }
    }
}

