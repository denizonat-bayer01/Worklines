using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wixi.Business.Abstract;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/settings")]
    [Authorize(Roles = "Admin")]
    public class AdminSettingsController : ControllerBase
    {
        private readonly ISystemSettingsService _settingsService;

        public AdminSettingsController(ISystemSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<ActionResult<SystemSettings>> GetSettings()
        {
            var s = await _settingsService.GetAsync();
            if (s == null)
            {
                // Return empty object if no settings exist yet
                return Ok(new SystemSettings());
            }
            return Ok(s);
        }

        public class UpdateSystemSettingsRequest
        {
            public string SiteName { get; set; } = string.Empty;
            public string SiteUrl { get; set; } = string.Empty;
            public string AdminEmail { get; set; } = string.Empty;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSettings([FromBody] UpdateSystemSettingsRequest req)
        {
            var entity = new SystemSettings
            {
                SiteName = req.SiteName,
                SiteUrl = req.SiteUrl,
                AdminEmail = req.AdminEmail
            };
            await _settingsService.UpsertAsync(entity, User?.Identity?.Name);
            return NoContent();
        }
    }
}

