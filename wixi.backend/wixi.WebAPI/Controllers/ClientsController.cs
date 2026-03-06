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
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientCreateDto createDto)
        {
            try
            {
                var result = await _clientService.CreateClientAsync(createDto);
                return Ok(new { success = true, message = "Client profile created successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating client");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetClient(int clientId)
        {
            try
            {
                var result = await _clientService.GetClientByIdAsync(clientId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting client {ClientId}", clientId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetClientByUser(int userId)
        {
            try
            {
                var result = await _clientService.GetClientByUserIdAsync(userId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting client for user {UserId}", userId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var result = await _clientService.GetAllClientsAsync();
                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all clients");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{clientId}")]
        public async Task<IActionResult> UpdateClient(int clientId, [FromBody] ClientUpdateDto updateDto)
        {
            try
            {
                var result = await _clientService.UpdateClientAsync(clientId, updateDto);
                return Ok(new { success = true, message = "Client profile updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating client {ClientId}", clientId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{clientId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int clientId)
        {
            try
            {
                await _clientService.DeleteClientAsync(clientId);
                return Ok(new { success = true, message = "Client profile deleted successfully" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting client {ClientId}", clientId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        // Education Info endpoints
        [HttpPost("education")]
        public async Task<IActionResult> AddEducationInfo([FromBody] EducationInfoCreateDto createDto)
        {
            try
            {
                var result = await _clientService.AddEducationInfoAsync(createDto);
                return Ok(new { success = true, message = "Education info added successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding education info");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("education/{educationInfoId}")]
        public async Task<IActionResult> UpdateEducationInfo(int educationInfoId, [FromBody] EducationInfoUpdateDto updateDto)
        {
            try
            {
                var result = await _clientService.UpdateEducationInfoAsync(educationInfoId, updateDto);
                return Ok(new { success = true, message = "Education info updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating education info {EducationInfoId}", educationInfoId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("education/{educationInfoId}")]
        public async Task<IActionResult> DeleteEducationInfo(int educationInfoId)
        {
            try
            {
                await _clientService.DeleteEducationInfoAsync(educationInfoId);
                return Ok(new { success = true, message = "Education info deleted successfully" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting education info {EducationInfoId}", educationInfoId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        // Education Types - Public endpoint for registration form
        [HttpGet("education-types")]
        [AllowAnonymous] // Allow access without authentication for registration form
        public async Task<IActionResult> GetEducationTypes()
        {
            try
            {
                var result = await _clientService.GetEducationTypesAsync();
                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting education types");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}

