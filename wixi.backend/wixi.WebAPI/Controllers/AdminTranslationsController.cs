using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using wixi.Business.Abstract;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/i18n")]
    [Authorize(Roles = "Admin")]
    public class AdminTranslationsController : ControllerBase
    {
        private readonly ITranslationService _service;

        public AdminTranslationsController(ITranslationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> List([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var items = await _service.ListAsync(search, page, pageSize);
            return Ok(items);
        }

        [HttpPut]
        public async Task<ActionResult> Upsert([FromBody] Translation item)
        {
            if (string.IsNullOrWhiteSpace(item.Key)) return BadRequest("Key is required");
            await _service.UpsertAsync(item, User?.Identity?.Name);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("invalidate")] 
        public ActionResult Invalidate([FromQuery] string? lang = null)
        {
            _service.InvalidateCache(lang);
            return NoContent();
        }

        public class BulkItem
        {
            [Required]
            [MaxLength(500)]
            public string Key { get; set; } = string.Empty;
            public string? De { get; set; }
            public string? Tr { get; set; }
            public string? En { get; set; }
            public string? Ar { get; set; }
        }

        public class BulkUpsertTranslationsRequest
        {
            [Required]
            public List<BulkItem> Items { get; set; } = new();
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> BulkUpsert([FromBody] BulkUpsertTranslationsRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest("No items provided");
            }

            foreach (var item in request.Items)
            {
                var entity = new Translation
                {
                    Key = item.Key,
                    De = item.De,
                    Tr = item.Tr,
                    En = item.En,
                    Ar = item.Ar
                };
                await _service.UpsertAsync(entity, User?.Identity?.Name);
            }

            return NoContent();
        }
    }
}


