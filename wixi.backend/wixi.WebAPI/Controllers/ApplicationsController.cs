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
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Create a new application for a client
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateApplication([FromBody] ApplicationCreateDto createDto)
        {
            try
            {
                Log.Information("Creating application for client {ClientId} with template {TemplateId}",
                    createDto.ClientId, createDto.TemplateId);

                var result = await _applicationService.CreateApplicationAsync(createDto);

                return Ok(new { success = true, message = "Application created successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating application");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get application by ID with full details
        /// </summary>
        [HttpGet("{applicationId}")]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetApplication(long applicationId)
        {
            try
            {
                Log.Information("Retrieving application {ApplicationId}", applicationId);

                var result = await _applicationService.GetApplicationByIdAsync(applicationId);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving application {ApplicationId}", applicationId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all applications for a client
        /// </summary>
        [HttpGet("client/{clientId}")]
        [ProducesResponseType(typeof(List<ApplicationResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientApplications(int clientId)
        {
            try
            {
                Log.Information("Retrieving applications for client {ClientId}", clientId);

                var result = await _applicationService.GetClientApplicationsAsync(clientId);

                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving applications for client {ClientId}", clientId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all applications (admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<ApplicationResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllApplications()
        {
            try
            {
                Log.Information("Admin retrieving all applications");

                var result = await _applicationService.GetAllApplicationsAsync();

                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving all applications");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update application notes
        /// </summary>
        [HttpPut("{applicationId}")]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateApplication(long applicationId, [FromBody] ApplicationUpdateDto updateDto)
        {
            try
            {
                Log.Information("Updating application {ApplicationId}", applicationId);

                var result = await _applicationService.UpdateApplicationAsync(applicationId, updateDto);

                return Ok(new { success = true, message = "Application updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating application {ApplicationId}", applicationId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an application
        /// </summary>
        [HttpDelete("{applicationId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteApplication(long applicationId)
        {
            try
            {
                Log.Information("Deleting application {ApplicationId}", applicationId);

                await _applicationService.DeleteApplicationAsync(applicationId);

                return Ok(new { success = true, message = "Application deleted successfully" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting application {ApplicationId}", applicationId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update step status (Admin/Staff)
        /// </summary>
        [HttpPut("steps/{stepId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApplicationStepResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStep(long stepId, [FromBody] StepUpdateDto updateDto)
        {
            try
            {
                // TODO: Get user ID from authenticated user
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                Log.Information("Admin {UserId} updating step {StepId}", userId, stepId);

                var result = await _applicationService.UpdateStepAsync(stepId, updateDto, userId);

                return Ok(new { success = true, message = "Step updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating step {StepId}", stepId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update sub-step completion status
        /// </summary>
        [HttpPut("sub-steps/{subStepId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApplicationSubStepResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSubStep(long subStepId, [FromBody] SubStepUpdateDto updateDto)
        {
            try
            {
                // TODO: Get user ID from authenticated user
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                Log.Information("Admin {UserId} updating sub-step {SubStepId}", userId, subStepId);

                var result = await _applicationService.UpdateSubStepAsync(subStepId, updateDto, userId);

                return Ok(new { success = true, message = "Sub-step updated successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating sub-step {SubStepId}", subStepId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get application history/timeline
        /// </summary>
        [HttpGet("{applicationId}/history")]
        [ProducesResponseType(typeof(List<ApplicationHistoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationHistory(long applicationId)
        {
            try
            {
                Log.Information("Retrieving history for application {ApplicationId}", applicationId);

                var result = await _applicationService.GetApplicationHistoryAsync(applicationId);

                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving history for application {ApplicationId}", applicationId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all application templates
        /// </summary>
        [HttpGet("templates")]
        [ProducesResponseType(typeof(List<ApplicationTemplateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTemplates()
        {
            try
            {
                Log.Information("Retrieving application templates");

                var result = await _applicationService.GetApplicationTemplatesAsync();

                return Ok(new { success = true, data = result, count = result.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving application templates");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get template by ID with steps
        /// </summary>
        [HttpGet("templates/{templateId}")]
        [ProducesResponseType(typeof(ApplicationTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTemplate(int templateId)
        {
            try
            {
                Log.Information("Retrieving template {TemplateId}", templateId);

                var result = await _applicationService.GetTemplateByIdAsync(templateId);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving template {TemplateId}", templateId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }
    }
}

