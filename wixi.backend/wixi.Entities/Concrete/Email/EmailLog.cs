using System;

namespace wixi.Entities.Concrete.Email
{
	public class EmailLog
	{
		public long Id { get; set; }
		public Guid? CorrelationId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string FromEmail { get; set; } = string.Empty;
		public string? FromName { get; set; }
		public string ToEmails { get; set; } = string.Empty;
		public string? CcEmails { get; set; }
		public string? BccEmails { get; set; }
		public string? Subject { get; set; }
		public string? BodyHtml { get; set; }
		public string? BodyText { get; set; }
		public string? Attachments { get; set; }
		public byte Status { get; set; } // 0:Queued,1:Sent,2:Failed,3:Retrying,4:Cancelled
		public int AttemptCount { get; set; }
		public DateTime? LastAttemptAt { get; set; }
		public string? LastError { get; set; }
		public string? SmtpHost { get; set; }
		public int? SmtpPort { get; set; }
		public bool? UsedSsl { get; set; }
		public string? UsedUserName { get; set; }
		public string? TemplateKey { get; set; }
		public string? MetadataJson { get; set; }
		public string? RequestIp { get; set; }
		public string? UserAgent { get; set; }
		public string? CreatedBy { get; set; }
	}
}
