using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AdminUsersController> _logger;

        public AdminUsersController(UserManager<AppUser> userManager, ILogger<AdminUsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var query = _userManager.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.ToLower();
                    query = query.Where(u =>
                        u.FirstName.ToLower().Contains(s) ||
                        u.LastName.ToLower().Contains(s) ||
                        u.Email!.ToLower().Contains(s));
                }

                var total = await query.CountAsync();
                var allUsers = await query
                    .OrderByDescending(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Get roles for each user and build response
                var usersWithRoles = new List<object>();
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var isActive = !user.LockoutEnabled || (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow);
                    
                    // Identity doesn't track creation date, so we'll use a default or null
                    usersWithRoles.Add(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        Roles = roles,
                        IsActive = isActive,
                        EmailConfirmed = user.EmailConfirmed,
                        CreatedAt = DateTime.UtcNow.AddMonths(-1) // Default date since Identity doesn't track this
                    });
                }

                return Ok(new { success = true, total, page, pageSize, items = usersWithRoles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching users" });
            }
        }

        /// <summary>
        /// Get user statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult> GetUserStats()
        {
            try
            {
                var allUsers = await _userManager.Users.ToListAsync();
                var totalUsers = allUsers.Count;
                var activeUsers = allUsers.Count(u => !u.LockoutEnabled || (u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow));
                
                var adminCount = 0;
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                        adminCount++;
                }

                return Ok(new
                {
                    totalUsers,
                    activeUsers,
                    inactiveUsers = totalUsers - activeUsers,
                    adminCount,
                    regularUsers = totalUsers - adminCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user statistics");
                return StatusCode(500, new { message = "An error occurred while fetching user statistics" });
            }
        }
    }
}

