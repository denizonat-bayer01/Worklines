using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    public class SitemapController : ControllerBase
    {
        private readonly WixiDbContext _db;

        public SitemapController(WixiDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("sitemap.xml")]
        public async Task<IActionResult> GetSitemap()
        {
            // Site base url
            var baseUrl = await _db.SystemSettings.AsNoTracking().Select(s => s.SiteUrl).FirstOrDefaultAsync() ?? "https://worklines.de";
            if (baseUrl.EndsWith("/")) baseUrl = baseUrl.TrimEnd('/');

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            void AddUrl(string path)
            {
                var loc = path.StartsWith("http") ? path : baseUrl + (path.StartsWith("/") ? path : "/" + path);
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape(loc)}</loc>");
                sb.AppendLine("  </url>");
            }

            // Static routes
            AddUrl("/");
            AddUrl("/about");
            AddUrl("/contact");
            AddUrl("/services/ausbildung");
            AddUrl("/services/language");
            AddUrl("/services/university");
            AddUrl("/services/work");

            // Team members
            var members = await _db.TeamMembers.AsNoTracking().Where(t => t.IsActive).OrderBy(t => t.DisplayOrder).Select(t => t.Slug).ToListAsync();
            foreach (var slug in members)
            {
                AddUrl($"/team/{slug}");
            }

            // News items (published + active)
            var news = await _db.NewsItems.AsNoTracking()
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.PublishedAt)
                .Select(n => n.Slug ?? (n.Id.ToString()))
                .ToListAsync();
            foreach (var slug in news)
            {
                AddUrl($"/news/{slug}");
            }

            sb.AppendLine("</urlset>");
            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}


