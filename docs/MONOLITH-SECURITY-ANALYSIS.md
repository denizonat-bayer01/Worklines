# 🔐 Monolith Güvenlik Analizi - Hangi Sorunlar Geçerli?

## 🎯 Genel Bakış

Bu doküman, MODULARIZATION-SECURITY-ANALYSIS.md'de belirtilen güvenlik sorunlarının **mevcut monolith yapıda** geçerli olup olmadığını analiz eder.

---

## 📊 Sorunların Geçerlilik Analizi

### ✅ Monolith İçin Geçerli Olan Sorunlar

#### 1. ✅ Shared DbContext Güvenlik Açığı

**Durum:** 🔴 **MONOLITH'TE ZATEN VAR**

**Açıklama:**
```csharp
// ŞU ANKİ MONOLITH YAPISI
public class WixiDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    // Tüm entity'ler aynı DbContext'te
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Application> Applications { get; set; }
    // ... tüm entity'ler
}
```

**Riskler (Monolith'te de geçerli):**
- ❌ **Privilege Escalation**: Bir service'teki SQL injection, tüm database'e erişim sağlayabilir
- ❌ **Data Leakage**: Yanlış yapılandırılmış query'ler, diğer service'lerin verilerine erişebilir
- ❌ **Audit Trail Zorluğu**: Hangi service'in hangi veriyi değiştirdiğini takip etmek zor

**Çözüm (Monolith için):**
```csharp
// 1. Row-Level Security (RLS) - SQL Server'da
CREATE SECURITY POLICY ClientSecurityPolicy
    ADD FILTER PREDICATE dbo.fn_security_predicate(ClientId)
    ON dbo.Clients;

// 2. Repository Pattern ile erişim kontrolü
public class ClientRepository
{
    public async Task<Client> GetByIdAsync(int id, int userId)
    {
        // Kullanıcı sadece kendi client'larına erişebilir
        return await _context.Clients
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();
    }
}

// 3. Audit Logging
public class AuditService
{
    public async Task LogEntityChangeAsync<T>(T entity, string action, string userId)
    {
        await _auditLogService.LogAsync(new AuditLog
        {
            EntityType = typeof(T).Name,
            EntityId = GetEntityId(entity),
            Action = action,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

**Öncelik:** 🔴 **YÜKSEK** (Monolith'te de çözülmeli)

---

#### 2. ✅ Authorization Granularity

**Durum:** 🔴 **MONOLITH'TE ZATEN VAR**

**Açıklama:**
```csharp
// ŞU ANKİ MONOLITH YAPISI
[Authorize(Roles = "Admin")]
public class AdminDocumentReviewController : ControllerBase
{
    // Tüm admin'ler tüm document'leri görebilir
    // Resource-based authorization yok
}
```

**Riskler (Monolith'te de geçerli):**
- ❌ **Over-Privileged Access**: Admin rolü, tüm kaynaklara erişim sağlıyor
- ❌ **Fine-Grained Authorization Yok**: Entity-level authorization yok
- ❌ **Resource-Based Authorization Eksik**: Kullanıcı sadece kendi kaynaklarına erişebilmeli

**Çözüm (Monolith için):**
```csharp
// 1. Policy-Based Authorization
services.AddAuthorization(options =>
{
    options.AddPolicy("Documents.Read", policy =>
        policy.RequireRole("Admin", "Client"));
    
    options.AddPolicy("Documents.Write", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("OwnDocumentsOnly", policy =>
        policy.Requirements.Add(new OwnResourceRequirement()));
});

// 2. Resource-Based Authorization Handler
public class DocumentAuthorizationHandler : 
    AuthorizationHandler<OwnResourceRequirement, Document>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnResourceRequirement requirement,
        Document resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Kullanıcı sadece kendi document'lerine erişebilir
        if (resource.ClientId.ToString() == userId)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}

// 3. Controller'da kullanım
[Authorize(Policy = "Documents.Read")]
public class DocumentsController : ControllerBase
{
    [Authorize(Policy = "OwnDocumentsOnly")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDocument(int id)
    {
        var document = await _documentService.GetByIdAsync(id);
        // Authorization handler otomatik çalışır
        return Ok(document);
    }
}
```

**Öncelik:** 🔴 **YÜKSEK** (Monolith'te de çözülmeli)

---

#### 3. ✅ Secret Management

**Durum:** 🔴 **MONOLITH'TE ZATEN VAR**

**Açıklama:**
```json
// ŞU ANKİ MONOLITH YAPISI
// appsettings.json
{
  "JwtTokenOptions": {
    "SecurityKey": "hardcoded-secret-key" // ❌ Git'te görülebilir
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WixiDb;Password=hardcoded-password;" // ❌
  }
}
```

**Riskler (Monolith'te de geçerli):**
- ❌ **Secret Leakage**: Git repository'de secret'lar görülebilir
- ❌ **No Secret Rotation**: Secret'lar düzenli olarak değiştirilmiyor
- ❌ **No Environment Separation**: Development ve production secret'ları aynı yerde

**Çözüm (Monolith için):**
```csharp
// 1. Environment Variables kullan
// appsettings.json'dan secret'ları kaldır
// Docker/Kubernetes environment variables kullan

// Docker Compose
environment:
  - JwtTokenOptions__SecurityKey=${JWT_SECRET_KEY}
  - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}

// 2. Azure Key Vault (Production için)
public class SecretManager
{
    public async Task<string> GetSecretAsync(string secretName)
    {
        // Azure Key Vault'tan secret'ı al
        var secret = await _keyVaultClient.GetSecretAsync(secretName);
        return secret.Value;
    }
}

// 3. User Secrets (Development için)
// dotnet user-secrets set "JwtTokenOptions:SecurityKey" "secret-key"
```

**Öncelik:** 🔴 **YÜKSEK** (Monolith'te de çözülmeli)

---

#### 4. ✅ Audit Logging

**Durum:** 🔴 **MONOLITH'TE ZATEN VAR (EKSIK)**

**Açıklama:**
```csharp
// ŞU ANKİ MONOLITH YAPISI
// Sadece basic logging var
_logger.LogInformation("Document created: {DocumentId}", documentId);
// ❌ Güvenlik olayları loglanmıyor
// ❌ Entity değişiklikleri loglanmıyor
// ❌ Compliance logging yok
```

**Riskler (Monolith'te de geçerli):**
- ❌ **No Security Event Logging**: Güvenlik olayları loglanmıyor
- ❌ **No Audit Trail**: Kim, ne zaman, neyi değiştirdi bilinmiyor
- ❌ **No Compliance Logging**: GDPR, HIPAA gibi compliance gereksinimleri karşılanmıyor

**Çözüm (Monolith için):**
```csharp
// 1. Security Event Logging
public class SecurityEventLogger
{
    public async Task LogSecurityEventAsync(SecurityEvent securityEvent)
    {
        await _auditLogService.LogAsync(new AuditLog
        {
            EventType = securityEvent.EventType, // Login, Logout, AccessDenied
            UserId = securityEvent.UserId,
            IpAddress = securityEvent.IpAddress,
            Timestamp = DateTime.UtcNow,
            Details = securityEvent.Details
        });
    }
}

// 2. Entity Change Logging
public class AuditTrailService
{
    public async Task LogEntityChangeAsync<T>(T entity, string action, string userId)
    {
        await _auditLogService.LogAsync(new EntityAuditLog
        {
            EntityType = typeof(T).Name,
            EntityId = GetEntityId(entity),
            Action = action, // Create, Update, Delete
            UserId = userId,
            Changes = SerializeChanges(entity),
            Timestamp = DateTime.UtcNow
        });
    }
}

// 3. Controller'da kullanım
[HttpPost]
public async Task<IActionResult> CreateDocument([FromBody] DocumentCreateDto dto)
{
    var document = await _documentService.CreateAsync(dto);
    
    // Audit log
    await _auditTrailService.LogEntityChangeAsync(
        document, 
        "Create", 
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
    );
    
    return Ok(document);
}
```

**Öncelik:** 🔴 **YÜKSEK** (Monolith'te de çözülmeli)

---

### ⚠️ Monolith İçin Geçerli Olmayan Sorunlar

#### 1. ⚠️ Inter-Module Communication Güvenliği

**Durum:** 🟡 **MODÜLERLEŞTİRME SONRASI ORTAYA ÇIKACAK**

**Açıklama:**
```csharp
// ŞU ANKİ MONOLITH YAPISI
// Direkt service-to-service çağrılar var
public class DocumentService
{
    private readonly IClientService _clientService; // Direkt bağımlılık
    
    public async Task<Document> CreateDocumentAsync(DocumentCreateDto dto)
    {
        var client = await _clientService.GetByIdAsync(dto.ClientId);
        // ...
    }
}
```

**Monolith'te Risk:**
- ✅ **Düşük Risk**: Aynı process içinde, aynı memory space'te çalışıyor
- ✅ **Güvenlik Kontrolleri**: Tüm service'ler aynı authentication/authorization context'ini kullanıyor
- ⚠️ **Potansiyel Risk**: Service'ler arası güvenlik kontrolleri bypass edilebilir

**Modülerleştirme Sonrası Risk:**
- ❌ **Yüksek Risk**: Modüller farklı assembly'lerde, farklı namespace'lerde
- ❌ **Security Bypass**: Bir modül, diğer modülün güvenlik kontrollerini bypass edebilir
- ❌ **Token Sharing**: JWT token'lar modüller arasında paylaşılıyor

**Çözüm (Monolith için - Şimdilik gerekli değil):**
```csharp
// Monolith'te direkt service çağrıları yeterli
// Modülerleştirme sonrası event-driven communication'a geçilebilir
```

**Öncelik:** 🟡 **ORTA** (Modülerleştirme sonrası çözülmeli)

---

### 🔴 Kritik Eksiklikler (Monolith İçin Geçerli)

#### 1. 🔴 Rate Limiting

**Durum:** ❌ **MONOLITH'TE YOK**

**Risk:**
- ❌ DDoS saldırılarına karşı korunmasız
- ❌ Brute force saldırılarına karşı korunmasız
- ❌ API abuse'e karşı korunmasız

**Çözüm (Monolith için):**
```csharp
// AspNetCoreRateLimit kullan
services.AddMemoryCache();
services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "/api/auth/login",
            Limit = 5,
            Period = "1m"
        }
    };
});
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
```

**Öncelik:** 🔴 **YÜKSEK**

---

#### 2. 🔴 Security Headers

**Durum:** ❌ **MONOLITH'TE YOK**

**Risk:**
- ❌ XSS saldırılarına karşı korunmasız
- ❌ Clickjacking saldırılarına karşı korunmasız
- ❌ MIME type sniffing saldırılarına karşı korunmasız

**Çözüm (Monolith için):**
```csharp
// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

**Öncelik:** 🔴 **YÜKSEK**

---

#### 3. 🟡 API Versioning

**Durum:** ❌ **MONOLITH'TE YOK**

**Risk:**
- ❌ Backward compatibility sorunları
- ❌ API breaking changes
- ❌ Client uyumluluk sorunları

**Çözüm (Monolith için):**
```csharp
// API versioning
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Controller'da
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class DocumentsController : ControllerBase
{
}
```

**Öncelik:** 🟡 **ORTA**

---

#### 4. 🟡 Request Size Limits

**Durum:** ❌ **MONOLITH'TE YOK**

**Risk:**
- ❌ Dosya upload saldırılarına karşı korunmasız
- ❌ Memory exhaustion saldırılarına karşı korunmasız

**Çözüm (Monolith için):**
```csharp
// Request size limits
services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10 MB
    options.ValueLengthLimit = 10485760;
    options.MemoryBufferThreshold = Int32.MaxValue;
});

// Controller'da
[RequestSizeLimit(10485760)] // 10 MB
[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file)
{
}
```

**Öncelik:** 🟡 **ORTA**

---

#### 5. 🟡 IP Whitelisting

**Durum:** ❌ **MONOLITH'TE YOK**

**Risk:**
- ❌ Admin panel'e her yerden erişilebilir
- ❌ Sensitive endpoint'lere her yerden erişilebilir

**Çözüm (Monolith için):**
```csharp
// IP whitelisting middleware
public class IpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var allowedIps = _configuration.GetSection("AllowedIPs").Get<string[]>();
        
        if (context.Request.Path.StartsWithSegments("/api/admin") &&
            !allowedIps.Contains(ipAddress))
        {
            context.Response.StatusCode = 403;
            return;
        }
        
        await _next(context);
    }
}
```

**Öncelik:** 🟡 **ORTA**

---

#### 6. 🟡 Token Refresh Mechanism

**Durum:** ⚠️ **MONOLITH'TE KISMEN VAR**

**Risk:**
- ❌ Token yenileme mekanizması eksik
- ❌ Kullanıcılar sık sık login olmak zorunda

**Çözüm (Monolith için):**
```csharp
// Token refresh endpoint
[HttpPost("refresh")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
{
    var principal = GetPrincipalFromExpiredToken(request.Token);
    var username = principal.Identity.Name;
    var user = await _userManager.FindByNameAsync(username);
    
    if (user == null || user.RefreshToken != request.RefreshToken ||
        user.RefreshTokenEndDate <= DateTime.UtcNow)
    {
        return BadRequest("Invalid refresh token");
    }
    
    var newToken = await _tokenHelper.CreateTokenAsync(user);
    var newRefreshToken = GenerateRefreshToken();
    
    user.RefreshToken = newRefreshToken;
    user.RefreshTokenEndDate = DateTime.UtcNow.AddDays(7);
    await _userManager.UpdateAsync(user);
    
    return Ok(new { token = newToken, refreshToken = newRefreshToken });
}
```

**Öncelik:** 🟡 **ORTA**

---

## 📊 Özet Tablo

| Sorun | Monolith'te Geçerli mi? | Öncelik | Durum |
|-------|------------------------|---------|-------|
| **Shared DbContext Güvenlik Açığı** | ✅ **EVET** | 🔴 Yüksek | ❌ Çözülmeli |
| **Authorization Granularity** | ✅ **EVET** | 🔴 Yüksek | ❌ Çözülmeli |
| **Secret Management** | ✅ **EVET** | 🔴 Yüksek | ❌ Çözülmeli |
| **Audit Logging** | ✅ **EVET** | 🔴 Yüksek | ❌ Çözülmeli |
| **Inter-Module Communication** | ⚠️ **HAYIR** (Modülerleştirme sonrası) | 🟡 Orta | ⏳ Modülerleştirme sonrası |
| **Rate Limiting** | ✅ **EVET** | 🔴 Yüksek | ❌ Çözülmeli |
| **Security Headers** | ✅ **EVET** | 🔴 Yüksek | ❌ Çözülmeli |
| **API Versioning** | ✅ **EVET** | 🟡 Orta | ❌ Çözülmeli |
| **Request Size Limits** | ✅ **EVET** | 🟡 Orta | ❌ Çözülmeli |
| **IP Whitelisting** | ✅ **EVET** | 🟡 Orta | ❌ Çözülmeli |
| **Token Refresh** | ✅ **EVET** (Kısmen) | 🟡 Orta | ⏳ İyileştirilmeli |

---

## 🎯 Sonuç ve Öneriler

### ✅ Monolith İçin Geçerli Olan Sorunlar

**Toplam:** 10 sorundan **9'u monolith için geçerli**

**Yüksek Öncelik (Hemen çözülmeli):**
1. 🔴 **Rate Limiting** - DDoS koruması
2. 🔴 **Security Headers** - XSS, Clickjacking koruması
3. 🔴 **Audit Logging** - Güvenlik olayları loglama
4. 🔴 **Shared DbContext Güvenlik** - Row-Level Security
5. 🔴 **Authorization Granularity** - Policy-based authorization
6. 🔴 **Secret Management** - Environment variables / Key Vault

**Orta Öncelik:**
7. 🟡 **API Versioning** - Backward compatibility
8. 🟡 **Request Size Limits** - Dosya upload koruması
9. 🟡 **IP Whitelisting** - Admin panel koruması
10. 🟡 **Token Refresh** - Token yenileme mekanizması

### ⚠️ Monolith İçin Geçerli Olmayan Sorunlar

**Sadece 1 sorun:**
- ⚠️ **Inter-Module Communication Güvenliği** - Modülerleştirme sonrası ortaya çıkacak

---

## 🚀 Hemen Yapılması Gerekenler (Monolith İçin)

### Phase 1: Kritik Güvenlik (1-2 Hafta)

1. ✅ **Rate Limiting** ekle
2. ✅ **Security Headers** ekle
3. ✅ **Audit Logging** ekle

### Phase 2: Veri Güvenliği (2-3 Hafta)

4. ✅ **Row-Level Security** ekle (SQL Server)
5. ✅ **Policy-Based Authorization** ekle
6. ✅ **Secret Management** düzelt (Environment variables)

### Phase 3: İyileştirmeler (1-2 Hafta)

7. ✅ **API Versioning** ekle
8. ✅ **Request Size Limits** ekle
9. ✅ **IP Whitelisting** ekle (Admin panel için)
10. ✅ **Token Refresh** mekanizması ekle

**Toplam Süre:** 4-7 hafta

---

## 📚 İlgili Dokümanlar

- **[MODULARIZATION-SECURITY-ANALYSIS.md](MODULARIZATION-SECURITY-ANALYSIS.md)** - Modülerleştirme sonrası güvenlik analizi
- **[INDEX.md](INDEX.md)** - Güvenlik özellikleri ve eksiklikler
- **[04-SECURITY.md](04-SECURITY.md)** - Güvenlik implementasyon rehberi (oluşturulacak)

---

**Son Güncelleme:** Ocak 2025  
**Maintainer:** Wixi Backend Team

