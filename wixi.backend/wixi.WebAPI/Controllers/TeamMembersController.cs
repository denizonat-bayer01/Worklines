using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/team-members")]
    public class TeamMembersController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly ILogger<TeamMembersController> _logger;

        public TeamMembersController(WixiDbContext db, ILogger<TeamMembersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Get all active team members (public)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetTeamMembers([FromQuery] string? lang = "de")
        {
            try
            {
                var members = await _db.TeamMembers
                    .AsNoTracking()
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.DisplayOrder)
                    .ThenBy(t => t.Name)
                    .ToListAsync();

                var result = members.Select(m => new
                {
                    id = m.Id,
                    slug = m.Slug,
                    name = m.Name,
                    imageUrl = m.ImageUrl,
                    image = m.ImageUrl,
                    email = m.Email,
                    phone = m.Phone,
                    experience = m.Experience,
                    position = lang == "tr" ? m.PositionTr : (lang == "en" ? m.PositionEn ?? m.PositionDe : m.PositionDe),
                    location = lang == "tr" ? m.LocationTr : (lang == "en" ? m.LocationEn ?? m.LocationDe : m.LocationDe),
                    education = lang == "tr" ? m.EducationTr : (lang == "en" ? m.EducationEn ?? m.EducationDe : m.EducationDe),
                    bio = lang == "tr" ? m.BioTr : (lang == "en" ? m.BioEn ?? m.BioDe : m.BioDe),
                    philosophy = lang == "tr" ? m.PhilosophyTr : (lang == "en" ? m.PhilosophyEn ?? m.PhilosophyDe : m.PhilosophyDe),
                    specializations = ParseJsonArray(lang == "tr" ? m.SpecializationsTr : (lang == "en" ? m.SpecializationsEn ?? m.SpecializationsDe : m.SpecializationsDe)),
                    languages = ParseJsonArray(lang == "tr" ? m.LanguagesTr : (lang == "en" ? m.LanguagesEn ?? m.LanguagesDe : m.LanguagesDe)),
                    achievements = ParseJsonArray(lang == "tr" ? m.AchievementsTr : (lang == "en" ? m.AchievementsEn ?? m.AchievementsDe : m.AchievementsDe)),
                    canProvideConsultation = m.CanProvideConsultation,
                    consultationPrice = m.ConsultationPrice,
                    consultationCurrency = m.ConsultationCurrency
                });

                return Ok(new { success = true, items = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching team members");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching team members" });
            }
        }

        /// <summary>
        /// Get team member by slug (public)
        /// </summary>
        [HttpGet("{slug}")]
        public async Task<ActionResult> GetTeamMemberBySlug(string slug, [FromQuery] string? lang = "de")
        {
            try
            {
                var member = await _db.TeamMembers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive);

                if (member == null)
                {
                    return NotFound(new { success = false, message = "Team member not found" });
                }

                var result = new
                {
                    id = member.Id,
                    slug = member.Slug,
                    name = member.Name,
                    imageUrl = member.ImageUrl,
                    image = member.ImageUrl,
                    email = member.Email,
                    phone = member.Phone,
                    experience = member.Experience,
                    position = lang == "tr" ? member.PositionTr : (lang == "en" ? member.PositionEn ?? member.PositionDe : member.PositionDe),
                    location = lang == "tr" ? member.LocationTr : (lang == "en" ? member.LocationEn ?? member.LocationDe : member.LocationDe),
                    education = lang == "tr" ? member.EducationTr : (lang == "en" ? member.EducationEn ?? member.EducationDe : member.EducationDe),
                    bio = lang == "tr" ? member.BioTr : (lang == "en" ? member.BioEn ?? member.BioDe : member.BioDe),
                    philosophy = lang == "tr" ? member.PhilosophyTr : (lang == "en" ? member.PhilosophyEn ?? member.PhilosophyDe : member.PhilosophyDe),
                    specializations = ParseJsonArray(lang == "tr" ? member.SpecializationsTr : (lang == "en" ? member.SpecializationsEn ?? member.SpecializationsDe : member.SpecializationsDe)),
                    languages = ParseJsonArray(lang == "tr" ? member.LanguagesTr : (lang == "en" ? member.LanguagesEn ?? member.LanguagesDe : member.LanguagesDe)),
                    achievements = ParseJsonArray(lang == "tr" ? member.AchievementsTr : (lang == "en" ? member.AchievementsEn ?? member.AchievementsDe : member.AchievementsDe)),
                    canProvideConsultation = member.CanProvideConsultation,
                    consultationPrice = member.ConsultationPrice,
                    consultationCurrency = member.ConsultationCurrency
                };

                return Ok(new { success = true, item = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching team member by slug: {Slug}", slug);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching team member" });
            }
        }

        private List<string> ParseJsonArray(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }

    [ApiController]
    [Route("api/admin/team-members")]
    [Authorize(Roles = "Admin")]
    public class AdminTeamMembersController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly ILogger<AdminTeamMembersController> _logger;

        public AdminTeamMembersController(WixiDbContext db, ILogger<AdminTeamMembersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// List all team members (admin)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetAllTeamMembers()
        {
            try
            {
                var members = await _db.TeamMembers
                    .AsNoTracking()
                    .OrderBy(t => t.DisplayOrder)
                    .ThenBy(t => t.Name)
                    .ToListAsync();

                return Ok(new { success = true, items = members });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching team members");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching team members" });
            }
        }

        /// <summary>
        /// Get team member by ID (admin)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTeamMemberById(long id)
        {
            try
            {
                var member = await _db.TeamMembers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (member == null)
                {
                    return NotFound(new { success = false, message = "Team member not found" });
                }

                return Ok(new { success = true, item = member });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching team member by ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching team member" });
            }
        }

        /// <summary>
        /// Create team member (admin)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateTeamMember([FromBody] TeamMemberDto dto)
        {
            try
            {
                if (await _db.TeamMembers.AnyAsync(t => t.Slug == dto.Slug))
                {
                    return BadRequest(new { success = false, message = "Slug already exists" });
                }

                var member = new TeamMember
                {
                    Name = dto.Name,
                    Slug = dto.Slug,
                    ImageUrl = dto.ImageUrl,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Experience = dto.Experience,
                    PositionDe = dto.PositionDe,
                    PositionTr = dto.PositionTr,
                    PositionEn = dto.PositionEn,
                    LocationDe = dto.LocationDe,
                    LocationTr = dto.LocationTr,
                    LocationEn = dto.LocationEn,
                    EducationDe = dto.EducationDe,
                    EducationTr = dto.EducationTr,
                    EducationEn = dto.EducationEn,
                    BioDe = dto.BioDe,
                    BioTr = dto.BioTr,
                    BioEn = dto.BioEn,
                    PhilosophyDe = dto.PhilosophyDe,
                    PhilosophyTr = dto.PhilosophyTr,
                    PhilosophyEn = dto.PhilosophyEn,
                    SpecializationsDe = SerializeJsonArray(dto.SpecializationsDe),
                    SpecializationsTr = SerializeJsonArray(dto.SpecializationsTr),
                    SpecializationsEn = SerializeJsonArray(dto.SpecializationsEn),
                    LanguagesDe = SerializeJsonArray(dto.LanguagesDe),
                    LanguagesTr = SerializeJsonArray(dto.LanguagesTr),
                    LanguagesEn = SerializeJsonArray(dto.LanguagesEn),
                    AchievementsDe = SerializeJsonArray(dto.AchievementsDe),
                    AchievementsTr = SerializeJsonArray(dto.AchievementsTr),
                    AchievementsEn = SerializeJsonArray(dto.AchievementsEn),
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CanProvideConsultation = dto.CanProvideConsultation,
                    ConsultationPrice = dto.ConsultationPrice,
                    ConsultationCurrency = dto.ConsultationCurrency,
                    CreatedAt = DateTime.UtcNow
                };

                _db.TeamMembers.Add(member);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, id = member.Id, message = "Team member created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team member");
                return StatusCode(500, new { success = false, message = "An error occurred while creating team member" });
            }
        }

        /// <summary>
        /// Update team member (admin)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTeamMember(long id, [FromBody] TeamMemberDto dto)
        {
            try
            {
                var member = await _db.TeamMembers.FindAsync(id);
                if (member == null)
                {
                    return NotFound(new { success = false, message = "Team member not found" });
                }

                if (member.Slug != dto.Slug && await _db.TeamMembers.AnyAsync(t => t.Slug == dto.Slug && t.Id != id))
                {
                    return BadRequest(new { success = false, message = "Slug already exists" });
                }

                member.Name = dto.Name;
                member.Slug = dto.Slug;
                member.ImageUrl = dto.ImageUrl;
                member.Email = dto.Email;
                member.Phone = dto.Phone;
                member.Experience = dto.Experience;
                member.PositionDe = dto.PositionDe;
                member.PositionTr = dto.PositionTr;
                member.PositionEn = dto.PositionEn;
                member.LocationDe = dto.LocationDe;
                member.LocationTr = dto.LocationTr;
                member.LocationEn = dto.LocationEn;
                member.EducationDe = dto.EducationDe;
                member.EducationTr = dto.EducationTr;
                member.EducationEn = dto.EducationEn;
                member.BioDe = dto.BioDe;
                member.BioTr = dto.BioTr;
                member.BioEn = dto.BioEn;
                member.PhilosophyDe = dto.PhilosophyDe;
                member.PhilosophyTr = dto.PhilosophyTr;
                member.PhilosophyEn = dto.PhilosophyEn;
                member.SpecializationsDe = SerializeJsonArray(dto.SpecializationsDe);
                member.SpecializationsTr = SerializeJsonArray(dto.SpecializationsTr);
                member.SpecializationsEn = SerializeJsonArray(dto.SpecializationsEn);
                member.LanguagesDe = SerializeJsonArray(dto.LanguagesDe);
                member.LanguagesTr = SerializeJsonArray(dto.LanguagesTr);
                member.LanguagesEn = SerializeJsonArray(dto.LanguagesEn);
                member.AchievementsDe = SerializeJsonArray(dto.AchievementsDe);
                member.AchievementsTr = SerializeJsonArray(dto.AchievementsTr);
                member.AchievementsEn = SerializeJsonArray(dto.AchievementsEn);
                member.DisplayOrder = dto.DisplayOrder;
                member.IsActive = dto.IsActive;
                member.CanProvideConsultation = dto.CanProvideConsultation;
                member.ConsultationPrice = dto.ConsultationPrice;
                member.ConsultationCurrency = dto.ConsultationCurrency;
                member.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Team member updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team member: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while updating team member" });
            }
        }

        /// <summary>
        /// Upload team member image (admin)
        /// </summary>
        [HttpPost("upload-image")]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Allowed types: jpg, jpeg, png, gif, webp" });
                }

                // Validate file size (max 5MB)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxFileSize)
                {
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TeamMembers");

                // Ensure directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return URL path
                var imageUrl = $"/TeamMembers/{fileName}";

                return Ok(new { success = true, imageUrl = imageUrl, message = "Image uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, new { success = false, message = "An error occurred while uploading image" });
            }
        }

        /// <summary>
        /// Delete team member image (admin)
        /// </summary>
        [HttpPost("delete-image")]
        public ActionResult DeleteImage([FromBody] DeleteImageRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.ImageUrl))
                {
                    return BadRequest(new { success = false, message = "Image URL is required" });
                }

                // Extract filename from URL
                var fileName = Path.GetFileName(request.ImageUrl);
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return BadRequest(new { success = false, message = "Invalid image URL" });
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TeamMembers", fileName);

                // Check if file exists
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { success = true, message = "Image deleted successfully" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Image not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting image" });
            }
        }

        /// <summary>
        /// Delete team member (admin)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeamMember(long id)
        {
            try
            {
                var member = await _db.TeamMembers.FindAsync(id);
                if (member == null)
                {
                    return NotFound(new { success = false, message = "Team member not found" });
                }

                _db.TeamMembers.Remove(member);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Team member deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team member: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while deleting team member" });
            }
        }

        private string? SerializeJsonArray(List<string>? items)
        {
            if (items == null || items.Count == 0)
                return null;

            try
            {
                return JsonSerializer.Serialize(items);
            }
            catch
            {
                return null;
            }
        }
    }

    public class DeleteImageRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class TeamMemberDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Experience { get; set; } = string.Empty;
        public string PositionDe { get; set; } = string.Empty;
        public string PositionTr { get; set; } = string.Empty;
        public string? PositionEn { get; set; }
        public string LocationDe { get; set; } = string.Empty;
        public string LocationTr { get; set; } = string.Empty;
        public string? LocationEn { get; set; }
        public string EducationDe { get; set; } = string.Empty;
        public string EducationTr { get; set; } = string.Empty;
        public string? EducationEn { get; set; }
        public string BioDe { get; set; } = string.Empty;
        public string BioTr { get; set; } = string.Empty;
        public string? BioEn { get; set; }
        public string? PhilosophyDe { get; set; }
        public string? PhilosophyTr { get; set; }
        public string? PhilosophyEn { get; set; }
        public List<string>? SpecializationsDe { get; set; }
        public List<string>? SpecializationsTr { get; set; }
        public List<string>? SpecializationsEn { get; set; }
        public List<string>? LanguagesDe { get; set; }
        public List<string>? LanguagesTr { get; set; }
        public List<string>? LanguagesEn { get; set; }
        public List<string>? AchievementsDe { get; set; }
        public List<string>? AchievementsTr { get; set; }
        public List<string>? AchievementsEn { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public bool CanProvideConsultation { get; set; } = false;
        public decimal? ConsultationPrice { get; set; }
        public string? ConsultationCurrency { get; set; }
    }
}

