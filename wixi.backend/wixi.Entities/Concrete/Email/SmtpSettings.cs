using System;

namespace wixi.Entities.Concrete.Email
{
	public class SmtpSettings
	{
		public int Id { get; set; }
		public string Host { get; set; } = string.Empty;
		public int Port { get; set; }
		public bool UseSsl { get; set; }
		public string UserName { get; set; } = string.Empty;
		// Encrypted at rest
		public string PasswordEnc { get; set; } = string.Empty;
		public string FromName { get; set; } = string.Empty;
		public string FromEmail { get; set; } = string.Empty;
		public int? TimeoutMs { get; set; }
		public int RetryCount { get; set; } = 3;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public string? UpdatedBy { get; set; }
		public byte[]? RowVersion { get; set; }
	}
}
