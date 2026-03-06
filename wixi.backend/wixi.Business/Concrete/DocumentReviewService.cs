using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete.Document;
using wixi.Entities.DTOs;

namespace wixi.Business.Concrete
{
    public class DocumentReviewService : IDocumentReviewService
    {
        private readonly WixiDbContext _context;
        private readonly ILogger<DocumentReviewService> _logger;

        public DocumentReviewService(
            WixiDbContext context,
            ILogger<DocumentReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<DocumentResponseDto>> GetPendingDocumentsAsync()
        {
            var documents = await _context.Documents
                .Include(d => d.Client)
                    .ThenInclude(c => c.User)
                .Include(d => d.DocumentType)
                .Where(d => d.Status == DocumentStatus.Pending && d.DeletedAt == null)
                .OrderBy(d => d.UploadedAt)
                .ToListAsync();

            return documents.Select(d => MapToDocumentDto(d)).ToList();
        }

        public async Task<List<DocumentResponseDto>> GetDocumentsByStatusAsync(string status)
        {
            if (!Enum.TryParse<DocumentStatus>(status, true, out var documentStatus))
            {
                throw new Exception($"Invalid document status: {status}");
            }

            var documents = await _context.Documents
                .Include(d => d.Client)
                    .ThenInclude(c => c.User)
                .Include(d => d.DocumentType)
                .Include(d => d.Review)
                .Where(d => d.Status == documentStatus && d.DeletedAt == null)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();

            return documents.Select(d => MapToDocumentDto(d)).ToList();
        }

        public async Task<DocumentResponseDto> ReviewDocumentAsync(long documentId, DocumentReviewDto reviewDto, int reviewerId)
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

            // Parse decision
            if (!Enum.TryParse<DocumentStatus>(reviewDto.Decision, true, out var decision))
            {
                throw new Exception($"Invalid review decision: {reviewDto.Decision}");
            }

            // Update document status based on decision
            document.Status = decision;

            // Create or update review
            if (document.Review == null)
            {
                document.Review = new DocumentReview
                {
                    DocumentId = documentId,
                    ReviewerId = reviewerId,
                    Decision = decision,
                    ReviewNote = reviewDto.ReviewNote,
                    FeedbackMessage = reviewDto.FeedbackMessage,
                    ReviewedAt = DateTime.UtcNow
                };
                _context.DocumentReviews.Add(document.Review);
            }
            else
            {
                document.Review.ReviewerId = reviewerId;
                document.Review.Decision = decision;
                document.Review.ReviewNote = reviewDto.ReviewNote;
                document.Review.FeedbackMessage = reviewDto.FeedbackMessage;
                document.Review.ReviewedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Document {DocumentId} reviewed with decision {Decision} by reviewer {ReviewerId}",
                documentId, decision, reviewerId);

            return MapToDocumentDto(document);
        }

        public async Task<List<DocumentReviewDto>> GetDocumentReviewHistoryAsync(long documentId)
        {
            var reviews = await _context.DocumentReviews
                .Include(r => r.Reviewer)
                .Where(r => r.DocumentId == documentId)
                .OrderByDescending(r => r.ReviewedAt)
                .ToListAsync();

            return reviews.Select(r => new DocumentReviewDto
            {
                DocumentId = r.DocumentId,
                Decision = r.Decision.ToString(),
                ReviewNote = r.ReviewNote,
                FeedbackMessage = r.FeedbackMessage
            }).ToList();
        }

        private DocumentResponseDto MapToDocumentDto(Entities.Concrete.Document.Document document)
        {
            return new DocumentResponseDto
            {
                Id = document.Id,
                ClientId = document.ClientId,
                ClientName = $"{document.Client.FirstName} {document.Client.LastName}",
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

