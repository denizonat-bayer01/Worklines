namespace wixi.Core.Configuration
{
    public class JwtTokenOptions
    {
        public string Audience { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public int AccessTokenExpiration { get; set; }
        public string SecurityKey { get; set; } = string.Empty;
        public int RefreshTokenExpiration { get; set; }
    }
}

