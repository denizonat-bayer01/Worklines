namespace wixi.Entities.DTOs
{
    public class TokenDto
    {
        public string? AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public bool RequiresTwoFactor { get; set; } = false;
        public string? Email { get; set; }
    }
}

