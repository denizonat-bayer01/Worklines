using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/admin/email-templates")]
    [Authorize(Roles = "Admin")]
    public class AdminEmailTemplatesController : ControllerBase
    {
        private readonly IEmailTemplateService _templateService;
        private readonly WixiDbContext _db;
        private readonly ILogger<AdminEmailTemplatesController> _logger;

        public AdminEmailTemplatesController(
            IEmailTemplateService templateService,
            WixiDbContext db,
            ILogger<AdminEmailTemplatesController> logger)
        {
            _templateService = templateService;
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Get all email templates
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<EmailTemplate>>> GetAll()
        {
            try
            {
                var templates = await _templateService.GetAllAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching email templates");
                return StatusCode(500, new { message = "Error fetching email templates" });
            }
        }

        /// <summary>
        /// Get email template by key
        /// </summary>
        [HttpGet("{key}")]
        public async Task<ActionResult<EmailTemplate>> GetByKey(string key)
        {
            try
            {
                var template = await _templateService.GetByKeyAsync(key);
                if (template == null)
                    return NotFound();
                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching email template {Key}", key);
                return StatusCode(500, new { message = "Error fetching email template" });
            }
        }

        /// <summary>
        /// Create or update email template
        /// </summary>
        [HttpPut("{key}")]
        public async Task<ActionResult<EmailTemplate>> Upsert(string key, [FromBody] EmailTemplateDto dto)
        {
            try
            {
                var template = new EmailTemplate
                {
                    Key = key,
                    Subject = dto.Subject,
                    BodyHtml = dto.BodyHtml,
                    Description = dto.Description,
                    IsActive = dto.IsActive ?? true
                };

                var result = await _templateService.UpsertAsync(template, User?.Identity?.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting email template {Key}", key);
                return StatusCode(500, new { message = "Error saving email template" });
            }
        }

        /// <summary>
        /// Delete email template
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _templateService.DeleteAsync(id);
                if (!deleted)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting email template {Id}", id);
                return StatusCode(500, new { message = "Error deleting email template" });
            }
        }

        /// <summary>
        /// Preview email template with sample data
        /// </summary>
        [HttpGet("{key}/preview")]
        public async Task<ActionResult> Preview(string key)
        {
            try
            {
                var template = await _templateService.GetByKeyAsync(key);
                if (template == null)
                    return NotFound(new { message = "Template not found" });

                // Create sample data based on template key
                var sampleData = CreateSampleDataForTemplate(key);
                
                // Replace placeholders
                var previewSubject = ReplacePlaceholders(template.Subject, sampleData);
                var previewBody = ReplacePlaceholders(template.BodyHtml, sampleData);

                return Ok(new
                {
                    subject = previewSubject,
                    bodyHtml = previewBody
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing email template {Key}", key);
                return StatusCode(500, new { message = "Error generating preview" });
            }
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

        public class EmailTemplateDto
        {
            public string Subject { get; set; } = string.Empty;
            public string BodyHtml { get; set; } = string.Empty;
            public string? Description { get; set; }
            public bool? IsActive { get; set; }
        }
    }
}

