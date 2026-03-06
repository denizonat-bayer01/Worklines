namespace wixi.Entities.Concrete.Core
{
    public class TokenBlacklist
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime BlacklistedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

