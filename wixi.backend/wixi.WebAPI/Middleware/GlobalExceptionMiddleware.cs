using System.Net;
using System.Text.Json;
using wixi.Core.Exceptions;
using wixi.WebAPI.Models;

namespace wixi.WebAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception ex)
        {
            var traceId = context.TraceIdentifier;
            var path = context.Request.Path.Value;
            var (status, message, errors) = ex switch
            {
                ValidationException ve => (HttpStatusCode.BadRequest, ve.Message, ve.Errors),
                BusinessException be => (HttpStatusCode.UnprocessableContent, be.Message, null),
                NotFoundException nf => (HttpStatusCode.NotFound, nf.Message, null),
                UnauthorizedException ue => (HttpStatusCode.Unauthorized, ue.Message, null),
                _ => (HttpStatusCode.InternalServerError, "Beklenmeyen bir hata oluştu.", null)
            };

            var response = new ErrorResponse
            {
                TraceId = traceId,
                StatusCode = (int)status,
                Message = message,
                Detail = ex is not (BusinessException or ValidationException or NotFoundException or UnauthorizedException) ? ex.Message : null,
                Errors = errors,
                Path = path,
                TimestampUtc = DateTime.UtcNow
            };

            // Log
            if ((int)status >= 500)
            {
                _logger.LogError(ex, "[Error] {Path} TraceId={TraceId}", path, traceId);
            }
            else
            {
                _logger.LogWarning(ex, "[Handled] {Status} {Path} TraceId={TraceId}", (int)status, path, traceId);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}


