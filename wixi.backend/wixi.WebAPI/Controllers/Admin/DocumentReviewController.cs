using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using wixi.Business.Abstract;
using wixi.Entities.DTOs;

namespace wixi.WebAPI.Controllers.Admin
{
    [Route("api/admin/document-review")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Requires Admin role
    public class DocumentReviewController : ControllerBase
    {
        private readonly IDocumentReviewService _documentReviewService;

        public DocumentReviewController(IDocumentReviewService documentReviewService)
        {
            _documentReviewService = documentReviewService;
        }

        /// <summary>
        /// Get all pending documents waiting for review
        /// </summary>
        [HttpGet("pending")]
        [ProducesResponseType(typeof(List<DocumentResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingDocuments()
        {
            try
            {
                Log.Information("Admin retrieving pending documents for review");

                var documents = await _documentReviewService.GetPendingDocumentsAsync();

                return Ok(new
                {
                    success = true,
                    data = documents,
                    count = documents.Count
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving pending documents");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get documents by status (Pending, Accepted, Rejected, MissingInfo, UnderReview)
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(List<DocumentResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentsByStatus(string status)
        {
            try
            {
                Log.Information("Admin retrieving documents with status {Status}", status);

                var documents = await _documentReviewService.GetDocumentsByStatusAsync(status);

                return Ok(new
                {
                    success = true,
                    data = documents,
                    count = documents.Count
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving documents by status {Status}", status);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Review a document (approve, reject, or request more info)
        /// </summary>
        [HttpPost("{documentId}/review")]
        [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReviewDocument(long documentId, [FromBody] DocumentReviewDto reviewDto)
        {
            try
            {
                // TODO: Get reviewer ID from authenticated user's claims
                var reviewerIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(reviewerIdClaim) || !int.TryParse(reviewerIdClaim, out var reviewerId))
                {
                    return Unauthorized(new { success = false, message = "Reviewer not authenticated" });
                }

                Log.Information("Admin {ReviewerId} reviewing document {DocumentId} with decision {Decision}",
                    reviewerId, documentId, reviewDto.Decision);

                var result = await _documentReviewService.ReviewDocumentAsync(documentId, reviewDto, reviewerId);

                return Ok(new
                {
                    success = true,
                    message = "Document reviewed successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error reviewing document {DocumentId}", documentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get review history for a document
        /// </summary>
        [HttpGet("{documentId}/history")]
        [ProducesResponseType(typeof(List<DocumentReviewDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReviewHistory(long documentId)
        {
            try
            {
                Log.Information("Admin retrieving review history for document {DocumentId}", documentId);

                var history = await _documentReviewService.GetDocumentReviewHistoryAsync(documentId);

                return Ok(new
                {
                    success = true,
                    data = history,
                    count = history.Count
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving review history for document {DocumentId}", documentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get document review statistics
        /// </summary>
        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReviewStats()
        {
            try
            {
                Log.Information("Admin retrieving document review statistics");

                var pending = await _documentReviewService.GetDocumentsByStatusAsync("Pending");
                var accepted = await _documentReviewService.GetDocumentsByStatusAsync("Accepted");
                var rejected = await _documentReviewService.GetDocumentsByStatusAsync("Rejected");
                var missingInfo = await _documentReviewService.GetDocumentsByStatusAsync("MissingInfo");

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        pending = pending.Count,
                        accepted = accepted.Count,
                        rejected = rejected.Count,
                        missingInfo = missingInfo.Count,
                        total = pending.Count + accepted.Count + rejected.Count + missingInfo.Count
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving review statistics");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}

