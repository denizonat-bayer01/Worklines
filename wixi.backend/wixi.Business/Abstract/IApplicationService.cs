using wixi.Entities.DTOs;

namespace wixi.Business.Abstract
{
    public interface IApplicationService
    {
        /// <summary>
        /// Create a new application for a client
        /// </summary>
        Task<ApplicationResponseDto> CreateApplicationAsync(ApplicationCreateDto createDto);

        /// <summary>
        /// Get application by ID with full details
        /// </summary>
        Task<ApplicationResponseDto> GetApplicationByIdAsync(long applicationId);

        /// <summary>
        /// Get all applications for a client
        /// </summary>
        Task<List<ApplicationResponseDto>> GetClientApplicationsAsync(int clientId);

        /// <summary>
        /// Get all applications (admin)
        /// </summary>
        Task<List<ApplicationResponseDto>> GetAllApplicationsAsync();

        /// <summary>
        /// Update application notes
        /// </summary>
        Task<ApplicationResponseDto> UpdateApplicationAsync(long applicationId, ApplicationUpdateDto updateDto);

        /// <summary>
        /// Delete application
        /// </summary>
        Task<bool> DeleteApplicationAsync(long applicationId);

        /// <summary>
        /// Update step status
        /// </summary>
        Task<ApplicationStepResponseDto> UpdateStepAsync(long stepId, StepUpdateDto updateDto, int userId);

        /// <summary>
        /// Complete a sub-step
        /// </summary>
        Task<ApplicationSubStepResponseDto> UpdateSubStepAsync(long subStepId, SubStepUpdateDto updateDto, int userId);

        /// <summary>
        /// Get application history
        /// </summary>
        Task<List<ApplicationHistoryDto>> GetApplicationHistoryAsync(long applicationId);

        /// <summary>
        /// Get all application templates
        /// </summary>
        Task<List<ApplicationTemplateDto>> GetApplicationTemplatesAsync();

        /// <summary>
        /// Get template by ID with steps
        /// </summary>
        Task<ApplicationTemplateDto> GetTemplateByIdAsync(int templateId);
    }
}

