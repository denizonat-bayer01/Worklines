using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.AspNetCore;
using Serilog.Extensions.Hosting;
using System.Security.Claims;

namespace wixi.WebAPI.Enrichers
{
    /// <summary>
    /// HTTP Context'ten ApplicationLog entity kolonlarına otomatik mapping yapan enricher
    /// AutoMapper tarzı merkezi mapping sistemi
    /// </summary>
    public class ApplicationLogEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationLogEnricher(IHttpContextAccessor? httpContextAccessor = null)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Log event'e ApplicationLog entity kolonlarını otomatik olarak ekler
        /// Serilog.Sinks.MSSqlServer bu property'leri custom column'lara eşler
        /// </summary>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            
            if (httpContext == null)
                return;

            // ApplicationLog entity property'lerini otomatik map et
            EnrichFromHttpContext(logEvent, propertyFactory, httpContext);
        }

        /// <summary>
        /// HTTP Context'ten ApplicationLog kolonlarına otomatik mapping
        /// </summary>
        private static void EnrichFromHttpContext(
            LogEvent logEvent, 
            ILogEventPropertyFactory propertyFactory, 
            HttpContext httpContext)
        {
            // Request bilgileri
            AddPropertyIfNotExists(logEvent, propertyFactory, "RequestPath", httpContext.Request.Path.Value ?? string.Empty);
            AddPropertyIfNotExists(logEvent, propertyFactory, "RequestMethod", httpContext.Request.Method);
            AddPropertyIfNotExists(logEvent, propertyFactory, "RequestId", httpContext.TraceIdentifier);
            
            // Response bilgileri (sadece response tamamlandıysa)
            if (httpContext.Response.HasStarted)
            {
                AddPropertyIfNotExists(logEvent, propertyFactory, "StatusCode", httpContext.Response.StatusCode);
            }

            // Network bilgileri - Smart IP Resolution
            var realIP = GetRealClientIP(httpContext);
            AddPropertyIfNotExists(logEvent, propertyFactory, "RemoteIP", realIP);
            AddPropertyIfNotExists(logEvent, propertyFactory, "UserAgent", httpContext.Request.Headers["User-Agent"].ToString() ?? string.Empty);

            // Authentication bilgileri
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                AddPropertyIfNotExists(logEvent, propertyFactory, "UserName", httpContext.User.Identity.Name ?? string.Empty);
                AddPropertyIfNotExists(logEvent, propertyFactory, "UserId", 
                    httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    httpContext.User.FindFirst("userId")?.Value ??
                    httpContext.User.FindFirst("sub")?.Value ??
                    string.Empty);
            }

            // Ek bilgiler (Properties JSON'da saklanır)
            AddPropertyIfNotExists(logEvent, propertyFactory, "RequestHost", httpContext.Request.Host.Value);
            AddPropertyIfNotExists(logEvent, propertyFactory, "RequestScheme", httpContext.Request.Scheme);
        }

        /// <summary>
        /// Gerçek client IP'sini bulur (proxy/load balancer header'larını kontrol eder)
        /// </summary>
        private static string GetRealClientIP(HttpContext httpContext)
        {
            // Priority order: Check multiple header sources
            var headerPriority = new[]
            {
                "CF-Connecting-IP",      // Cloudflare
                "True-Client-IP",        // Akamai, Cloudflare Enterprise
                "X-Real-IP",             // Nginx
                "X-Forwarded-For",       // Standard proxy header
                "X-Client-IP",           // Apache
                "X-Original-For",        // Custom proxy
                "Forwarded",             // RFC 7239
                "X-Cluster-Client-IP"    // Rackspace LB
            };

            foreach (var header in headerPriority)
            {
                var value = httpContext.Request.Headers[header].FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    // X-Forwarded-For can contain multiple IPs (client, proxy1, proxy2)
                    // Take the first one (real client)
                    var ip = value.Split(',')[0].Trim();
                    
                    // Clean up brackets and port (e.g., [::ffff:172.27.0.1]:40440)
                    if (ip.StartsWith("[") && ip.Contains("]:"))
                    {
                        ip = ip.Substring(1, ip.IndexOf("]") - 1);
                    }
                    else if (ip.Contains(":") && !ip.Contains("::"))
                    {
                        // IPv4:port format
                        ip = ip.Split(':')[0];
                    }
                    
                    // Clean up IPv6 mapped IPv4
                    ip = ip.Replace("::ffff:", "");
                    
                    // Validate it's not internal/private IP
                    if (!ip.StartsWith("127.") && 
                        !ip.StartsWith("10.") && 
                        !ip.StartsWith("172.16.") && 
                        !ip.StartsWith("172.17.") && 
                        !ip.StartsWith("172.18.") && 
                        !ip.StartsWith("172.19.") && 
                        !ip.StartsWith("172.27.") && 
                        !ip.StartsWith("192.168.") &&
                        !ip.Contains("::1") &&
                        ip != "localhost")
                    {
                        return ip;
                    }
                }
            }
            
            // Fallback to connection remote IP
            var remoteIP = httpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
            return remoteIP.Replace("::ffff:", "");
        }

        /// <summary>
        /// Property yoksa ekler (mevcut property'yi override etmez)
        /// </summary>
        private static void AddPropertyIfNotExists(
            LogEvent logEvent, 
            ILogEventPropertyFactory propertyFactory, 
            string propertyName, 
            object? value)
        {
            if (value == null)
                return;

            // Zaten varsa ekleme (önceden set edilmiş olabilir)
            if (logEvent.Properties.ContainsKey(propertyName))
                return;

            var property = propertyFactory.CreateProperty(propertyName, value);
            logEvent.AddPropertyIfAbsent(property);
        }
    }

    /// <summary>
    /// HTTP Request için özel enricher - Request logging middleware'inde kullanılır
    /// Elapsed time gibi request-specific bilgileri ekler
    /// </summary>
    public class RequestLogEnricher
    {
        /// <summary>
        /// Gerçek client IP'sini bulur (proxy/load balancer header'larını kontrol eder)
        /// </summary>
        private static string GetRealClientIP(HttpContext httpContext)
        {
            // Priority order: Check multiple header sources
            var headerPriority = new[]
            {
                "CF-Connecting-IP",      // Cloudflare
                "True-Client-IP",        // Akamai, Cloudflare Enterprise
                "X-Real-IP",             // Nginx
                "X-Forwarded-For",       // Standard proxy header
                "X-Client-IP",           // Apache
                "X-Original-For",        // Custom proxy
                "Forwarded",             // RFC 7239
                "X-Cluster-Client-IP"    // Rackspace LB
            };

            foreach (var header in headerPriority)
            {
                var value = httpContext.Request.Headers[header].FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    // X-Forwarded-For can contain multiple IPs (client, proxy1, proxy2)
                    // Take the first one (real client)
                    var ip = value.Split(',')[0].Trim();
                    
                    // Clean up brackets and port (e.g., [::ffff:172.27.0.1]:40440)
                    if (ip.StartsWith("[") && ip.Contains("]:"))
                    {
                        ip = ip.Substring(1, ip.IndexOf("]") - 1);
                    }
                    else if (ip.Contains(":") && !ip.Contains("::"))
                    {
                        // IPv4:port format
                        ip = ip.Split(':')[0];
                    }
                    
                    // Clean up IPv6 mapped IPv4
                    ip = ip.Replace("::ffff:", "");
                    
                    // Validate it's not internal/private IP
                    if (!ip.StartsWith("127.") && 
                        !ip.StartsWith("10.") && 
                        !ip.StartsWith("172.16.") && 
                        !ip.StartsWith("172.17.") && 
                        !ip.StartsWith("172.18.") && 
                        !ip.StartsWith("172.19.") && 
                        !ip.StartsWith("172.27.") && 
                        !ip.StartsWith("192.168.") &&
                        !ip.Contains("::1") &&
                        ip != "localhost")
                    {
                        return ip;
                    }
                }
            }
            
            // Fallback to connection remote IP
            var remoteIP = httpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
            return remoteIP.Replace("::ffff:", "");
        }

        /// <summary>
        /// DiagnosticContext'e ApplicationLog kolonlarını otomatik map eder
        /// Bu metod Serilog'un UseSerilogRequestLogging middleware'i tarafından otomatik çağrılır
        /// </summary>
        public static void EnrichDiagnosticContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            // ============================================
            // AUTO-MAPPING: ApplicationLog Entity Kolonları
            // ============================================
            // Bu property'ler Serilog.Sinks.MSSqlServer tarafından otomatik olarak
            // ApplicationLogs tablosundaki custom column'lara eşlenir
            
            // Request bilgileri → Database kolonları
            diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? string.Empty);
            diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
            diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
            diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
            
            // Network bilgileri → Database kolonları - Smart IP Resolution
            diagnosticContext.Set("RemoteIP", GetRealClientIP(httpContext));
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString() ?? string.Empty);
            
            // Authentication bilgileri → Database kolonları
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserName", httpContext.User.Identity.Name ?? string.Empty);
                diagnosticContext.Set("UserId", 
                    httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    httpContext.User.FindFirst("userId")?.Value ??
                    httpContext.User.FindFirst("sub")?.Value ??
                    string.Empty);
            }
            
            // Ek context (sadece Properties JSON'da saklanır, database kolonu değil)
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        }

        /// <summary>
        /// Elapsed time'ı database kolonuna ekler
        /// UseSerilogRequestLogging otomatik olarak {Elapsed} property'sini ekler,
        /// bu metod onu ElapsedMs kolonuna map eder
        /// </summary>
        public static void EnrichWithElapsedTime(IDiagnosticContext diagnosticContext, double elapsedMs)
        {
            diagnosticContext.Set("ElapsedMs", elapsedMs);
        }
    }
}

