using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using wixi.Core.Utilities.Security.Protection;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.Business.Concrete
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly WixiDbContext _db;
        private readonly ISmtpSettingsService _settingsService;
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly ISecretProtector _protector;

        public SmtpEmailSender(WixiDbContext db, ISmtpSettingsService settingsService, ILogger<SmtpEmailSender> logger, ISecretProtector protector)
        {
            _db = db;
            _settingsService = settingsService;
            _logger = logger;
            _protector = protector;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var settings = await _settingsService.GetAsync();
            if (settings == null) throw new InvalidOperationException("SMTP settings not configured");

            var retries = settings.RetryCount <= 0 ? 1 : settings.RetryCount;
            var attempt = 0;
            Exception? last = null;
            do
            {
                attempt++;
                var log = new EmailLog
                {
                    CorrelationId = message.CorrelationId,
                    MetadataJson = message.Metadata != null ? System.Text.Json.JsonSerializer.Serialize(message.Metadata) : null,
                    FromEmail = string.IsNullOrEmpty(message.FromEmail) ? settings.FromEmail : message.FromEmail,
                    FromName = string.IsNullOrEmpty(message.FromName) ? settings.FromName : message.FromName,
                    ToEmails = string.Join(',', message.To ?? Enumerable.Empty<string>()),
                    CcEmails = message.Cc != null ? string.Join(',', message.Cc) : null,
                    BccEmails = message.Bcc != null ? string.Join(',', message.Bcc) : null,
                    Subject = message.Subject,
                    BodyHtml = message.BodyHtml,
                    BodyText = message.BodyText,
                    Status = 0,
                    AttemptCount = attempt,
                    SmtpHost = settings.Host,
                    SmtpPort = settings.Port,
                    UsedSsl = settings.UseSsl,
                    UsedUserName = settings.UserName,
                };
                _db.EmailLogs.Add(log);
                await _db.SaveChangesAsync();

                try
                {
                    // Use password directly from database (stored as plaintext)
                    string password = settings.PasswordEnc ?? string.Empty;
                    
                    using var smtp = new SmtpClient(settings.Host, settings.Port)
                    {
                        EnableSsl = settings.UseSsl,
                        Credentials = new NetworkCredential(settings.UserName, password)
                    };

                    var fromEmail = string.IsNullOrEmpty(message.FromEmail) ? settings.FromEmail : message.FromEmail;
                    var fromName = string.IsNullOrEmpty(message.FromName) ? settings.FromName : message.FromName;
                    
                    using var mail = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = message.Subject,
                        Body = !string.IsNullOrEmpty(message.BodyHtml) ? message.BodyHtml : message.BodyText,
                        IsBodyHtml = !string.IsNullOrEmpty(message.BodyHtml)
                    };
                    foreach (var t in message.To) mail.To.Add(t);
                    if (message.Cc != null) foreach (var c in message.Cc) mail.CC.Add(c);
                    if (message.Bcc != null) foreach (var b in message.Bcc) mail.Bcc.Add(b);

                    // Add attachments if any
                    if (message.Attachments != null && message.Attachments.Any())
                    {
                        foreach (var attachment in message.Attachments)
                        {
                            try
                            {
                                if (System.IO.File.Exists(attachment.FilePath))
                                {
                                    var mailAttachment = new System.Net.Mail.Attachment(attachment.FilePath);
                                    mailAttachment.Name = attachment.FileName;
                                    if (!string.IsNullOrEmpty(attachment.ContentType))
                                    {
                                        mailAttachment.ContentType = new System.Net.Mime.ContentType(attachment.ContentType);
                                    }
                                    mail.Attachments.Add(mailAttachment);
                                    _logger.LogInformation("Added attachment to email: {FileName}", attachment.FileName);
                                }
                                else
                                {
                                    _logger.LogWarning("Attachment file not found: {FilePath}", attachment.FilePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error adding attachment {FileName}: {Error}", attachment.FileName, ex.Message);
                                // Continue with email send even if attachment fails
                            }
                        }
                    }

                    await smtp.SendMailAsync(mail);

                    log.Status = 1; // sent
                    log.LastAttemptAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                    return; // Success, exit immediately
                }
                catch (Exception ex)
                {
                    log.Status = 2; // failed
                    log.LastAttemptAt = DateTime.UtcNow;
                    log.LastError = ex.Message.Length > 4000 ? ex.Message.Substring(0, 4000) : ex.Message;
                    await _db.SaveChangesAsync();
                    _logger.LogError(ex, "Email send failed on attempt {Attempt}", attempt);
                    last = ex;
                }
                
                if (attempt < retries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Min(30, attempt * 2)));
                }
            } while (attempt < retries);
            if (last != null) throw last;
        }
    }
}

