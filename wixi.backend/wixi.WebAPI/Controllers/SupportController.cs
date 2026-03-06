using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using wixi.Business.Abstract;
using wixi.Entities.DTOs;

namespace wixi.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }

        // Ticket endpoints
        [HttpPost("tickets")]
        public async Task<IActionResult> CreateTicket([FromBody] SupportTicketCreateDto createDto)
        {
            try
            {
                var result = await _supportService.CreateTicketAsync(createDto);
                return Ok(new { success = true, message = "Ticket created successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating ticket");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("tickets/{ticketId}")]
        public async Task<IActionResult> GetTicket(long ticketId)
        {
            try
            {
                var result = await _supportService.GetTicketByIdAsync(ticketId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting ticket {TicketId}", ticketId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("tickets/client/{clientId}")]
        public async Task<IActionResult> GetClientTickets(int clientId)
        {
            try
            {
                var result = await _supportService.GetClientTicketsAsync(clientId);
                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting tickets for client {ClientId}", clientId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("tickets")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var result = await _supportService.GetAllTicketsAsync();
                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all tickets");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("tickets/{ticketId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTicket(long ticketId, [FromBody] SupportTicketUpdateDto updateDto)
        {
            try
            {
                var result = await _supportService.UpdateTicketAsync(ticketId, updateDto);
                return Ok(new { success = true, message = "Ticket updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating ticket {TicketId}", ticketId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("tickets/{ticketId}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTicket(long ticketId, [FromBody] TicketAssignDto assignDto)
        {
            try
            {
                var result = await _supportService.AssignTicketAsync(ticketId, assignDto);
                return Ok(new { success = true, message = "Ticket assigned successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error assigning ticket {TicketId}", ticketId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("tickets/{ticketId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTicketStatus(long ticketId, [FromBody] TicketStatusUpdateDto statusDto)
        {
            try
            {
                var result = await _supportService.UpdateTicketStatusAsync(ticketId, statusDto);
                return Ok(new { success = true, message = "Ticket status updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating ticket status {TicketId}", ticketId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // Message endpoints
        [HttpPost("messages")]
        public async Task<IActionResult> AddMessage([FromBody] SupportMessageCreateDto createDto)
        {
            try
            {
                var result = await _supportService.AddMessageAsync(createDto);
                return Ok(new { success = true, message = "Message added successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding message");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("tickets/{ticketId}/messages")]
        public async Task<IActionResult> GetTicketMessages(long ticketId)
        {
            try
            {
                var result = await _supportService.GetTicketMessagesAsync(ticketId);
                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting messages for ticket {TicketId}", ticketId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // FAQ endpoints
        [HttpGet("faqs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFAQs()
        {
            try
            {
                var result = await _supportService.GetAllFAQsAsync();
                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting FAQs");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("faqs/category/{category}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFAQsByCategory(string category)
        {
            try
            {
                var result = await _supportService.GetFAQsByCategoryAsync(category);
                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting FAQs for category {Category}", category);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("faqs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFAQ([FromBody] FAQCreateDto createDto)
        {
            try
            {
                var result = await _supportService.CreateFAQAsync(createDto);
                return Ok(new { success = true, message = "FAQ created successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating FAQ");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("faqs/{faqId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFAQ(int faqId, [FromBody] FAQUpdateDto updateDto)
        {
            try
            {
                var result = await _supportService.UpdateFAQAsync(faqId, updateDto);
                return Ok(new { success = true, message = "FAQ updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating FAQ {FaqId}", faqId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("faqs/{faqId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFAQ(int faqId)
        {
            try
            {
                await _supportService.DeleteFAQAsync(faqId);
                return Ok(new { success = true, message = "FAQ deleted successfully" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting FAQ {FaqId}", faqId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}

