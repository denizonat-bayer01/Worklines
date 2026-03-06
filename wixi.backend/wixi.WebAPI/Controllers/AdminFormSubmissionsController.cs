using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Business.Abstract;
using wixi.Entities.DTOs;
using wixi.Entities.Concrete.Identity;
using wixi.Entities.Concrete.Client;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/form-submissions")]
    [Authorize(Roles = "Admin")]
    public class AdminFormSubmissionsController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly ILogger<AdminFormSubmissionsController> _logger;
        private readonly IClientService _clientService;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IWebHostEnvironment _env;

        public AdminFormSubmissionsController(
            WixiDbContext db, 
            ILogger<AdminFormSubmissionsController> logger,
            IClientService clientService,
            IEmailSender emailSender,
            UserManager<AppUser> userManager,
            IEmailTemplateService emailTemplateService,
            IWebHostEnvironment env)
        {
            _db = db;
            _logger = logger;
            _clientService = clientService;
            _emailSender = emailSender;
            _userManager = userManager;
            _emailTemplateService = emailTemplateService;
            _env = env;
        }

        /// <summary>
        /// List employer form submissions (admin)
        /// </summary>
        [HttpGet("employers")]
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
        /// Get employer form submission by ID (admin)
        /// </summary>
        [HttpGet("employers/{id}")]
        public async Task<ActionResult> GetEmployerSubmissionById(long id)
        {
            try
            {
                var submission = await _db.EmployerFormSubmissions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (submission == null)
                {
                    return NotFound(new { success = false, message = "Employer submission not found" });
                }

                return Ok(new { success = true, item = submission });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employer submission by ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching employer submission" });
            }
        }

        /// <summary>
        /// List employee form submissions (admin)
        /// </summary>
        [HttpGet("employees")]
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

                var query = _db.EmployeeFormSubmissions
                    .AsNoTracking()
                    .OrderByDescending(e => e.CreatedAt)
                    .Select(e => new
                    {
                        e.Id,
                        e.CreatedAt,
                        e.Salutation,
                        e.FullName,
                        e.Email,
                        e.Phone,
                        e.Profession,
                        e.Experience,
                        e.Education,
                        e.GermanLevel,
                        e.AdditionalInfo,
                        e.SpecialRequests,
                        e.Language,
                        ClientCode = _db.Clients
                            .Where(c => c.Email == e.Email && c.DeletedAt == null)
                            .Select(c => c.ClientCode)
                            .FirstOrDefault(),
                        ClientCreatedAt = _db.Clients
                            .Where(c => c.Email == e.Email && c.DeletedAt == null)
                            .Select(c => c.CreatedAt)
                            .FirstOrDefault()
                    })
                    .AsQueryable();

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

        /// <summary>
        /// Get employee form submission by ID (admin)
        /// </summary>
        [HttpGet("employees/{id}")]
        public async Task<ActionResult> GetEmployeeSubmissionById(long id)
        {
            try
            {
                var submission = await _db.EmployeeFormSubmissions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (submission == null)
                {
                    return NotFound(new { success = false, message = "Employee submission not found" });
                }

                return Ok(new { success = true, item = submission });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee submission by ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching employee submission" });
            }
        }

        /// <summary>
        /// Get all pending client codes (admin)
        /// </summary>
        [HttpGet("pending-client-codes")]
        public async Task<ActionResult> GetPendingClientCodes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null) // "all", "valid", "used", "expired"
        {
            try
            {
                var query = _db.PendingClientCodes.AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    query = query.Where(p => 
                        p.ClientCode.ToLower().Contains(search) ||
                        p.Email.ToLower().Contains(search) ||
                        p.FullName.ToLower().Contains(search));
                }

                // Status filter
                if (!string.IsNullOrWhiteSpace(status) && status != "all")
                {
                    var now = DateTime.UtcNow;
                    switch (status.ToLower())
                    {
                        case "valid":
                            query = query.Where(p => !p.IsUsed && p.ExpirationDate > now);
                            break;
                        case "used":
                            query = query.Where(p => p.IsUsed);
                            break;
                        case "expired":
                            query = query.Where(p => !p.IsUsed && p.ExpirationDate <= now);
                            break;
                    }
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PendingClientCodeResponseDto
                    {
                        Id = p.Id,
                        ClientCode = p.ClientCode,
                        Email = p.Email,
                        FullName = p.FullName,
                        ExpirationDate = p.ExpirationDate,
                        IsUsed = p.IsUsed,
                        UsedAt = p.UsedAt,
                        EmployeeSubmissionId = p.EmployeeSubmissionId,
                        CreatedAt = p.CreatedAt,
                        IsExpired = p.ExpirationDate <= DateTime.UtcNow,
                        IsValid = !p.IsUsed && p.ExpirationDate > DateTime.UtcNow,
                        DaysRemaining = p.ExpirationDate > DateTime.UtcNow 
                            ? (int?)(p.ExpirationDate - DateTime.UtcNow).TotalDays 
                            : (int?)(DateTime.UtcNow - p.ExpirationDate).TotalDays * -1,
                        Status = p.IsUsed ? "Used" : (p.ExpirationDate <= DateTime.UtcNow ? "Expired" : "Valid")
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items,
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending client codes");
                return StatusCode(500, new { success = false, message = "Error fetching pending client codes: " + ex.Message });
            }
        }

        /// <summary>
        /// Download CV file for employee submission (admin)
        /// </summary>
        [HttpGet("employees/{id}/cv")]
        public async Task<ActionResult> DownloadEmployeeCv(long id)
        {
            try
            {
                var submission = await _db.EmployeeFormSubmissions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (submission == null)
                {
                    return NotFound(new { success = false, message = "Employee submission not found" });
                }

                if (string.IsNullOrEmpty(submission.CvFilePath) || string.IsNullOrEmpty(submission.CvFileName))
                {
                    return NotFound(new { success = false, message = "CV file not found for this submission" });
                }

                // Get file path
                var fullPath = Path.Combine(_env.WebRootPath, submission.CvFilePath.TrimStart('/', '\\'));

                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogWarning("CV file not found on disk: {FilePath}", fullPath);
                    return NotFound(new { success = false, message = "CV file not found on server" });
                }

                // Determine content type based on file extension
                var extension = Path.GetExtension(submission.CvFileName).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream"
                };

                var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                return File(fileBytes, contentType, submission.CvFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading CV for employee submission: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while downloading CV" });
            }
        }

        /// <summary>
        /// Create client code from employee submission (only code, no client/user creation)
        /// </summary>
        [HttpPost("employees/{id}/create-client")]
        public async Task<ActionResult> CreateClientFromEmployeeSubmission(long id, [FromBody] object? request = null)
        {
            try
            {
                var submission = await _db.EmployeeFormSubmissions
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (submission == null)
                {
                    return NotFound(new { success = false, message = "Employee submission not found" });
                }

                // Check if client already exists for this email
                var existingClient = await _db.Clients
                    .FirstOrDefaultAsync(c => c.Email == submission.Email && c.DeletedAt == null);

                if (existingClient != null)
                {
                    return BadRequest(new { success = false, message = "Client already exists for this email", clientCode = existingClient.ClientCode });
                }

                // Check if there's already a pending code for this email that is still valid
                var existingPendingCode = await _db.PendingClientCodes
                    .FirstOrDefaultAsync(p => p.Email == submission.Email && !p.IsUsed && p.ExpirationDate > DateTime.UtcNow);

                if (existingPendingCode != null)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "A valid client code already exists for this email", 
                        clientCode = existingPendingCode.ClientCode,
                        expirationDate = existingPendingCode.ExpirationDate
                    });
                }

                // Generate unique client code
                string clientCode;
                bool isUnique = false;
                int attempts = 0;
                const int maxAttempts = 10;

                do
                {
                    clientCode = $"CL-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(10000, 99999)}";
                    var codeExists = await _db.PendingClientCodes.AnyAsync(p => p.ClientCode == clientCode) ||
                                     await _db.Clients.AnyAsync(c => c.ClientCode == clientCode);
                    isUnique = !codeExists;
                    attempts++;
                } while (!isUnique && attempts < maxAttempts);

                if (!isUnique)
                {
                    _logger.LogError("Failed to generate unique client code after {Attempts} attempts", attempts);
                    return StatusCode(500, new { success = false, message = "Failed to generate unique client code" });
                }

                // Set expiration date (3 days from now)
                var expirationDate = DateTime.UtcNow.AddDays(3);

                // Create pending client code record
                var pendingCode = new PendingClientCode
                {
                    ClientCode = clientCode,
                    Email = submission.Email,
                    FullName = submission.FullName,
                    ExpirationDate = expirationDate,
                    EmployeeSubmissionId = id,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _db.PendingClientCodes.Add(pendingCode);
                await _db.SaveChangesAsync();

                // Send email to client using template
                var baseUrl = "https://worklines.de";
                var registerUrl = $"{baseUrl}/register?code={Uri.EscapeDataString(clientCode)}";
                
                // Get email template
                var template = await _emailTemplateService.GetByKeyAsync("ClientCode");
                string emailSubject;
                string emailBody;
                
                if (template != null && template.IsActive)
                {
                    // Prepare data for template
                    var templateData = new
                    {
                        FullName = submission.FullName,
                        ClientCode = clientCode,
                        ExpirationDate = expirationDate.ToString("dd.MM.yyyy HH:mm"),
                        RegisterUrl = registerUrl
                    };
                    
                    emailSubject = ReplacePlaceholders(template.Subject, templateData);
                    emailBody = ReplacePlaceholders(template.BodyHtml, templateData);
                }
                else
                {
                    // Fallback to hardcoded template
                    emailSubject = $"Willkommen bei Worklines - Ihr Kundenkode: {clientCode}";
                    emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2563eb; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9fafb; padding: 20px; border: 1px solid #e5e7eb; }}
        .code-box {{ background-color: white; border: 2px solid #2563eb; padding: 15px; margin: 20px 0; text-align: center; border-radius: 5px; }}
        .code {{ font-size: 24px; font-weight: bold; color: #2563eb; letter-spacing: 2px; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #e5e7eb; font-size: 12px; color: #6b7280; }}
        .warning {{ background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 10px; margin: 15px 0; }}
        .button {{ background-color: #2563eb; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 10px 0; }}
        .info {{ background-color: #dbeafe; border-left: 4px solid #2563eb; padding: 10px; margin: 15px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Willkommen bei Worklines</h1>
        </div>
        <div class='content'>
            <p>Sehr geehrte/r {submission.FullName},</p>
            
            <p>Vielen Dank für Ihr Interesse an unseren Dienstleistungen. Wir haben Ihnen einen persönlichen Kundenkode erstellt.</p>
            
            <p><strong>Ihr Kundenkode:</strong></p>
            <div class='code-box'>
                <div class='code'>{clientCode}</div>
            </div>
            
            <div class='warning'>
                <strong>⚠️ Wichtig:</strong> Dieser Kode ist <strong>3 Tage gültig</strong> (bis {expirationDate:dd.MM.yyyy HH:mm} Uhr).
                Bitte verwenden Sie ihn innerhalb dieser Zeit, um Ihr Konto zu registrieren.
            </div>
            
            <div class='info'>
                <strong>📋 So geht's weiter:</strong>
                <ol style='margin: 10px 0; padding-left: 20px;'>
                    <li>Klicken Sie auf den untenstehenden Button, um zur Registrierung zu gelangen</li>
                    <li>Geben Sie Ihren Kundenkode bei der Registrierung ein</li>
                    <li>Vervollständigen Sie Ihr Profil und laden Sie die erforderlichen Dokumente hoch</li>
                </ol>
            </div>
            
            <p style='text-align: center;'>
                <a href='{registerUrl}' class='button'>Jetzt registrieren</a>
            </p>
            
            <p style='font-size: 12px; color: #6b7280; text-align: center;'>
                Oder besuchen Sie: <a href='{registerUrl}'>{registerUrl}</a>
            </p>
            
            <p>Mit diesem Kode können Sie sich in unserem System registrieren und Ihre Bewerbung fortsetzen.</p>
            
            <p>Bei Fragen stehen wir Ihnen gerne zur Verfügung.</p>
            
            <p>Mit freundlichen Grüßen,<br>Das Worklines Team</p>
        </div>
        <div class='footer'>
            <p>Worklines - Ihr Partner für Bildung und Karriere in Deutschland</p>
            <p>Diese E-Mail wurde automatisch generiert. Bitte antworten Sie nicht direkt auf diese E-Mail.</p>
        </div>
    </div>
</body>
</html>";
                }

                var emailMessage = new EmailMessage
                {
                    To = new List<string> { submission.Email },
                    Subject = emailSubject,
                    BodyHtml = emailBody,
                    CorrelationId = Guid.NewGuid(),
                    Metadata = new Dictionary<string, string>
                    {
                        { "Type", "ClientCode" },
                        { "SubmissionId", submission.Id.ToString() },
                        { "PendingCodeId", pendingCode.Id.ToString() },
                        { "ClientCode", clientCode },
                        { "ExpirationDate", expirationDate.ToString("O") }
                    }
                };

                await _emailSender.SendAsync(emailMessage);

                _logger.LogInformation("Client code created from employee submission: {SubmissionId} -> Code: {Code}, Expires: {ExpirationDate}", 
                    id, clientCode, expirationDate);

                return Ok(new { 
                    success = true, 
                    message = "Client code created successfully and email sent",
                    clientCode = clientCode,
                    expirationDate = expirationDate,
                    registerUrl = registerUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client from employee submission: {Id}", id);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// List contact form submissions (admin)
        /// </summary>
        [HttpGet("contacts")]
        public async Task<ActionResult> GetContactSubmissions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? interest = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var query = _db.ContactFormSubmissions.AsNoTracking().OrderByDescending(e => e.CreatedAt).AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.ToLower();
                    query = query.Where(e =>
                        e.FirstName.ToLower().Contains(s) ||
                        e.LastName.ToLower().Contains(s) ||
                        e.Email.ToLower().Contains(s) ||
                        e.Phone.ToLower().Contains(s));
                }

                if (!string.IsNullOrWhiteSpace(interest))
                {
                    var i = interest.ToLower();
                    query = query.Where(e => e.Interest != null && e.Interest.ToLower().Contains(i));
                }

                var total = await query.CountAsync();
                var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                return Ok(new { success = true, total, page, pageSize, items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching contact submissions");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching contact submissions" });
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
                    
                    // Don't HTML encode URLs - they should remain as links
                    if (prop.PropertyType == typeof(string) && !prop.Name.Contains("Url"))
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

        private string GenerateRandomPassword()
        {
            // Generate a secure random password
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789!@#$%";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
    }
}

