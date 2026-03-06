using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly ILogger<NewsController> _logger;

        public NewsController(WixiDbContext db, ILogger<NewsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Get all active news items (public)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetNewsItems([FromQuery] string? lang = "de")
        {
            try
            {
                var newsItems = await _db.NewsItems
                    .AsNoTracking()
                    .Where(n => n.IsActive && (n.PublishedAt == null || n.PublishedAt <= DateTime.UtcNow))
                    .OrderBy(n => n.DisplayOrder)
                    .ThenByDescending(n => n.PublishedAt ?? n.CreatedAt)
                    .ToListAsync();

                var result = newsItems.Select(n => new
                {
                    id = n.Id,
                    title = GetTitleByLang(n, lang),
                    excerpt = GetExcerptByLang(n, lang),
                    content = GetContentByLang(n, lang),
                    image = n.ImageUrl,
                    category = n.Category,
                    featured = n.Featured,
                    date = (n.PublishedAt ?? n.CreatedAt).ToString("dd.MM.yyyy"),
                    publishedAt = n.PublishedAt,
                    slug = n.Slug
                });

                return Ok(new { success = true, items = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news items");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching news items" });
            }
        }

        /// <summary>
        /// Get news item by ID (public)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetNewsItemById(long id, [FromQuery] string? lang = "de")
        {
            try
            {
                var newsItem = await _db.NewsItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id && n.IsActive && (n.PublishedAt == null || n.PublishedAt <= DateTime.UtcNow));

                if (newsItem == null)
                {
                    return NotFound(new { success = false, message = "News item not found" });
                }

                var result = new
                {
                    id = newsItem.Id,
                    title = GetTitleByLang(newsItem, lang),
                    excerpt = GetExcerptByLang(newsItem, lang),
                    content = GetContentByLang(newsItem, lang),
                    image = newsItem.ImageUrl,
                    category = newsItem.Category,
                    featured = newsItem.Featured,
                    date = (newsItem.PublishedAt ?? newsItem.CreatedAt).ToString("dd.MM.yyyy"),
                    publishedAt = newsItem.PublishedAt,
                    slug = newsItem.Slug
                };

                return Ok(new { success = true, item = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news item by ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching news item" });
            }
        }

        /// <summary>
        /// Get news item by slug (public)
        /// </summary>
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult> GetNewsItemBySlug(string slug, [FromQuery] string? lang = "de")
        {
            try
            {
                var newsItem = await _db.NewsItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Slug == slug && n.IsActive && (n.PublishedAt == null || n.PublishedAt <= DateTime.UtcNow));

                if (newsItem == null)
                {
                    return NotFound(new { success = false, message = "News item not found" });
                }

                var result = new
                {
                    id = newsItem.Id,
                    title = GetTitleByLang(newsItem, lang),
                    excerpt = GetExcerptByLang(newsItem, lang),
                    content = GetContentByLang(newsItem, lang),
                    image = newsItem.ImageUrl,
                    category = newsItem.Category,
                    featured = newsItem.Featured,
                    date = (newsItem.PublishedAt ?? newsItem.CreatedAt).ToString("dd.MM.yyyy"),
                    publishedAt = newsItem.PublishedAt,
                    slug = newsItem.Slug
                };

                return Ok(new { success = true, item = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news item by slug: {Slug}", slug);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching news item" });
            }
        }

        private string GetTitleByLang(NewsItem item, string? lang)
        {
            return lang switch
            {
                "tr" => item.TitleTr,
                "en" => item.TitleEn ?? item.TitleDe,
                "ar" => item.TitleAr ?? item.TitleDe,
                _ => item.TitleDe
            };
        }

        private string GetExcerptByLang(NewsItem item, string? lang)
        {
            return lang switch
            {
                "tr" => item.ExcerptTr,
                "en" => item.ExcerptEn ?? item.ExcerptDe,
                "ar" => item.ExcerptAr ?? item.ExcerptDe,
                _ => item.ExcerptDe
            };
        }

        private string? GetContentByLang(NewsItem item, string? lang)
        {
            return lang switch
            {
                "tr" => item.ContentTr,
                "en" => item.ContentEn ?? item.ContentDe,
                "ar" => item.ContentAr ?? item.ContentDe,
                _ => item.ContentDe
            };
        }
    }
}

