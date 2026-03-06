using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace wixi.Business.Abstract
{
    public class EmailMessage
    {
        public string FromEmail { get; set; } = string.Empty;
        public string? FromName { get; set; }
        public List<string> To { get; set; } = new();
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? BodyHtml { get; set; }
        public string? BodyText { get; set; }
        public Guid? CorrelationId { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
        public List<EmailAttachment>? Attachments { get; set; }
    }

    public class EmailAttachment
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
    }

    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message);
    }
}

