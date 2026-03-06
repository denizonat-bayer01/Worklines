using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormSubmissionsController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateService _templateService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<FormSubmissionsController> _logger;
        private readonly IWebHostEnvironment _env;

        public FormSubmissionsController(
            WixiDbContext db, 
            IEmailSender emailSender, 
            IEmailTemplateService templateService,
            IFileStorageService fileStorageService,
            IWebHostEnvironment env,
            ILogger<FormSubmissionsController> logger)
        {
            _db = db;
            _emailSender = emailSender;
            _templateService = templateService;
            _fileStorageService = fileStorageService;
            _env = env;
            _logger = logger;
        }

        // DTO Models
        public class ContactFormDto
        {
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public required string Email { get; set; }
            public required string Phone { get; set; }
            public int? Age { get; set; }
            public string? Nationality { get; set; }
            public string? Education { get; set; }
            public string? FieldOfStudy { get; set; }
            public string? WorkExperience { get; set; }
            public string? GermanLevel { get; set; }
            public string? EnglishLevel { get; set; }
            public string? Interest { get; set; }
            public string? PreferredCity { get; set; }
            public string? Timeline { get; set; }
            public string? Message { get; set; }
            public bool PrivacyConsent { get; set; }
            public bool Newsletter { get; set; }
            public string? Language { get; set; }
        }

        public class EmployerFormDto
        {
            public required string CompanyName { get; set; }
            public required string ContactPerson { get; set; }
            public required string Email { get; set; }
            public required string Phone { get; set; }
            public required string Industry { get; set; }
            public string? CompanySize { get; set; }
            public required string Positions { get; set; }
            public required string Requirements { get; set; }
            public string? Message { get; set; }
            public string? SpecialRequests { get; set; }
            public string? Language { get; set; }
        }

        public class EmployeeFormDto
        {
            public string? Salutation { get; set; }
            public required string FullName { get; set; }
            public required string Email { get; set; }
            public required string Phone { get; set; }
            public string? Profession { get; set; }
            public int? Experience { get; set; }
            public string? Education { get; set; }
            public string? GermanLevel { get; set; }
            public string? AdditionalInfo { get; set; }
            public string? SpecialRequests { get; set; }
            public string? Language { get; set; }
            // CV upload will be handled separately
        }

        /// <summary>
        /// Submit contact form (general inquiry/application)
        /// </summary>
        [HttpPost("contact")]
        public async Task<ActionResult> SubmitContactForm([FromBody] ContactFormDto dto)
        {
            try
            {
                // Capture request metadata
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // Save to database
                var submission = new ContactFormSubmission
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Age = dto.Age,
                    Nationality = dto.Nationality,
                    Education = dto.Education,
                    FieldOfStudy = dto.FieldOfStudy,
                    WorkExperience = dto.WorkExperience,
                    GermanLevel = dto.GermanLevel,
                    EnglishLevel = dto.EnglishLevel,
                    Interest = dto.Interest,
                    PreferredCity = dto.PreferredCity,
                    Timeline = dto.Timeline,
                    Message = dto.Message,
                    PrivacyConsent = dto.PrivacyConsent,
                    Newsletter = dto.Newsletter,
                    RequestIp = ip,
                    UserAgent = userAgent,
                    Language = dto.Language,
                    CreatedAt = DateTime.UtcNow
                };

                _db.ContactFormSubmissions.Add(submission);
                await _db.SaveChangesAsync();

                // Send email notification using template
                var template = await _templateService.GetByKeyAsync("ContactForm");
                string emailBody;
                string subject;
                
                if (template != null && template.IsActive)
                {
                    emailBody = ReplacePlaceholders(template.BodyHtml, dto);
                    subject = ReplacePlaceholders(template.Subject, dto);
                }
                else
                {
                    // Fallback to hardcoded template
                    emailBody = FormatContactFormEmail(dto);
                    subject = $"🆕 Neue Kontaktanfrage von {dto.FirstName} {dto.LastName}";
                }

                var emailMessage = new EmailMessage
                {
                    To = new List<string> { "info@worklines.de" },
                    Subject = subject,
                    BodyHtml = emailBody,
                    CorrelationId = Guid.NewGuid(),
                    Metadata = new Dictionary<string, string>
                    {
                        { "FormType", "Contact" },
                        { "SubmissionId", submission.Id.ToString() }
                    }
                };

                await _emailSender.SendAsync(emailMessage);

                // Update submission with email log ID if needed
                var emailLog = await _db.EmailLogs
                    .Where(e => e.CorrelationId == emailMessage.CorrelationId)
                    .OrderByDescending(e => e.CreatedAt)
                    .FirstOrDefaultAsync();
                if (emailLog != null)
                {
                    submission.EmailLogId = emailLog.Id;
                    await _db.SaveChangesAsync();
                }

                _logger.LogInformation("Contact form submitted successfully. ID: {Id}, Email: {Email}", submission.Id, dto.Email);

                return Ok(new { success = true, id = submission.Id, message = "Form successfully submitted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting contact form. Email: {Email}", dto.Email);
                return StatusCode(500, new { success = false, message = "An error occurred while submitting the form" });
            }
        }

        /// <summary>
        /// Submit employer form (companies looking for workers)
        /// </summary>
        [HttpPost("employer")]
        public async Task<ActionResult> SubmitEmployerForm([FromBody] EmployerFormDto dto)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var submission = new EmployerFormSubmission
                {
                    CompanyName = dto.CompanyName,
                    ContactPerson = dto.ContactPerson,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Industry = dto.Industry,
                    CompanySize = dto.CompanySize,
                    Positions = dto.Positions,
                    Requirements = dto.Requirements,
                    Message = dto.Message,
                    SpecialRequests = dto.SpecialRequests,
                    RequestIp = ip,
                    UserAgent = userAgent,
                    Language = dto.Language,
                    CreatedAt = DateTime.UtcNow
                };

                _db.EmployerFormSubmissions.Add(submission);
                await _db.SaveChangesAsync();

                // Send email notification using template
                var template = await _templateService.GetByKeyAsync("EmployerForm");
                string emailBody;
                string subject;
                
                if (template != null && template.IsActive)
                {
                    emailBody = ReplacePlaceholders(template.BodyHtml, dto);
                    subject = ReplacePlaceholders(template.Subject, dto);
                }
                else
                {
                    // Fallback to hardcoded template
                    emailBody = FormatEmployerFormEmail(dto);
                    subject = $"🏢 Neue Arbeitgeberanfrage von {dto.CompanyName}";
                }

                var emailMessage = new EmailMessage
                {
                    To = new List<string> { "info@worklines.de" },
                    Subject = subject,
                    BodyHtml = emailBody,
                    CorrelationId = Guid.NewGuid(),
                    Metadata = new Dictionary<string, string>
                    {
                        { "FormType", "Employer" },
                        { "SubmissionId", submission.Id.ToString() }
                    }
                };

                await _emailSender.SendAsync(emailMessage);

                var emailLog = await _db.EmailLogs
                    .Where(e => e.CorrelationId == emailMessage.CorrelationId)
                    .OrderByDescending(e => e.CreatedAt)
                    .FirstOrDefaultAsync();
                if (emailLog != null)
                {
                    submission.EmailLogId = emailLog.Id;
                    await _db.SaveChangesAsync();
                }

                _logger.LogInformation("Employer form submitted successfully. ID: {Id}, Company: {Company}", submission.Id, dto.CompanyName);

                return Ok(new { success = true, id = submission.Id, message = "Form successfully submitted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting employer form. Company: {Company}", dto.CompanyName);
                return StatusCode(500, new { success = false, message = "An error occurred while submitting the form" });
            }
        }

        /// <summary>
        /// Submit employee form (candidates looking for work) with CV upload
        /// </summary>
        [HttpPost("employee")]
        public async Task<ActionResult> SubmitEmployeeForm(
            [FromForm] string? salutation,
            [FromForm] string fullName,
            [FromForm] string email,
            [FromForm] string phone,
            [FromForm] string? profession,
            [FromForm] int? experience,
            [FromForm] string? education,
            [FromForm] string? germanLevel,
            [FromForm] string? additionalInfo,
            [FromForm] string? specialRequests,
            [FromForm] string? language,
            [FromForm] IFormFile? cvFile)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                // Create DTO for email template
                var dto = new EmployeeFormDto
                {
                    Salutation = salutation,
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Profession = profession,
                    Experience = experience,
                    Education = education,
                    GermanLevel = germanLevel,
                    AdditionalInfo = additionalInfo,
                    SpecialRequests = specialRequests,
                    Language = language
                };

                string? cvFilePath = null;
                string? cvFileName = null;
                long? cvFileSize = null;

                // Handle CV upload if provided
                if (cvFile != null && cvFile.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                    var fileExtension = System.IO.Path.GetExtension(cvFile.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { success = false, message = "CV dosyası sadece PDF, DOC veya DOCX formatında olabilir." });
                    }

                    // Validate file size (max 10MB)
                    const long maxFileSize = 10 * 1024 * 1024; // 10MB
                    if (cvFile.Length > maxFileSize)
                    {
                        return BadRequest(new { success = false, message = "CV dosyası en fazla 10MB olabilir." });
                    }

                    // Upload CV file
                    var uploadResult = await _fileStorageService.UploadFileAsync(cvFile, "employee-cvs");
                    
                    if (!uploadResult.Success)
                    {
                        _logger.LogError("CV upload failed: {Error}", uploadResult.ErrorMessage);
                        return StatusCode(500, new { success = false, message = "CV yüklenirken hata oluştu: " + uploadResult.ErrorMessage });
                    }

                    cvFilePath = uploadResult.FilePath;
                    cvFileName = uploadResult.FileName;
                    cvFileSize = uploadResult.FileSizeBytes;

                    _logger.LogInformation("CV uploaded successfully: {FileName} -> {FilePath}", cvFileName, cvFilePath);
                }

                var submission = new EmployeeFormSubmission
                {
                    Salutation = dto.Salutation,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Profession = dto.Profession,
                    Experience = dto.Experience,
                    Education = dto.Education,
                    GermanLevel = dto.GermanLevel,
                    AdditionalInfo = dto.AdditionalInfo,
                    SpecialRequests = dto.SpecialRequests,
                    CvFileName = cvFileName,
                    CvFilePath = cvFilePath,
                    CvFileSize = cvFileSize,
                    RequestIp = ip,
                    UserAgent = userAgent,
                    Language = dto.Language,
                    CreatedAt = DateTime.UtcNow
                };

                _db.EmployeeFormSubmissions.Add(submission);
                await _db.SaveChangesAsync();

                // Send email notification using template
                var template = await _templateService.GetByKeyAsync("EmployeeForm");
                string emailBody;
                string subject;
                
                if (template != null && template.IsActive)
                {
                    emailBody = ReplacePlaceholders(template.BodyHtml, dto);
                    subject = ReplacePlaceholders(template.Subject, dto);
                }
                else
                {
                    // Fallback to hardcoded template
                    emailBody = FormatEmployeeFormEmail(dto);
                    subject = $"👤 Neue Arbeitnehmerbewerbung von {dto.FullName}";
                }

                // Add CV attachment if available
                var attachments = new List<EmailAttachment>();
                if (!string.IsNullOrEmpty(cvFilePath) && !string.IsNullOrEmpty(cvFileName))
                {
                    var fullCvPath = Path.Combine(_env.WebRootPath, cvFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullCvPath))
                    {
                        attachments.Add(new EmailAttachment
                        {
                            FilePath = fullCvPath,
                            FileName = cvFileName,
                            ContentType = cvFile?.ContentType ?? "application/pdf"
                        });
                        _logger.LogInformation("CV attachment added to email: {FileName}", cvFileName);
                    }
                    else
                    {
                        _logger.LogWarning("CV file not found for email attachment: {FilePath}", fullCvPath);
                    }
                }

                var emailMessage = new EmailMessage
                {
                    To = new List<string> { "info@worklines.de" },
                    Subject = subject,
                    BodyHtml = emailBody,
                    CorrelationId = Guid.NewGuid(),
                    Attachments = attachments.Any() ? attachments : null,
                    Metadata = new Dictionary<string, string>
                    {
                        { "FormType", "Employee" },
                        { "SubmissionId", submission.Id.ToString() },
                        { "HasCv", cvFilePath != null ? "true" : "false" }
                    }
                };

                await _emailSender.SendAsync(emailMessage);

                var emailLog = await _db.EmailLogs
                    .Where(e => e.CorrelationId == emailMessage.CorrelationId)
                    .OrderByDescending(e => e.CreatedAt)
                    .FirstOrDefaultAsync();
                if (emailLog != null)
                {
                    submission.EmailLogId = emailLog.Id;
                    await _db.SaveChangesAsync();
                }

                _logger.LogInformation("Employee form submitted successfully. ID: {Id}, Name: {Name}", submission.Id, dto.FullName);

                return Ok(new { success = true, id = submission.Id, message = "Form successfully submitted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting employee form. Name: {Name}", fullName);
                return StatusCode(500, new { success = false, message = "An error occurred while submitting the form" });
            }
        }

        /// <summary>
        /// List employer form submissions (admin)
        /// </summary>
        [HttpGet("employers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetEmployerSubmissions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? industry = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var query = _db.EmployerFormSubmissions.AsNoTracking().OrderByDescending(e => e.CreatedAt).AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.ToLower();
                    query = query.Where(e =>
                        e.CompanyName.ToLower().Contains(s) ||
                        e.ContactPerson.ToLower().Contains(s) ||
                        e.Email.ToLower().Contains(s) ||
                        e.Phone.ToLower().Contains(s));
                }

                if (!string.IsNullOrWhiteSpace(industry))
                {
                    var i = industry.ToLower();
                    query = query.Where(e => e.Industry.ToLower().Contains(i));
                }

                var total = await query.CountAsync();
                var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                return Ok(new { success = true, total, page, pageSize, items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employer submissions");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching employer submissions" });
            }
        }

        /// <summary>
        /// List employee form submissions (admin) - Legacy endpoint for backward compatibility
        /// </summary>
        [HttpGet("employees")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetEmployeeSubmissions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? profession = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var query = _db.EmployeeFormSubmissions.AsNoTracking().OrderByDescending(e => e.CreatedAt).AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.ToLower();
                    query = query.Where(e =>
                        e.FullName.ToLower().Contains(s) ||
                        e.Email.ToLower().Contains(s) ||
                        e.Phone.ToLower().Contains(s));
                }

                if (!string.IsNullOrWhiteSpace(profession))
                {
                    var p = profession.ToLower();
                    query = query.Where(e => e.Profession != null && e.Profession.ToLower().Contains(p));
                }

                var total = await query.CountAsync();
                var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                return Ok(new { success = true, total, page, pageSize, items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee submissions");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching employee submissions" });
            }
        }

        // Placeholder replacement helper
        private string ReplacePlaceholders(string template, object dto)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            var result = template;
            var properties = dto.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(dto);
                var placeholder = $"{{{{{prop.Name}}}}}";
                
                if (value != null)
                {
                    // Handle nullable values
                    var stringValue = value.ToString() ?? string.Empty;
                    
                    // Escape HTML for safe rendering (except in HTML context, but keep line breaks)
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

        // Email Formatting Methods (kept as fallback)
        private string FormatContactFormEmail(ContactFormDto dto)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2563eb; color: white; padding: 20px; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9fafb; padding: 20px; }}
        .section {{ background-color: white; margin: 15px 0; padding: 15px; border-radius: 6px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }}
        .section-title {{ color: #2563eb; font-weight: bold; margin-bottom: 10px; font-size: 16px; }}
        .field {{ margin: 8px 0; }}
        .label {{ font-weight: 600; color: #4b5563; }}
        .value {{ color: #1f2937; }}
        .footer {{ text-align: center; padding: 20px; color: #6b7280; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>🆕 Neue Kontaktanfrage</h2>
        </div>
        <div class=""content"">
            <div class=""section"">
                <div class=""section-title"">👤 Persönliche Informationen</div>
                <div class=""field""><span class=""label"">Name:</span> <span class=""value"">{dto.FirstName} {dto.LastName}</span></div>
                <div class=""field""><span class=""label"">E-Mail:</span> <span class=""value"">{dto.Email}</span></div>
                <div class=""field""><span class=""label"">Telefon:</span> <span class=""value"">{dto.Phone}</span></div>
                {(dto.Age.HasValue ? $@"<div class=""field""><span class=""label"">Alter:</span> <span class=""value"">{dto.Age}</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.Nationality) ? $@"<div class=""field""><span class=""label"">Nationalität:</span> <span class=""value"">{dto.Nationality}</span></div>" : "")}
            </div>

            {(!string.IsNullOrEmpty(dto.Education) || !string.IsNullOrEmpty(dto.FieldOfStudy) || !string.IsNullOrEmpty(dto.WorkExperience) ? $@"
            <div class=""section"">
                <div class=""section-title"">🎓 Bildung & Erfahrung</div>
                {(!string.IsNullOrEmpty(dto.Education) ? $@"<div class=""field""><span class=""label"">Bildung:</span> <span class=""value"">{dto.Education}</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.FieldOfStudy) ? $@"<div class=""field""><span class=""label"">Studienrichtung:</span> <span class=""value"">{dto.FieldOfStudy}</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.WorkExperience) ? $@"<div class=""field""><span class=""label"">Berufserfahrung:</span> <span class=""value"">{dto.WorkExperience}</span></div>" : "")}
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.GermanLevel) || !string.IsNullOrEmpty(dto.EnglishLevel) ? $@"
            <div class=""section"">
                <div class=""section-title"">🌐 Sprachkenntnisse</div>
                {(!string.IsNullOrEmpty(dto.GermanLevel) ? $@"<div class=""field""><span class=""label"">Deutsch:</span> <span class=""value"">{dto.GermanLevel}</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.EnglishLevel) ? $@"<div class=""field""><span class=""label"">Englisch:</span> <span class=""value"">{dto.EnglishLevel}</span></div>" : "")}
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.Interest) || !string.IsNullOrEmpty(dto.PreferredCity) || !string.IsNullOrEmpty(dto.Timeline) ? $@"
            <div class=""section"">
                <div class=""section-title"">💼 Interesse & Präferenzen</div>
                {(!string.IsNullOrEmpty(dto.Interest) ? $@"<div class=""field""><span class=""label"">Interesse:</span> <span class=""value"">{dto.Interest}</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.PreferredCity) ? $@"<div class=""field""><span class=""label"">Bevorzugte Stadt:</span> <span class=""value"">{dto.PreferredCity}</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.Timeline) ? $@"<div class=""field""><span class=""label"">Zeitplan:</span> <span class=""value"">{dto.Timeline}</span></div>" : "")}
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.Message) ? $@"
            <div class=""section"">
                <div class=""section-title"">💬 Nachricht</div>
                <div class=""field""><span class=""value"">{dto.Message?.Replace("\n", "<br>")}</span></div>
            </div>" : "")}

            <div class=""section"">
                <div class=""section-title"">ℹ️ Zusätzliche Informationen</div>
                <div class=""field""><span class=""label"">Datenschutz akzeptiert:</span> <span class=""value"">{(dto.PrivacyConsent ? "✅ Ja" : "❌ Nein")}</span></div>
                <div class=""field""><span class=""label"">Newsletter:</span> <span class=""value"">{(dto.Newsletter ? "✅ Ja" : "❌ Nein")}</span></div>
                {(!string.IsNullOrEmpty(dto.Language) ? $@"<div class=""field""><span class=""label"">Sprache:</span> <span class=""value"">{dto.Language}</span></div>" : "")}
            </div>
        </div>
        <div class=""footer"">
            <p>Worklines - Ihre Brücke zur Karriere in Deutschland</p>
            <p>Automatisch generierte E-Mail vom Kontaktformular</p>
        </div>
    </div>
</body>
</html>
";
        }

        private string FormatEmployerFormEmail(EmployerFormDto dto)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #059669; color: white; padding: 20px; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9fafb; padding: 20px; }}
        .section {{ background-color: white; margin: 15px 0; padding: 15px; border-radius: 6px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }}
        .section-title {{ color: #059669; font-weight: bold; margin-bottom: 10px; font-size: 16px; }}
        .field {{ margin: 8px 0; }}
        .label {{ font-weight: 600; color: #4b5563; }}
        .value {{ color: #1f2937; }}
        .footer {{ text-align: center; padding: 20px; color: #6b7280; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>🏢 Neue Arbeitgeberanfrage</h2>
        </div>
        <div class=""content"">
            <div class=""section"">
                <div class=""section-title"">🏢 Unternehmensinformationen</div>
                <div class=""field""><span class=""label"">Firmenname:</span> <span class=""value"">{dto.CompanyName}</span></div>
                <div class=""field""><span class=""label"">Ansprechpartner:</span> <span class=""value"">{dto.ContactPerson}</span></div>
                <div class=""field""><span class=""label"">E-Mail:</span> <span class=""value"">{dto.Email}</span></div>
                <div class=""field""><span class=""label"">Telefon:</span> <span class=""value"">{dto.Phone}</span></div>
                <div class=""field""><span class=""label"">Branche:</span> <span class=""value"">{dto.Industry}</span></div>
                {(!string.IsNullOrEmpty(dto.CompanySize) ? $@"<div class=""field""><span class=""label"">Unternehmensgröße:</span> <span class=""value"">{dto.CompanySize}</span></div>" : "")}
            </div>

            <div class=""section"">
                <div class=""section-title"">👥 Stellenanforderungen</div>
                <div class=""field""><span class=""label"">Gesuchte Positionen:</span> <span class=""value"">{dto.Positions}</span></div>
                <div class=""field""><span class=""label"">Anforderungen:</span><br><span class=""value"">{dto.Requirements.Replace("\n", "<br>")}</span></div>
            </div>

            {(!string.IsNullOrEmpty(dto.Message) ? $@"
            <div class=""section"">
                <div class=""section-title"">💬 Nachricht</div>
                <div class=""field""><span class=""value"">{dto.Message?.Replace("\n", "<br>")}</span></div>
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.SpecialRequests) ? $@"
            <div class=""section"">
                <div class=""section-title"">⭐ Besondere Wünsche</div>
                <div class=""field""><span class=""value"">{dto.SpecialRequests?.Replace("\n", "<br>")}</span></div>
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.Language) ? $@"
            <div class=""section"">
                <div class=""section-title"">ℹ️ Zusätzliche Informationen</div>
                <div class=""field""><span class=""label"">Sprache:</span> <span class=""value"">{dto.Language}</span></div>
            </div>" : "")}
        </div>
        <div class=""footer"">
            <p>Worklines - Ihre Brücke zur Karriere in Deutschland</p>
            <p>Automatisch generierte E-Mail vom Arbeitgeberformular</p>
        </div>
    </div>
</body>
</html>
";
        }

        private string FormatEmployeeFormEmail(EmployeeFormDto dto)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #dc2626; color: white; padding: 20px; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9fafb; padding: 20px; }}
        .section {{ background-color: white; margin: 15px 0; padding: 15px; border-radius: 6px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }}
        .section-title {{ color: #dc2626; font-weight: bold; margin-bottom: 10px; font-size: 16px; }}
        .field {{ margin: 8px 0; }}
        .label {{ font-weight: 600; color: #4b5563; }}
        .value {{ color: #1f2937; }}
        .footer {{ text-align: center; padding: 20px; color: #6b7280; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>👤 Neue Arbeitnehmerbewerbung</h2>
        </div>
        <div class=""content"">
            <div class=""section"">
                <div class=""section-title"">👤 Persönliche Informationen</div>
                {(!string.IsNullOrEmpty(dto.Salutation) ? $@"<div class=""field""><span class=""label"">Anrede:</span> <span class=""value"">{dto.Salutation}</span></div>" : "")}
                <div class=""field""><span class=""label"">Name:</span> <span class=""value"">{dto.FullName}</span></div>
                <div class=""field""><span class=""label"">E-Mail:</span> <span class=""value"">{dto.Email}</span></div>
                <div class=""field""><span class=""label"">Telefon:</span> <span class=""value"">{dto.Phone}</span></div>
            </div>

            {(!string.IsNullOrEmpty(dto.Profession) || dto.Experience.HasValue || !string.IsNullOrEmpty(dto.Education) || !string.IsNullOrEmpty(dto.GermanLevel) ? $@"
            <div class=""section"">
                <div class=""section-title"">🎓 Qualifikationen</div>
                {(!string.IsNullOrEmpty(dto.Profession) ? $@"<div class=""field""><span class=""label"">Beruf:</span> <span class=""value"">{dto.Profession}</span></div>" : "")}
                {(dto.Experience.HasValue ? $@"<div class=""field""><span class=""label"">Berufserfahrung:</span> <span class=""value"">{dto.Experience} Jahre</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.Education) ? $@"<div class=""field""><span class=""label"">Bildung:</span> <span class=""value"">{dto.Education}</span></div>" : "")}
                {(!string.IsNullOrEmpty(dto.GermanLevel) ? $@"<div class=""field""><span class=""label"">Deutschkenntnisse:</span> <span class=""value"">{dto.GermanLevel}</span></div>" : "")}
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.AdditionalInfo) ? $@"
            <div class=""section"">
                <div class=""section-title"">💬 Zusätzliche Informationen</div>
                <div class=""field""><span class=""value"">{dto.AdditionalInfo?.Replace("\n", "<br>")}</span></div>
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.SpecialRequests) ? $@"
            <div class=""section"">
                <div class=""section-title"">⭐ Besondere Wünsche</div>
                <div class=""field""><span class=""value"">{dto.SpecialRequests?.Replace("\n", "<br>")}</span></div>
            </div>" : "")}

            {(!string.IsNullOrEmpty(dto.Language) ? $@"
            <div class=""section"">
                <div class=""section-title"">ℹ️ Zusätzliche Informationen</div>
                <div class=""field""><span class=""label"">Sprache:</span> <span class=""value"">{dto.Language}</span></div>
            </div>" : "")}
        </div>
        <div class=""footer"">
            <p>Worklines - Ihre Brücke zur Karriere in Deutschland</p>
            <p>Automatisch generierte E-Mail vom Arbeitnehmerformular</p>
        </div>
    </div>
</body>
</html>
";
        }
    }
}

