using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wixi.Business.Abstract;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/content-settings")]
    [Authorize(Roles = "Admin")]
    public class AdminContentSettingsController : ControllerBase
    {
        private readonly IContentSettingsService _settingsService;

        public AdminContentSettingsController(IContentSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<ActionResult<ContentSettings>> GetSettings()
        {
            var s = await _settingsService.GetAsync();
            if (s == null)
            {
                // Return empty object if no settings exist yet
                return Ok(new ContentSettings());
            }
            return Ok(s);
        }

        public class UpdateContentSettingsRequest
        {
            // Footer Company Description
            public string FooterCompanyDescDe { get; set; } = string.Empty;
            public string FooterCompanyDescTr { get; set; } = string.Empty;
            public string? FooterCompanyDescEn { get; set; }
            public string? FooterCompanyDescAr { get; set; }
            
            // Social Media Links
            public string? FacebookUrl { get; set; }
            public string? InstagramUrl { get; set; }
            public string? TwitterUrl { get; set; }
            public string? LinkedInUrl { get; set; }
            
            // About Mission Text 1
            public string AboutMissionText1De { get; set; } = string.Empty;
            public string AboutMissionText1Tr { get; set; } = string.Empty;
            public string? AboutMissionText1En { get; set; }
            public string? AboutMissionText1Ar { get; set; }
            
            // About Mission Text 2
            public string AboutMissionText2De { get; set; } = string.Empty;
            public string AboutMissionText2Tr { get; set; } = string.Empty;
            public string? AboutMissionText2En { get; set; }
            public string? AboutMissionText2Ar { get; set; }
            
            // Contact Information
            public string ContactPhone { get; set; } = string.Empty;
            public string ContactEmail { get; set; } = string.Empty;
            public string AddressGermany { get; set; } = string.Empty;
            public string AddressTurkeyMersin { get; set; } = string.Empty;
            public string AddressTurkeyIstanbul { get; set; } = string.Empty;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSettings([FromBody] UpdateContentSettingsRequest req)
        {
            var entity = new ContentSettings
            {
                FooterCompanyDescDe = req.FooterCompanyDescDe,
                FooterCompanyDescTr = req.FooterCompanyDescTr,
                FooterCompanyDescEn = req.FooterCompanyDescEn,
                FooterCompanyDescAr = req.FooterCompanyDescAr,
                FacebookUrl = req.FacebookUrl,
                InstagramUrl = req.InstagramUrl,
                TwitterUrl = req.TwitterUrl,
                LinkedInUrl = req.LinkedInUrl,
                AboutMissionText1De = req.AboutMissionText1De,
                AboutMissionText1Tr = req.AboutMissionText1Tr,
                AboutMissionText1En = req.AboutMissionText1En,
                AboutMissionText1Ar = req.AboutMissionText1Ar,
                AboutMissionText2De = req.AboutMissionText2De,
                AboutMissionText2Tr = req.AboutMissionText2Tr,
                AboutMissionText2En = req.AboutMissionText2En,
                AboutMissionText2Ar = req.AboutMissionText2Ar,
                ContactPhone = req.ContactPhone,
                ContactEmail = req.ContactEmail,
                AddressGermany = req.AddressGermany,
                AddressTurkeyMersin = req.AddressTurkeyMersin,
                AddressTurkeyIstanbul = req.AddressTurkeyIstanbul
            };
            await _settingsService.UpsertAsync(entity, User?.Identity?.Name);
            return NoContent();
        }
    }
}

