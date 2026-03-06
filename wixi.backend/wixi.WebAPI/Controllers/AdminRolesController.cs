using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wixi.Entities.Concrete;
using wixi.Core.Exceptions;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/roles")]
    [Authorize(Roles = "Admin")]
    public class AdminRolesController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AdminRolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
            return Ok(roles);
        }

        public class CreateRoleRequest { public string Name { get; set; } = string.Empty; }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Role name is required");
            var exists = await _roleManager.RoleExistsAsync(request.Name);
            if (exists) return Conflict("Role already exists");
            var result = await _roleManager.CreateAsync(new AppRole { Name = request.Name });
            if (!result.Succeeded) return UnprocessableEntity(result.Errors);
            return NoContent();
        }

        [HttpGet("users")]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users.Select(u => new { u.Id, u.UserName, u.Email }).ToList();
            return Ok(users);
        }

        public class ChangeUserRoleRequest
        {
            public int UserId { get; set; }
            public string Role { get; set; } = string.Empty;
        }

        public class CreateUserRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Role { get; set; }
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("Email ve şifre gereklidir.", new Dictionary<string, string[]> 
                { 
                    { "Email", new[] { "Email gereklidir." } },
                    { "Password", new[] { "Şifre gereklidir." } }
                });

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null) 
                throw new BusinessException($"'{request.Email}' email adresi zaten kullanılıyor.");

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.ToDictionary(
                    e => e.Code,
                    e => new[] { e.Description }
                );
                throw new ValidationException("Kullanıcı oluşturulamadı.", errors);
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                if (!await _roleManager.RoleExistsAsync(request.Role))
                {
                    await _roleManager.CreateAsync(new AppRole { Name = request.Role });
                }
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            return Ok(new { user.Id, user.UserName, user.Email });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] ChangeUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return NotFound("User not found");
            if (!await _roleManager.RoleExistsAsync(request.Role)) return NotFound("Role not found");
            var res = await _userManager.AddToRoleAsync(user, request.Role);
            if (!res.Succeeded) return UnprocessableEntity(res.Errors);
            return NoContent();
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole([FromBody] ChangeUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return NotFound("User not found");
            if (!await _roleManager.RoleExistsAsync(request.Role)) return NotFound("Role not found");
            var res = await _userManager.RemoveFromRoleAsync(user, request.Role);
            if (!res.Succeeded) return UnprocessableEntity(res.Errors);
            return NoContent();
        }
    }
}


