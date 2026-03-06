using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/news")]
    [Authorize(Roles = "Admin")]
    public class AdminNewsController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly ILogger<AdminNewsController> _logger;

        public AdminNewsController(WixiDbContext db, ILogger<AdminNewsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// List all news items (admin)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetAllNewsItems()
        {
            try
            {
                var newsItems = await _db.NewsItems
                    .AsNoTracking()
                    .OrderBy(n => n.DisplayOrder)
                    .ThenByDescending(n => n.PublishedAt ?? n.CreatedAt)
                    .ToListAsync();

                return Ok(new { success = true, items = newsItems });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news items");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching news items" });
            }
        }

        /// <summary>
        /// Get news item by ID (admin)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetNewsItemById(long id)
        {
            try
            {
                var newsItem = await _db.NewsItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (newsItem == null)
                {
                    return NotFound(new { success = false, message = "News item not found" });
                }

                return Ok(new { success = true, item = newsItem });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news item by ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching news item" });
            }
        }

        /// <summary>
        /// Create news item (admin)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateNewsItem([FromBody] NewsItemDto dto)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(dto.Slug) && await _db.NewsItems.AnyAsync(n => n.Slug == dto.Slug))
                {
                    return BadRequest(new { success = false, message = "Slug already exists" });
                }

                var newsItem = new NewsItem
                {
                    TitleDe = dto.TitleDe,
                    TitleTr = dto.TitleTr,
                    TitleEn = dto.TitleEn,
                    TitleAr = dto.TitleAr,
                    ExcerptDe = dto.ExcerptDe,
                    ExcerptTr = dto.ExcerptTr,
                    ExcerptEn = dto.ExcerptEn,
                    ExcerptAr = dto.ExcerptAr,
                    ContentDe = dto.ContentDe,
                    ContentTr = dto.ContentTr,
                    ContentEn = dto.ContentEn,
                    ContentAr = dto.ContentAr,
                    ImageUrl = dto.ImageUrl,
                    Category = dto.Category,
                    Featured = dto.Featured,
                    PublishedAt = dto.PublishedAt,
                    Slug = dto.Slug,
                    DisplayOrder = dto.DisplayOrder,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _db.NewsItems.Add(newsItem);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, item = newsItem, message = "News item created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news item");
                return StatusCode(500, new { success = false, message = "An error occurred while creating news item" });
            }
        }

        /// <summary>
        /// Update news item (admin)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateNewsItem(long id, [FromBody] NewsItemDto dto)
        {
            try
            {
                var newsItem = await _db.NewsItems.FirstOrDefaultAsync(n => n.Id == id);

                if (newsItem == null)
                {
                    return NotFound(new { success = false, message = "News item not found" });
                }

                if (!string.IsNullOrWhiteSpace(dto.Slug) && dto.Slug != newsItem.Slug && await _db.NewsItems.AnyAsync(n => n.Slug == dto.Slug))
                {
                    return BadRequest(new { success = false, message = "Slug already exists" });
                }

                newsItem.TitleDe = dto.TitleDe;
                newsItem.TitleTr = dto.TitleTr;
                newsItem.TitleEn = dto.TitleEn;
                newsItem.TitleAr = dto.TitleAr;
                newsItem.ExcerptDe = dto.ExcerptDe;
                newsItem.ExcerptTr = dto.ExcerptTr;
                newsItem.ExcerptEn = dto.ExcerptEn;
                newsItem.ExcerptAr = dto.ExcerptAr;
                newsItem.ContentDe = dto.ContentDe;
                newsItem.ContentTr = dto.ContentTr;
                newsItem.ContentEn = dto.ContentEn;
                newsItem.ContentAr = dto.ContentAr;
                newsItem.ImageUrl = dto.ImageUrl;
                newsItem.Category = dto.Category;
                newsItem.Featured = dto.Featured;
                newsItem.PublishedAt = dto.PublishedAt;
                newsItem.Slug = dto.Slug;
                newsItem.DisplayOrder = dto.DisplayOrder;
                newsItem.IsActive = dto.IsActive;
                newsItem.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Ok(new { success = true, item = newsItem, message = "News item updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news item: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while updating news item" });
            }
        }

        /// <summary>
        /// Delete news item (admin)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNewsItem(long id)
        {
            try
            {
                var newsItem = await _db.NewsItems.FirstOrDefaultAsync(n => n.Id == id);

                if (newsItem == null)
                {
                    return NotFound(new { success = false, message = "News item not found" });
                }

                // Delete image if exists
                if (!string.IsNullOrWhiteSpace(newsItem.ImageUrl) && newsItem.ImageUrl.StartsWith("/News/"))
                {
                    var fileName = Path.GetFileName(newsItem.ImageUrl);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "News", fileName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _db.NewsItems.Remove(newsItem);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "News item deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news item: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while deleting news item" });
            }
        }

        /// <summary>
        /// Upload news image (admin)
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
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "News");

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
                var imageUrl = $"/News/{fileName}";

                return Ok(new { success = true, imageUrl = imageUrl, message = "Image uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, new { success = false, message = "An error occurred while uploading image" });
            }
        }

        /// <summary>
        /// Delete news image (admin)
        /// </summary>
        [HttpPost("delete-image")]
        public ActionResult DeleteImage([FromBody] DeleteNewsImageRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ImageUrl))
                {
                    return BadRequest(new { success = false, message = "Image URL is required" });
                }

                var fileName = Path.GetFileName(request.ImageUrl);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "News", fileName);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    return Ok(new { success = true, message = "Image deleted successfully" });
                }

                return NotFound(new { success = false, message = "Image not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image");
                return StatusCode(500, new { success = false, message = "An error occurred while deleting image" });
            }
        }
    }

    public class NewsItemDto
    {
        public string TitleDe { get; set; } = string.Empty;
        public string TitleTr { get; set; } = string.Empty;
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string ExcerptDe { get; set; } = string.Empty;
        public string ExcerptTr { get; set; } = string.Empty;
        public string? ExcerptEn { get; set; }
        public string? ExcerptAr { get; set; }
        public string? ContentDe { get; set; }
        public string? ContentTr { get; set; }
        public string? ContentEn { get; set; }
        public string? ContentAr { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool Featured { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? Slug { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class DeleteNewsImageRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
    }
}

