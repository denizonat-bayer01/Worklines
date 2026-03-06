using System.Diagnostics;
using System.Text;

namespace wixi.WebAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        // Add RequestId to response headers
        context.Response.Headers.Add("X-Request-ID", requestId);

        // Log request
        await LogRequest(context, requestId);

        // Capture original response body stream
        var originalBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
                stopwatch.Stop();

                // Log response
                await LogResponse(context, requestId, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Request {RequestId} failed after {ElapsedMs}ms - {Method} {Path}",
                    requestId, stopwatch.ElapsedMilliseconds, context.Request.Method, context.Request.Path);
                throw;
            }
            finally
            {
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }

    private async Task LogRequest(HttpContext context, string requestId)
    {
        var request = context.Request;

        var requestLog = new StringBuilder();
        requestLog.AppendLine($"Request {requestId} started");
        requestLog.AppendLine($"Method: {request.Method}");
        requestLog.AppendLine($"Path: {request.Path}");
        requestLog.AppendLine($"QueryString: {request.QueryString}");
        requestLog.AppendLine($"RemoteIP: {context.Connection.RemoteIpAddress}");
        requestLog.AppendLine($"UserAgent: {request.Headers["User-Agent"]}");

        // Log request headers (only important ones)
        var importantHeaders = new[] { "Content-Type", "Accept", "Authorization" };
        foreach (var header in importantHeaders)
        {
            if (request.Headers.TryGetValue(header, out var value))
            {
                var logValue = header == "Authorization" ? "***REDACTED***" : value.ToString();
                requestLog.AppendLine($"{header}: {logValue}");
            }
        }

        // Log request body (only for POST/PUT/PATCH and if content type is JSON)
        if (request.ContentLength > 0 &&
            (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH") &&
            request.ContentType?.Contains("application/json") == true)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body))
            {
                requestLog.AppendLine($"Body: {body}");
            }
        }

        _logger.LogInformation(requestLog.ToString());
    }

    private async Task LogResponse(HttpContext context, string requestId, long elapsedMs)
    {
        var response = context.Response;

        var responseLog = new StringBuilder();
        responseLog.AppendLine($"Request {requestId} completed in {elapsedMs}ms");
        responseLog.AppendLine($"StatusCode: {response.StatusCode}");
        responseLog.AppendLine($"ContentType: {response.ContentType}");

        // Log response body (only for successful JSON responses and if not too large)
        if (response.StatusCode < 400 &&
            response.ContentType?.Contains("application/json") == true &&
            response.Body.CanSeek &&
            response.Body.Length > 0 &&
            response.Body.Length < 10000) // Max 10KB
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            if (!string.IsNullOrEmpty(body))
            {
                responseLog.AppendLine($"Body: {body}");
            }
        }

        var logLevel = response.StatusCode >= 500 ? LogLevel.Error :
                       response.StatusCode >= 400 ? LogLevel.Warning :
                       LogLevel.Information;

        _logger.Log(logLevel, responseLog.ToString());
    }
}

