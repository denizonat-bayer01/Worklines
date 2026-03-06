using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/email")]
    [Authorize(Roles = "Admin")]
    public class AdminEmailController : ControllerBase
    {
        private readonly ISmtpSettingsService _settingsService;
        private readonly IEmailSender _emailSender;
        private readonly ISmtpHealthCheck _healthCheck;
        private readonly IEmailTemplateService _templateService;
        private readonly WixiDbContext _db;
        private readonly ILogger<AdminEmailController> _logger;

        public AdminEmailController(
            ISmtpSettingsService settingsService, 
            IEmailSender emailSender, 
            ISmtpHealthCheck healthCheck,
            IEmailTemplateService templateService,
            WixiDbContext db, 
            ILogger<AdminEmailController> logger)
        {
            _settingsService = settingsService;
            _emailSender = emailSender;
            _healthCheck = healthCheck;
            _templateService = templateService;
            _db = db;
            _logger = logger;
        }

        [HttpGet("smtp-settings")]
        public async Task<ActionResult<SmtpSettings>> GetSettings()
        {
            var s = await _settingsService.GetAsync();
            if (s == null) return NotFound();
            return Ok(new
            {
                s.Id,
                s.Host,
                s.Port,
                s.UseSsl,
                s.UserName,
                Password = "******",
                s.FromName,
                s.FromEmail,
                s.TimeoutMs,
                s.RetryCount,
                s.UpdatedAt,
                s.UpdatedBy
            });
        }

        public class UpdateSmtpRequest
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public bool UseSsl { get; set; }
            public string UserName { get; set; }
            public string? Password { get; set; } // optional; update only if provided
            public string FromName { get; set; }
            public string FromEmail { get; set; }
            public int? TimeoutMs { get; set; }
            public int RetryCount { get; set; } = 3;
        }

        [HttpPut("smtp-settings")]
        public async Task<ActionResult> UpdateSettings([FromBody] UpdateSmtpRequest req)
        {
            var entity = new SmtpSettings
            {
                Host = req.Host,
                Port = req.Port,
                UseSsl = req.UseSsl,
                UserName = req.UserName,
                PasswordEnc = req.Password ?? string.Empty,
                FromName = req.FromName,
                FromEmail = req.FromEmail,
                TimeoutMs = req.TimeoutMs,
                RetryCount = req.RetryCount
            };
            await _settingsService.UpsertAsync(entity, User?.Identity?.Name);
            return NoContent();
        }

        public class TestSendRequest
        {
            public required string To { get; set; }
            public string? TemplateKey { get; set; } // Optional: ContactForm, EmployerForm, EmployeeForm
            public string Subject { get; set; } = "Test Email";
            public string Body { get; set; } = "This is a test email.";
        }

        [HttpPost("test-send")]
        public async Task<ActionResult> TestSend([FromBody] TestSendRequest req)
        {
            string subject = req.Subject;
            string? bodyHtml = null;
            string bodyText = req.Body;

            // If template key provided, use template
            if (!string.IsNullOrEmpty(req.TemplateKey))
            {
                var template = await _templateService.GetByKeyAsync(req.TemplateKey);
                if (template != null && template.IsActive)
                {
                    // Create sample data for testing
                    var sampleData = CreateSampleDataForTemplate(req.TemplateKey);
                    
                    // Replace placeholders
                    subject = ReplacePlaceholders(template.Subject, sampleData);
                    bodyHtml = ReplacePlaceholders(template.BodyHtml, sampleData);
                }
            }

            await _emailSender.SendAsync(new EmailMessage
            {
                FromEmail = string.Empty, // will fallback to settings
                To = new List<string> { req.To },
                Subject = subject,
                BodyHtml = bodyHtml,
                BodyText = bodyText
            });
            return Ok();
        }

        private object CreateSampleDataForTemplate(string templateKey)
        {
            return templateKey switch
            {
                "ContactForm" => new
                {
                    FirstName = "Max",
                    LastName = "Mustermann",
                    Email = "max.mustermann@example.com",
                    Phone = "+49 123 456789",
                    Age = 25,
                    Nationality = "Deutsch",
                    Education = "Bachelor",
                    FieldOfStudy = "Informatik",
                    WorkExperience = "2 Jahre",
                    GermanLevel = "C1",
                    EnglishLevel = "B2",
                    Interest = "Software Development",
                    PreferredCity = "Berlin",
                    Timeline = "2025",
                    Message = "Ich interessiere mich für eine Stelle als Software-Entwickler.",
                    PrivacyConsent = true,
                    Newsletter = false,
                    Language = "de"
                },
                "EmployerForm" => new
                {
                    CompanyName = "TechCorp GmbH",
                    ContactPerson = "Maria Schmidt",
                    Email = "maria.schmidt@techcorp.de",
                    Phone = "+49 30 123456",
                    Industry = "IT & Software",
                    CompanySize = "50-100",
                    Positions = "Software Developer, DevOps Engineer",
                    Requirements = "Bachelor in Informatik\n3+ Jahre Erfahrung\nGute Deutschkenntnisse",
                    Message = "Wir suchen qualifizierte Entwickler für unser Team.",
                    SpecialRequests = "Vollzeit, Remote möglich",
                    Language = "de"
                },
                "EmployeeForm" => new
                {
                    Salutation = "Herr",
                    FullName = "Ahmet Yılmaz",
                    Email = "ahmet.yilmaz@example.com",
                    Phone = "+49 176 1234567",
                    Profession = "Software Developer",
                    Experience = 5,
                    Education = "Bachelor",
                    GermanLevel = "B2",
                    AdditionalInfo = "Erfahren in .NET und React",
                    SpecialRequests = "Vollzeit bevorzugt",
                    Language = "de"
                },
                "ClientCode" => new
                {
                    FullName = "Max Mustermann",
                    ClientCode = "CL-20250115-12345",
                    ExpirationDate = DateTime.UtcNow.AddDays(3).ToString("dd.MM.yyyy HH:mm"),
                    RegisterUrl = "https://worklines.de/register?code=CL-20250115-12345"
                },
                _ => new { }
            };
        }

        private string ReplacePlaceholders(string template, object data)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            var result = template;
            var properties = data.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(data);
                var placeholder = $"{{{{{prop.Name}}}}}";
                
                if (value != null)
                {
                    var stringValue = value.ToString() ?? string.Empty;
                    
                    // HTML encode strings but keep line breaks
                    if (prop.PropertyType == typeof(string))
                    {
                        stringValue = System.Net.WebUtility.HtmlEncode(stringValue);
                        stringValue = stringValue.Replace("\n", "<br>");
                    }
                    
                    result = result.Replace(placeholder, stringValue);
                }
                else
                {
                    result = result.Replace(placeholder, string.Empty);
                }
            }

            return result;
        }

        [HttpGet("health")]
        public async Task<ActionResult> HealthCheck()
        {
            var (isHealthy, message) = await _healthCheck.CheckAsync();
            if (!isHealthy) return StatusCode(503, new { healthy = false, message });
            return Ok(new { healthy = true, message });
        }

        /// <summary>
        /// Get email logs with filtering and pagination
        /// </summary>
        [HttpGet("logs")]
        public async Task<ActionResult> GetEmailLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? status = null)
        {
            try
            {
                var query = _db.EmailLogs.AsQueryable();

                // Filter by status if provided
                if (status.HasValue && status.Value >= 0)
                {
                    query = query.Where(e => e.Status == status.Value);
                }

                // Order by creation date descending
                query = query.OrderByDescending(e => e.CreatedAt);

                // Pagination
                var skip = (page - 1) * pageSize;
                var logs = await query
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new { items = logs, page, pageSize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching email logs");
                return StatusCode(500, new { message = "Error fetching email logs" });
            }
        }

        /// <summary>
        /// Get email log by ID
        /// </summary>
        [HttpGet("logs/{id}")]
        public async Task<ActionResult> GetEmailLog(long id)
        {
            try
            {
                var log = await _db.EmailLogs.FindAsync(id);
                if (log == null)
                {
                    return NotFound();
                }
                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching email log {Id}", id);
                return StatusCode(500, new { message = "Error fetching email log" });
            }
        }
    }
}

