using wixi.Entities.DTOs;

namespace wixi.Business.Abstract
{
    public interface IClientService
    {
        /// <summary>
        /// Create a new client profile
        /// </summary>
        Task<ClientResponseDto> CreateClientAsync(ClientCreateDto createDto);

        /// <summary>
        /// Get client by ID with full details
        /// </summary>
        Task<ClientResponseDto> GetClientByIdAsync(int clientId);

        /// <summary>
        /// Get client by user ID
        /// </summary>
        Task<ClientResponseDto> GetClientByUserIdAsync(int userId);

        /// <summary>
        /// Get all clients (admin)
        /// </summary>
        Task<List<ClientResponseDto>> GetAllClientsAsync();

        /// <summary>
        /// Update client profile
        /// </summary>
        Task<ClientResponseDto> UpdateClientAsync(int clientId, ClientUpdateDto updateDto);

        /// <summary>
        /// Delete client
        /// </summary>
        Task<bool> DeleteClientAsync(int clientId);

        /// <summary>
        /// Add education info to client
        /// </summary>
        Task<EducationInfoDto> AddEducationInfoAsync(EducationInfoCreateDto createDto);

        /// <summary>
        /// Update education info
        /// </summary>
        Task<EducationInfoDto> UpdateEducationInfoAsync(int educationInfoId, EducationInfoUpdateDto updateDto);

        /// <summary>
        /// Delete education info
        /// </summary>
        Task<bool> DeleteEducationInfoAsync(int educationInfoId);

        /// <summary>
        /// Get all education types
        /// </summary>
        Task<List<EducationTypeDto>> GetEducationTypesAsync();
    }
}

