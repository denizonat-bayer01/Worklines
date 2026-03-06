using Microsoft.AspNetCore.Mvc;
using wixi.Business.Abstract;
using wixi.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using wixi.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace wixi.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(
            IAuthService authService,
            UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserForLoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserForRegisterDto registerDto)
        {
            try
            {
                Log.Information("Register endpoint called - Email: {Email}, ClientCode: {ClientCode}", 
                    registerDto.Email, registerDto.ClientCode ?? "N/A");

                // Parse educationHistory from JSON string if provided
                var educationHistoryJson = Request.Form["educationHistory"].FirstOrDefault();
                if (!string.IsNullOrEmpty(educationHistoryJson))
                {
                    try
                    {
                        registerDto.EducationHistory = System.Text.Json.JsonSerializer.Deserialize<List<EducationInfoCreateDto>>(
                            educationHistoryJson,
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                        Log.Information("Education history parsed successfully - Count: {Count}", 
                            registerDto.EducationHistory?.Count ?? 0);
                    }
                    catch (Exception parseEx)
                    {
                        // Log but don't fail registration
                        Log.Warning(parseEx, "Failed to parse educationHistory - Error: {Error}", parseEx.Message);
                    }
                }

                Log.Information("Calling AuthService.RegisterAsync");
                var result = await _authService.RegisterAsync(registerDto);
                Log.Information("Registration successful - Email: {Email}", registerDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log full exception details including inner exceptions
                Log.Error(ex, "Registration failed - Email: {Email}, Error: {Error}, StackTrace: {StackTrace}", 
                    registerDto?.Email ?? "N/A", ex.Message, ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException, "Inner exception - Type: {Type}, Message: {Message}", 
                        ex.InnerException.GetType().Name, ex.InnerException.Message);
                    
                    if (ex.InnerException.InnerException != null)
                    {
                        Log.Error(ex.InnerException.InnerException, "Inner inner exception - Type: {Type}, Message: {Message}", 
                            ex.InnerException.InnerException.GetType().Name, ex.InnerException.InnerException.Message);
                    }
                }

                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromForm] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromForm] string refreshToken)
        {
            try
            {
                await _authService.LogoutAsync(refreshToken);
                return Ok(new { message = "Başarıyla çıkış yapıldı" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _authService.GetUserByIdAsync(int.Parse(userId));
                if (user == null)
                {
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("is-admin")]
        public IActionResult IsAdmin()
        {
            try
            {
                var isAdmin = User.Claims
                    .Any(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");

                return Ok(isAdmin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "İstek işlenirken bir hata oluştu" });
            }
        }
    }
}

