using wixi.Entities.DTOs;

namespace wixi.Business.Abstract
{
    public interface IDocumentReviewService
    {
        /// <summary>
        /// Get all pending documents for review
        /// </summary>
        Task<List<DocumentResponseDto>> GetPendingDocumentsAsync();

        /// <summary>
        /// Get documents by status
        /// </summary>
        Task<List<DocumentResponseDto>> GetDocumentsByStatusAsync(string status);

        /// <summary>
        /// Review a document (approve/reject/request more info)
        /// </summary>
        Task<DocumentResponseDto> ReviewDocumentAsync(long documentId, DocumentReviewDto reviewDto, int reviewerId);

        /// <summary>
        /// Get review history for a document
        /// </summary>
        Task<List<DocumentReviewDto>> GetDocumentReviewHistoryAsync(long documentId);
    }
}

