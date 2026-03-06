using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wixi.Business.Abstract;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/i18n")]
    public class TranslationsController : ControllerBase
    {
        private readonly ITranslationService _service;

        public TranslationsController(ITranslationService service)
        {
            _service = service;
        }

        // Public: get overrides for a language
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string lang = "de")
        {
            var dict = await _service.GetTranslationsAsync(lang);
            return Ok(dict);
        }
    }
}


