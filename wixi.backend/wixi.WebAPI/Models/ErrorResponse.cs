using System;

namespace wixi.WebAPI.Models
{
    public sealed class ErrorResponse
    {
        public string TraceId { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public object? Errors { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public string? Path { get; set; }
    }
}


