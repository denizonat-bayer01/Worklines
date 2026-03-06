using Microsoft.AspNetCore.Http;
using wixi.Entities.DTOs;

namespace wixi.Business.Abstract
{
    public interface IDocumentService
    {
        /// <summary>
        /// Upload a new document
        /// </summary>
        Task<DocumentResponseDto> UploadDocumentAsync(int clientId, int documentTypeId, IFormFile file);

        /// <summary>
        /// Get all documents for a client
        /// </summary>
        Task<ClientDocumentListDto> GetClientDocumentsAsync(int clientId);

        /// <summary>
        /// Get document by ID
        /// </summary>
        Task<DocumentResponseDto> GetDocumentByIdAsync(long documentId);

        /// <summary>
        /// Delete document
        /// </summary>
        Task<bool> DeleteDocumentAsync(long documentId, int clientId);

        /// <summary>
        /// Get document types by education type
        /// </summary>
        Task<List<DocumentTypeDto>> GetDocumentTypesByEducationAsync(int educationTypeId);

        /// <summary>
        /// Get all document types
        /// </summary>
        Task<List<DocumentTypeDto>> GetAllDocumentTypesAsync();

        /// <summary>
        /// Download document
        /// </summary>
        Task<(Stream stream, string fileName, string contentType)> DownloadDocumentAsync(long documentId);
    }
}
