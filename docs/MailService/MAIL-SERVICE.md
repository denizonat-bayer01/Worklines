# Mail Service - Implementation Plan and Checklist

Tag: @MailService
Branch: mailServis

## Goal
Add SMTP-based mail sending with runtime-configurable settings stored in the database and editable from the Admin UI.

## Architecture Overview
- In-process module inside `wixi.WebAPI` (no separate microservice initially)
- Settings persisted in DB (`SmtpSettings`) and cached in memory
- Password encrypted at rest; never logged
- SMTP sender with retry policy (Polly)

## Database
- Table: `SmtpSettings`
  - Id (int, PK)
  - Host (nvarchar(255))
  - Port (int)
  - UseSsl (bit)
  - UserName (nvarchar(255))
  - PasswordEnc (varbinary / nvarchar)  // encrypted
  - FromName (nvarchar(255))
  - FromEmail (nvarchar(255))
  - TimeoutMs (int, nullable)
  - RetryCount (int, default 3)
  - UpdatedAt (datetime2)
  - UpdatedBy (nvarchar(100))
  - RowVersion (rowversion) // optimistic concurrency

Optional (later): `EmailTemplate` (Key, Subject, BodyHtml, BodyText, IsActive, UpdatedAt)

## Services
- `ISecretProtector` (AES-256/GCM or DPAPI) for encrypt/decrypt
- `ISmtpSettingsService` (Get/Update with cache + concurrency)
- `IEmailSender` (SendAsync) with SMTP (MailKit or System.Net.Mail) + Polly retry

## API (Admin)
- GET `/api/admin/email/smtp-settings` (mask password)
- PUT `/api/admin/email/smtp-settings` (encrypt password if changed)
- POST `/api/admin/email/test-send` (to address required)
- AuthZ: Admin only
- Rate-limit for `test-send`

## Caching
- `IMemoryCache` (e.g., 5 min sliding). Invalidate on update.

## Logging/Security
- Serilog filter/redaction for `Password`, `PasswordEnc`, and SMTP credentials
- No sensitive data in logs

## Health Checks
- SMTP connectivity/auth check endpoint (optional in prod)

## Incremental Checklist
- [ ] Create EF entity + migration for `SmtpSettings` (@smtp-db-schema)
- [ ] Implement `ISecretProtector` with key sourcing from env vars (@encryption-service)
- [ ] Add repository/service with memory cache + invalidation (@settings-repo-cache)
- [ ] Implement `IEmailSender` using SMTP + Polly retry (@smtp-email-sender)
- [ ] Admin endpoints: get/update settings, test-send (@admin-api-endpoints)
- [ ] Add Serilog redaction for sensitive fields (@serilog-redaction)
- [ ] Add SMTP health check (@smtp-healthcheck)
- [ ] (Optional) Email templates table and CRUD (@email-templates)
- [ ] Docs: usage + ops notes (@docs-usage)

## Notes
- Strato SMTP will be used; exact host/port/SSL values to be provided.
- Default fallback via appsettings may exist but DB is source of truth.
- Consider future background queue worker if traffic increases.

## System Tables (Prefix: wixi_)

### Table: wixi_EmailLog
Detaylı e-posta gönderim loglarını saklar.

Columns:
- Id (bigint, PK, identity)
- CorrelationId (uniqueidentifier, nullable) // aynı işlem zinciri için
- CreatedAt (datetime2, not null, default sysutcdatetime())
- FromEmail (nvarchar(255), not null)
- FromName (nvarchar(255), null)
- ToEmails (nvarchar(max), not null) // virgül ile ayrılmış
- CcEmails (nvarchar(max), null)
- BccEmails (nvarchar(max), null)
- Subject (nvarchar(500), null)
- BodyHtml (nvarchar(max), null)
- BodyText (nvarchar(max), null)
- Attachments (nvarchar(max), null) // dosya adları veya içerik hash’leri
- Status (tinyint, not null) // 0:Queued, 1:Sent, 2:Failed, 3:Retrying, 4:Cancelled
- AttemptCount (int, not null, default 0)
- LastAttemptAt (datetime2, null)
- LastError (nvarchar(max), null) // hata mesajı (kırpılmış)
- SmtpHost (nvarchar(255), null)
- SmtpPort (int, null)
- UsedSsl (bit, null)
- UsedUserName (nvarchar(255), null)
- TemplateKey (nvarchar(100), null)
- MetadataJson (nvarchar(max), null) // dinamik alanlar
- RequestIp (nvarchar(64), null)
- UserAgent (nvarchar(512), null)
- CreatedBy (nvarchar(100), null)

Indexes:
- IX_wixi_EmailLog_CreatedAt (nonclustered)
- IX_wixi_EmailLog_Status_CreatedAt (nonclustered)
- IX_wixi_EmailLog_CorrelationId (nonclustered)

Retention:
- Operasyonel ihtiyaçlara göre 90-180 gün saklama; arşiv/job ile temizlik.

Logging Kuralları:
- Şifre, token, credential gibi hassas bilgiler asla kaydedilmez.
- BodyHtml/BodyText çok büyükse kısaltılabilir (örn. 2000 char).

## Checklist Updates
- [x] Create EF entity + migration for `wixi_EmailLog` and indexes (@smtp-db-schema)
- [x] Persist send attempts and outcomes to `wixi_EmailLog` (@email-logging)

---

## Usage Guide

### Initial Configuration (Admin Only)

Configure SMTP settings via API:

```bash
PUT /api/admin/email/smtp-settings
Authorization: Bearer <admin_token>
Content-Type: application/json

{
  "host": "smtp.strato.de",
  "port": 465,
  "useSsl": true,
  "userName": "info@worklines.de",
  "password": "your_password",
  "fromName": "Worklines",
  "fromEmail": "info@worklines.de",
  "retryCount": 3,
  "timeoutMs": 30000
}
```

**Note:** Password is encrypted at rest using AES-256-GCM (if `MAIL_ENC_KEY` env var set) or Windows DPAPI.

### Health Check

```bash
GET /api/admin/email/health
Authorization: Bearer <admin_token>

Response:
{
  "healthy": true,
  "message": "SMTP smtp.strato.de:465 reachable"
}
```

### Test Email

```bash
POST /api/admin/email/test-send
Authorization: Bearer <admin_token>
Content-Type: application/json

{
  "to": "test@example.com",
  "subject": "Test Email",
  "body": "This is a test."
}
```

### Sending Emails Programmatically

```csharp
public class YourService
{
    private readonly IEmailSender _emailSender;

    public YourService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendWelcomeEmail(string userEmail)
    {
        await _emailSender.SendAsync(new EmailMessage
        {
            To = new List<string> { userEmail },
            Subject = "Welcome to Worklines",
            BodyHtml = "<h1>Welcome!</h1><p>Thanks for joining us.</p>",
            BodyText = "Welcome! Thanks for joining us."
        });
    }
}
```

### Query Email Logs

```csharp
// Get failed emails in last 24h
var failedLogs = await _db.EmailLogs
    .Where(e => e.Status == 2 && e.CreatedAt > DateTime.UtcNow.AddDays(-1))
    .OrderByDescending(e => e.CreatedAt)
    .ToListAsync();
```

---

## Environment Variables

- **MAIL_ENC_KEY** (optional): Base64-encoded 32-byte AES key for password encryption. If not set, Windows DPAPI is used (Windows only).

Example (PowerShell):
```powershell
$key = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
$env:MAIL_ENC_KEY = $key
```

---

## Operations Notes

- **Retry Logic**: Emails are retried on failure (configured via `RetryCount`). Each attempt is logged in `wixi_EmailLog`.
- **Cache**: SMTP settings are cached for 5 minutes; manual cache invalidation via service update.
- **Log Retention**: Consider archiving `wixi_EmailLog` rows older than 90-180 days.
- **Security**: Password fields are redacted in Serilog; never logged in plaintext.

---

## Troubleshooting

**Email not sending:**
1. Check SMTP health: `GET /api/admin/email/health`
2. Verify settings: `GET /api/admin/email/smtp-settings`
3. Check `wixi_EmailLog` for error messages (Status=2)
4. Ensure firewall allows outbound port 465/587

**"SMTP settings not configured":**
- Run `PUT /api/admin/email/smtp-settings` with valid config.

**Password encryption issues on Linux:**
- Set `MAIL_ENC_KEY` environment variable (DPAPI is Windows-only).

---
