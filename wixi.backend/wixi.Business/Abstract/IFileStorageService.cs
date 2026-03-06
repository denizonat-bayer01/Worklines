using Microsoft.AspNetCore.Http;

namespace wixi.Business.Abstract
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Upload a file and return file metadata
        /// </summary>
        Task<FileUploadResult> UploadFileAsync(IFormFile file, string directory, string? customFileName = null);

        /// <summary>
        /// Delete a file
        /// </summary>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Get file stream for download
        /// </summary>
        Task<Stream?> GetFileStreamAsync(string filePath);

        /// <summary>
        /// Check if file exists
        /// </summary>
        Task<bool> FileExistsAsync(string filePath);

        /// <summary>
        /// Get file size in bytes
        /// </summary>
        Task<long> GetFileSizeAsync(string filePath);

        /// <summary>
        /// Validate file (extension, size, content)
        /// </summary>
        Task<FileValidationResult> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSizeBytes);
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public long FileSizeBytes { get; set; }
        public string? FileExtension { get; set; }
        public string? MimeType { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}

