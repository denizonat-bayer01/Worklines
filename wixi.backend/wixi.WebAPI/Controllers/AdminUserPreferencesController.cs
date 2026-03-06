using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wixi.Business.Abstract;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/user-preferences")]
    [Authorize]
    public class AdminUserPreferencesController : ControllerBase
    {
        private readonly IUserPreferenceService _service;

        public AdminUserPreferencesController(IUserPreferenceService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        public async Task<ActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var pref = await _service.GetOrCreateAsync(userId);
            return Ok(new { pref.Language, pref.Theme });
        }

        public class UpdatePreferenceRequest
        {
            public string? Language { get; set; }
            public string? Theme { get; set; }
        }

        [HttpPut("me")]
        public async Task<ActionResult> UpdateMe([FromBody] UpdatePreferenceRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var pref = await _service.UpsertAsync(userId, request.Language, request.Theme);
            return Ok(new { pref.Language, pref.Theme });
        }
    }
}


