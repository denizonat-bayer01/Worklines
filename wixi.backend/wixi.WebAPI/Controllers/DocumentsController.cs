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
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Upload a document for a client
        /// </summary>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument([FromForm] int clientId, [FromForm] int documentTypeId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "File is required" });
                }

                Log.Information("Uploading document for client {ClientId}, type {DocumentTypeId}", clientId, documentTypeId);

                var result = await _documentService.UploadDocumentAsync(clientId, documentTypeId, file);

                return Ok(new { success = true, message = "Document uploaded successfully", data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error uploading document for client {ClientId}", clientId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all documents for a client
        /// </summary>
        [HttpGet("client/{clientId}")]
        [ProducesResponseType(typeof(ClientDocumentListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClientDocuments(int clientId)
        {
            try
            {
                Log.Information("Retrieving documents for client {ClientId}", clientId);

                var result = await _documentService.GetClientDocumentsAsync(clientId);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving documents for client {ClientId}", clientId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get my documents (for authenticated client)
        /// </summary>
        [HttpGet("my-documents")]
        [ProducesResponseType(typeof(ClientDocumentListDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyDocuments()
        {
            // TODO: Get client ID from authenticated user's claims
            // For now, this is a placeholder - needs integration with auth system
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            // TODO: Get client ID from user ID
            // This would require a query to get the client associated with the user
            return BadRequest(new { success = false, message = "Client ID mapping not yet implemented. Use /api/documents/client/{clientId} endpoint" });
        }

        /// <summary>
        /// Get document by ID
        /// </summary>
        [HttpGet("{documentId}")]
        [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocument(long documentId)
        {
            try
            {
                Log.Information("Retrieving document {DocumentId}", documentId);

                var result = await _documentService.GetDocumentByIdAsync(documentId);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving document {DocumentId}", documentId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        [HttpDelete("{documentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(long documentId, [FromQuery] int clientId)
        {
            try
            {
                Log.Information("Deleting document {DocumentId} for client {ClientId}", documentId, clientId);

                await _documentService.DeleteDocumentAsync(documentId, clientId);

                return Ok(new { success = true, message = "Document deleted successfully" });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting document {DocumentId}", documentId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Download a document
        /// </summary>
        [HttpGet("download/{documentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadDocument(long documentId)
        {
            try
            {
                Log.Information("Downloading document {DocumentId}", documentId);

                var (stream, fileName, contentType) = await _documentService.DownloadDocumentAsync(documentId);

                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error downloading document {DocumentId}", documentId);
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all document types
        /// </summary>
        [HttpGet("types")]
        [ProducesResponseType(typeof(List<DocumentTypeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentTypes()
        {
            try
            {
                Log.Information("Retrieving all document types");

                var result = await _documentService.GetAllDocumentTypesAsync();

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving document types");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get document types by education type
        /// </summary>
        [HttpGet("types/education/{educationTypeId}")]
        [ProducesResponseType(typeof(List<DocumentTypeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentTypesByEducation(int educationTypeId)
        {
            try
            {
                Log.Information("Retrieving document types for education type {EducationTypeId}", educationTypeId);

                var result = await _documentService.GetDocumentTypesByEducationAsync(educationTypeId);

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving document types for education type {EducationTypeId}", educationTypeId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
