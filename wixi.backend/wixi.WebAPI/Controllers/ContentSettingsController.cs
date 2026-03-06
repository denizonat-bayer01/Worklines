using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wixi.Business.Abstract;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/content-settings")]
    public class ContentSettingsController : ControllerBase
    {
        private readonly IContentSettingsService _settingsService;

        public ContentSettingsController(IContentSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<ActionResult> GetSettings([FromQuery] string? lang = "de")
        {
            var s = await _settingsService.GetAsync();
            if (s == null) return NotFound();
            
            // Return translated content based on language
            var result = new
            {
                footerCompanyDesc = lang switch
                {
                    "tr" => s.FooterCompanyDescTr,
                    "en" => s.FooterCompanyDescEn ?? s.FooterCompanyDescDe,
                    "ar" => s.FooterCompanyDescAr ?? s.FooterCompanyDescDe,
                    _ => s.FooterCompanyDescDe
                },
                socialMedia = new
                {
                    facebook = s.FacebookUrl,
                    instagram = s.InstagramUrl,
                    twitter = s.TwitterUrl,
                    linkedin = s.LinkedInUrl
                },
                aboutMission = new
                {
                    text1 = lang switch
                    {
                        "tr" => s.AboutMissionText1Tr,
                        "en" => s.AboutMissionText1En ?? s.AboutMissionText1De,
                        "ar" => s.AboutMissionText1Ar ?? s.AboutMissionText1De,
                        _ => s.AboutMissionText1De
                    },
                    text2 = lang switch
                    {
                        "tr" => s.AboutMissionText2Tr,
                        "en" => s.AboutMissionText2En ?? s.AboutMissionText2De,
                        "ar" => s.AboutMissionText2Ar ?? s.AboutMissionText2De,
                        _ => s.AboutMissionText2De
                    }
                },
                contact = new
                {
                    phone = s.ContactPhone,
                    email = s.ContactEmail,
                    addresses = new
                    {
                        germany = s.AddressGermany,
                        turkeyMersin = s.AddressTurkeyMersin,
                        turkeyIstanbul = s.AddressTurkeyIstanbul
                    }
                }
            };
            
            return Ok(result);
        }
    }
}

