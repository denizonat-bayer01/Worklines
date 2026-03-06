using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly WixiDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(
            WixiDbContext db,
            UserManager<AppUser> userManager,
            ILogger<AdminDashboardController> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult> GetDashboardStats()
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();
                var activeUsers = await _userManager.Users
                    .CountAsync(u => !u.LockoutEnabled || (u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow));

                var totalEmployeeSubmissions = await _db.EmployeeFormSubmissions.CountAsync();
                var totalEmployerSubmissions = await _db.EmployerFormSubmissions.CountAsync();
                var totalContactSubmissions = await _db.ContactFormSubmissions.CountAsync();
                var totalFormSubmissions = totalEmployeeSubmissions + totalEmployerSubmissions + totalContactSubmissions;

                var totalEmailLogs = await _db.EmailLogs.CountAsync();
                var sentEmails = await _db.EmailLogs.CountAsync(e => e.Status == 1);
                var failedEmails = await _db.EmailLogs.CountAsync(e => e.Status == 2);

                var today = DateTime.UtcNow.Date;
                var recentEmployeeSubmissions = await _db.EmployeeFormSubmissions
                    .CountAsync(e => e.CreatedAt >= today);
                var recentEmployerSubmissions = await _db.EmployerFormSubmissions
                    .CountAsync(e => e.CreatedAt >= today);

                return Ok(new
                {
                    users = new
                    {
                        total = totalUsers,
                        active = activeUsers
                    },
                    formSubmissions = new
                    {
                        total = totalFormSubmissions,
                        employees = totalEmployeeSubmissions,
                        employers = totalEmployerSubmissions,
                        contact = totalContactSubmissions,
                        today = recentEmployeeSubmissions + recentEmployerSubmissions
                    },
                    emails = new
                    {
                        total = totalEmailLogs,
                        sent = sentEmails,
                        failed = failedEmails
                    },
                    systemStatus = "Aktif"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard statistics");
                return StatusCode(500, new { message = "An error occurred while fetching dashboard statistics" });
            }
        }
    }
}

