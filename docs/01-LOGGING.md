# 📝 Logging Sistemi - Serilog Entegrasyonu

## 🎯 Amaç

Production-ready bir logging sistemi kurmak için Serilog entegrasyonu.

---

## 📦 Gerekli NuGet Paketleri

```bash
# wixi.WebAPI projesine eklenecek
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Enrichers.Process
dotnet add package Serilog.Enrichers.Thread
dotnet add package Serilog.Settings.Configuration
```

---

## 🔧 Konfigürasyon

### 1. appsettings.json Güncelle

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "fileSizeLimitBytes": 10485760,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithProcessId", "WithThreadId" ],
    "Properties": {
      "Application": "Wixi.Backend"
    }
  },
  
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "JwtTokenOptions": {
    ...
  }
}
```

### 2. appsettings.Development.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

### 3. appsettings.Production.json (oluştur)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/app/logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

---

## 🛠️ Implementation

### 1. Program.cs Güncelle

```csharp
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Wixi Backend API");

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Swagger yapılandırması
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wixi Backend API", Version = "v1" });
        
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme.",
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // Configure Services
    builder.Services.AddDbContextExt(builder.Configuration);
    builder.Services.AddIdentityExt();
    builder.Services.AddJwtExt(builder.Configuration);
    builder.Services.AddServiceCollectionExt();
    builder.Services.AddHttpContextAccessor();

    // CORS yapılandırması
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(corsBuilder =>
        {
            corsBuilder.WithOrigins(
                    "https://worklines.de",
                    "https://www.worklines.de",
                    "http://localhost:5500",
                    "http://localhost:5173"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    var app = builder.Build();

    // Seed data
    await app.SeedDataAsync();

    // Serilog request logging middleware
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : LogEventLevel.Information;
        
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
            
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
            }
        };
    });

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Wixi Backend API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
```

---

## 📂 Yeni Klasör Yapısı

```
wixi.backend/
├── Logs/                       # Log dosyaları (gitignore'a ekle)
│   └── log-20251028.txt
│
└── wixi.WebAPI/
    ├── Extensions/
    │   └── LoggingExtension.cs  # (OLUŞTURULACAK)
    └── Middleware/
        └── RequestLoggingMiddleware.cs  # (OLUŞTURULACAK)
```

---

## 🔨 Custom Extensions

### 1. LoggingExtension.cs Oluştur

```csharp
// wixi.WebAPI/Extensions/LoggingExtension.cs
using Serilog;

namespace wixi.WebAPI.Extensions
{
    public static class LoggingExtension
    {
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .Enrich.WithProperty("Application", "Wixi.Backend")
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}
```

### 2. RequestLoggingMiddleware.cs (Opsiyonel)

```csharp
// wixi.WebAPI/Middleware/RequestLoggingMiddleware.cs
using System.Diagnostics;
using System.Text;

namespace wixi.WebAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Request logging
            var request = await FormatRequest(context.Request);
            _logger.LogInformation("Incoming Request: {Request}", request);

            // Response logging
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            stopwatch.Stop();
            
            var response = await FormatResponse(context.Response);
            _logger.LogInformation(
                "Outgoing Response: {Response} | Elapsed: {ElapsedMs}ms",
                response,
                stopwatch.ElapsedMilliseconds
            );

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            return $"{request.Method} {request.Path} {request.QueryString} {body}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{response.StatusCode}: {text}";
        }
    }
}
```

---

## 📝 Controller'larda Kullanım

### AuthController.cs Örneği

```csharp
using Microsoft.AspNetCore.Mvc;
using wixi.Business.Abstract;
using wixi.Entities.DTOs;

namespace wixi.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger; // ILogger ekle

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserForLoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Email}", loginDto.Email);
                
                var result = await _authService.LoginAsync(loginDto);
                
                _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user: {Email}", loginDto.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserForRegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);
                
                var result = await _authService.RegisterAsync(registerDto);
                
                _logger.LogInformation("User {Email} registered successfully", registerDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for email: {Email}", registerDto.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromForm] string refreshToken)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Logout attempt for user: {UserId}", userId);
                
                await _authService.LogoutAsync(refreshToken);
                
                _logger.LogInformation("User {UserId} logged out successfully", userId);
                return Ok(new { message = "Başarıyla çıkış yapıldı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
```

---

## 🧪 Test Etme

### 1. Local Development

```bash
# API'yi çalıştır
dotnet run --project wixi.WebAPI

# Log dosyalarını kontrol et
ls Logs/
cat Logs/log-20251028.txt
```

### 2. Seq ile Görselleştirme

```bash
# Seq'i Docker ile çalıştır
docker run -d --name seq -p 5341:80 -e ACCEPT_EULA=Y datalust/seq:latest

# Browser'da aç
http://localhost:5341
```

### 3. Log Örnekleri

```log
[14:35:21 INF] Starting Wixi Backend API
[14:35:22 INF] Wixi Backend API started successfully
[14:35:25 INF] Login attempt for user: "admin@worklines.de"
[14:35:26 INF] HTTP POST /api/Auth/login responded 200 in 234.5678 ms
[14:35:26 INF] User "admin@worklines.de" logged in successfully
```

---

## 📊 Log Levels

| Level | Kullanım | Örnek |
|-------|----------|-------|
| **Trace** | Çok detaylı debug | Method entry/exit |
| **Debug** | Development debugging | Variable values |
| **Information** | Genel bilgi | User actions |
| **Warning** | Uyarılar | Deprecated API usage |
| **Error** | Hatalar | Exceptions |
| **Fatal** | Kritik hatalar | Application crash |

---

## 🔒 .gitignore Güncelle

```gitignore
# Logs
Logs/
*.log
*.txt
```

---

## 🐳 Docker Compose Güncelle

```yaml
# docker-compose.yml
services:
  wixi-backend:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5045:5045"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    volumes:
      - ./logs:/app/logs  # Log volume
    restart: unless-stopped

  seq:
    image: datalust/seq:latest
    ports:
      - "5341:80"
    environment:
      - ACCEPT_EULA=Y
    volumes:
      - seq-data:/data
    restart: unless-stopped

volumes:
  seq-data:
```

---

## ✅ Checklist

- [ ] NuGet paketleri yüklendi
- [ ] appsettings.json güncellendi
- [ ] Program.cs güncellendi
- [ ] LoggingExtension.cs oluşturuldu
- [ ] Controller'lara ILogger eklendi
- [ ] .gitignore güncellendi
- [ ] Logs/ klasörü oluşturuldu
- [ ] Seq kuruldu (opsiyonel)
- [ ] Docker compose güncellendi
- [ ] Test edildi

---

## 🚀 Sonraki Adımlar

- [ ] **Error Handling** - Global exception middleware
- [ ] **Performance Logging** - Yavaş istekleri logla
- [ ] **Audit Logging** - User activity tracking
- [ ] **ELK Stack** - Production log aggregation

---

**Tahmini Süre:** 2-3 saat  
**Zorluk:** Kolay  
**Öncelik:** 🔴 KRİTİK


