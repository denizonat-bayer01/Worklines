using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete.Document;
using wixi.Entities.DTOs;

namespace wixi.Business.Concrete
{
    public class DocumentService : IDocumentService
    {
        private readonly WixiDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
            WixiDbContext context,
            IFileStorageService fileStorageService,
            ILogger<DocumentService> logger)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task<DocumentResponseDto> UploadDocumentAsync(int clientId, int documentTypeId, IFormFile file)
        {
            // Validate client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
            {
                throw new Exception("Client not found");
            }

            // Get document type
            var documentType = await _context.DocumentTypes.FindAsync(documentTypeId);
            if (documentType == null || !documentType.IsActive)
            {
                throw new Exception("Document type not found or inactive");
            }

            // Validate file
            var allowedExtensions = documentType.AllowedFileTypes?.Split(',')
                .Select(e => e.Trim())
                .ToArray() ?? new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            var maxSize = documentType.MaxFileSizeBytes ?? 10485760; // 10MB default

            var validationResult = await _fileStorageService.ValidateFileAsync(file, allowedExtensions, maxSize);
            if (!validationResult.IsValid)
            {
                throw new Exception(string.Join(", ", validationResult.Errors));
            }

            // Check if document already exists for this type (get version)
            var existingCount = await _context.Documents
                .Where(d => d.ClientId == clientId && d.DocumentTypeId == documentTypeId)
                .CountAsync();

            // Upload file
            var uploadResult = await _fileStorageService.UploadFileAsync(file, $"client-{clientId}");

            if (!uploadResult.Success)
            {
                throw new Exception(uploadResult.ErrorMessage ?? "File upload failed");
            }

            // Create document entity
            var document = new Entities.Concrete.Document.Document
            {
                ClientId = clientId,
                DocumentTypeId = documentTypeId,
                OriginalFileName = uploadResult.FileName!,
                StoredFileName = uploadResult.StoredFileName!,
                FilePath = uploadResult.FilePath!,
                FileExtension = uploadResult.FileExtension!,
                FileSizeBytes = uploadResult.FileSizeBytes,
                MimeType = uploadResult.MimeType ?? "application/octet-stream",
                Status = DocumentStatus.Pending,
                Version = existingCount + 1,
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Document uploaded successfully. DocumentId: {DocumentId}, ClientId: {ClientId}",
                document.Id, clientId);

            // Return response
            return await GetDocumentByIdAsync(document.Id);
        }

        public async Task<ClientDocumentListDto> GetClientDocumentsAsync(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
            {
                throw new Exception("Client not found");
            }

            var documents = await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Review)
                .Where(d => d.ClientId == clientId && d.DeletedAt == null)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();

            var documentDtos = documents.Select(d => MapToDocumentDto(d, client)).ToList();

            return new ClientDocumentListDto
            {
                ClientId = clientId,
                ClientName = $"{client.FirstName} {client.LastName}",
                TotalDocuments = documents.Count,
                PendingDocuments = documents.Count(d => d.Status == DocumentStatus.Pending || d.Status == DocumentStatus.UnderReview),
                AcceptedDocuments = documents.Count(d => d.Status == DocumentStatus.Accepted),
                RejectedDocuments = documents.Count(d => d.Status == DocumentStatus.Rejected || d.Status == DocumentStatus.MissingInfo),
                Documents = documentDtos
            };
        }

        public async Task<DocumentResponseDto> GetDocumentByIdAsync(long documentId)
        {
            var document = await _context.Documents
                .Include(d => d.Client)
                    .ThenInclude(c => c.User)
                .Include(d => d.DocumentType)
                .Include(d => d.Review)
                .FirstOrDefaultAsync(d => d.Id == documentId && d.DeletedAt == null);

            if (document == null)
            {
                throw new Exception("Document not found");
            }

            return MapToDocumentDto(document, document.Client);
        }

        public async Task<bool> DeleteDocumentAsync(long documentId, int clientId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId && d.ClientId == clientId && d.DeletedAt == null);

            if (document == null)
            {
                throw new Exception("Document not found");
            }

            // Soft delete
            document.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Delete physical file
            await _fileStorageService.DeleteFileAsync(document.FilePath);

            _logger.LogInformation("Document deleted. DocumentId: {DocumentId}, ClientId: {ClientId}",
                documentId, clientId);

            return true;
        }

        public async Task<List<DocumentTypeDto>> GetDocumentTypesByEducationAsync(int educationTypeId)
        {
            var documentTypes = await _context.DocumentTypes
                .Include(dt => dt.EducationType)
                .Where(dt => dt.EducationTypeId == educationTypeId && dt.IsActive)
                .OrderBy(dt => dt.DisplayOrder)
                .ToListAsync();

            return documentTypes.Select(dt => new DocumentTypeDto
            {
                Id = dt.Id,
                Code = dt.Code,
                Name = dt.Name,
                NameEn = dt.NameEn,
                IsRequired = dt.IsRequired,
                AllowedFileTypes = dt.AllowedFileTypes,
                MaxFileSizeBytes = dt.MaxFileSizeBytes,
                MaxFileSizeFormatted = FormatFileSize(dt.MaxFileSizeBytes ?? 0),
                RequiresApproval = dt.RequiresApproval,
                Note = dt.Note,
                EducationTypeId = dt.EducationTypeId,
                EducationTypeName = dt.EducationType?.Name
            }).ToList();
        }

        public async Task<List<DocumentTypeDto>> GetAllDocumentTypesAsync()
        {
            var documentTypes = await _context.DocumentTypes
                .Include(dt => dt.EducationType)
                .Where(dt => dt.IsActive)
                .OrderBy(dt => dt.EducationTypeId)
                    .ThenBy(dt => dt.DisplayOrder)
                .ToListAsync();

            return documentTypes.Select(dt => new DocumentTypeDto
            {
                Id = dt.Id,
                Code = dt.Code,
                Name = dt.Name,
                NameEn = dt.NameEn,
                IsRequired = dt.IsRequired,
                AllowedFileTypes = dt.AllowedFileTypes,
                MaxFileSizeBytes = dt.MaxFileSizeBytes,
                MaxFileSizeFormatted = FormatFileSize(dt.MaxFileSizeBytes ?? 0),
                RequiresApproval = dt.RequiresApproval,
                Note = dt.Note,
                EducationTypeId = dt.EducationTypeId,
                EducationTypeName = dt.EducationType?.Name
            }).ToList();
        }

        public async Task<(Stream stream, string fileName, string contentType)> DownloadDocumentAsync(long documentId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId && d.DeletedAt == null);

            if (document == null)
            {
                throw new Exception("Document not found");
            }

            var stream = await _fileStorageService.GetFileStreamAsync(document.FilePath);

            if (stream == null)
            {
                throw new Exception("File not found on server");
            }

            var contentType = document.MimeType ?? "application/octet-stream";

            return (stream, document.OriginalFileName, contentType);
        }

        // Helper methods
        private DocumentResponseDto MapToDocumentDto(Entities.Concrete.Document.Document document, Entities.Concrete.Client.Client client)
        {
            return new DocumentResponseDto
            {
                Id = document.Id,
                ClientId = document.ClientId,
                ClientName = $"{client.FirstName} {client.LastName}",
                DocumentTypeId = document.DocumentTypeId,
                DocumentTypeName = document.DocumentType.Name,
                OriginalFileName = document.OriginalFileName,
                FilePath = document.FilePath,
                FileSizeBytes = document.FileSizeBytes,
                FileSizeFormatted = FormatFileSize(document.FileSizeBytes),
                FileExtension = document.FileExtension,
                Status = document.Status.ToString(),
                Version = document.Version,
                UploadedAt = document.UploadedAt,
                HasReview = document.Review != null,
                ReviewDecision = document.Review?.Decision.ToString(),
                FeedbackMessage = document.Review?.FeedbackMessage,
                ReviewedAt = document.Review?.ReviewedAt
            };
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
