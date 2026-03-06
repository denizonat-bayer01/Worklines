using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/logs")]
    [Authorize(Roles = "Admin")]
    public class AdminLogsController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly ILogger<AdminLogsController> _logger;

        public AdminLogsController(WixiDbContext db, ILogger<AdminLogsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Get application logs with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetApplicationLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? level = null,
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var query = _db.ApplicationLogs.AsQueryable();

                // Filter by level
                if (!string.IsNullOrEmpty(level) && level != "all")
                {
                    query = query.Where(l => l.Level == level);
                }

                // Filter by date range
                if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
                {
                    query = query.Where(l => l.Timestamp >= start);
                }

                if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
                {
                    var endOfDay = end.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(l => l.Timestamp <= endOfDay);
                }

                // Order by timestamp descending
                query = query.OrderByDescending(l => l.Timestamp);

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
                _logger.LogError(ex, "Error fetching application logs");
                return StatusCode(500, new { message = "Error fetching logs" });
            }
        }

        /// <summary>
        /// Get application logs statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult> GetLogsStats(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var query = _db.ApplicationLogs.AsQueryable();

                // Filter by date range
                if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
                {
                    query = query.Where(l => l.Timestamp >= start);
                }

                if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
                {
                    var endOfDay = end.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(l => l.Timestamp <= endOfDay);
                }

                var stats = new
                {
                    total = await query.CountAsync(),
                    errors = await query.CountAsync(l => l.Level == "Error" || l.Level == "Fatal"),
                    warnings = await query.CountAsync(l => l.Level == "Warning" || l.Level == "Warn"),
                    info = await query.CountAsync(l => l.Level == "Information" || l.Level == "Info"),
                    debug = await query.CountAsync(l => l.Level == "Debug"),
                    todayCount = await query.CountAsync(l => l.Timestamp >= DateTime.UtcNow.Date)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching logs statistics");
                return StatusCode(500, new { message = "Error fetching statistics" });
            }
        }
    }
}

