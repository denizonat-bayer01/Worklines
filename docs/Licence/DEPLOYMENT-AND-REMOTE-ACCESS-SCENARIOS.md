# 🚀 Deployment ve Uzaktan Erişim Senaryoları

**Multi-Tenant Sistem için Profesyonel Dağıtım ve Erişim Stratejileri**

---

## 📋 İçindekiler

1. [Genel Bakış](#genel-bakış)
2. [Mevcut Modüller](#mevcut-modüller)
3. [Deployment Senaryoları](#deployment-senaryoları)
4. [Uzaktan Erişim Modelleri](#uzaktan-erisim-modelleri)
5. [Modül Bazlı Lisanslama](#modül-bazlı-lisanslama)
6. [Güvenlik ve Erişim Kontrolü](#güvenlik-ve-erisim-kontrolü)
7. [Profesyonel Uygulama Örnekleri](#profesyonel-uygulama-örnekleri)
8. [Teknik Detaylar](#teknik-detaylar)

---

## 🎯 Genel Bakış

Bu doküman, **multi-tenant sistemin** farklı deployment senaryolarını ve uzaktan erişim modellerini detaylandırır. Sistem, müşterilerin ihtiyaçlarına göre farklı şekillerde dağıtılabilir ve erişilebilir.

### Temel Prensipler

- ✅ **Modüler Yapı**: Her modül bağımsız olarak aktif/pasif edilebilir
- ✅ **Esnek Deployment**: SaaS, On-Premise, Hybrid modelleri destekler
- ✅ **Güvenli Erişim**: VPN, API Gateway, mTLS gibi profesyonel yöntemler
- ✅ **Lisans Kontrolü**: Modül bazlı feature flag sistemi
- ✅ **Ölçeklenebilirlik**: 10'dan 1000+ tenant'a kadar destek

---

## 📦 Mevcut Modüller

### Core Modüller (Zorunlu)
| Modül | Açıklama | Durum |
|-------|----------|-------|
| **Identity** | Kimlik doğrulama, yetkilendirme, kullanıcı yönetimi | ✅ Aktif |
| **Core** | Temel utilities, exceptions, helpers | ✅ Aktif |

### Business Modülleri (Opsiyonel)
| Modül | Açıklama | Durum | Lisans Tipi |
|-------|----------|-------|------------|
| **Clients** | Müşteri yönetimi, eğitim bilgileri | ✅ Aktif | Basic+ |
| **Documents** | Belge yönetimi, dosya yükleme, inceleme | ✅ Aktif | Basic+ |
| **Applications** | Başvuru takibi, adım yönetimi | ✅ Aktif | Premium |
| **Support** | Destek sistemi, ticket yönetimi | ✅ Aktif | Basic+ |
| **Appointments** | Randevu sistemi, takvim entegrasyonu | ✅ Aktif | Premium |
| **Payments** | Ödeme işlemleri, iyzico entegrasyonu | ✅ Aktif | Enterprise |

### Content Modülleri (Opsiyonel)
| Modül | Açıklama | Durum | Lisans Tipi |
|-------|----------|-------|------------|
| **Content** | Haberler, ekip üyeleri, çeviriler | ✅ Aktif | Basic+ |
| **Forms** | Form gönderimleri, iletişim formları | ✅ Aktif | Basic |

### Infrastructure Modülleri (Opsiyonel)
| Modül | Açıklama | Durum | Lisans Tipi |
|-------|----------|-------|------------|
| **Email** | Email servisi, template yönetimi | ✅ Aktif | Basic+ |

**Toplam:** 11 Modül (2 Core + 9 Business/Content/Infrastructure)

---

## 🏗️ Deployment Senaryoları

### Senaryo 1: SaaS Modeli (Cloud-Based) ☁️

**Açıklama:** Tüm modüller merkezi sunucuda çalışır, müşteriler internet üzerinden erişir.

#### Mimari
```
[Müşteri Makinesi]
    ↓ HTTPS/WSS
[Internet]
    ↓
[API Gateway / Load Balancer]
    ↓
[Merkezi API Sunucusu]
    ├── Identity Module
    ├── Clients Module
    ├── Documents Module
    ├── Applications Module
    ├── Support Module
    ├── Appointments Module
    ├── Payments Module
    ├── Content Module
    ├── Forms Module
    └── Email Module
    ↓
[SQL Server Cluster]
    ├── DB_FIRMA_001
    ├── DB_FIRMA_002
    └── DB_FIRMA_003
```

#### Avantajlar
- ✅ Tek merkezden yönetim
- ✅ Otomatik güncellemeler
- ✅ Yüksek ölçeklenebilirlik
- ✅ Düşük bakım maliyeti
- ✅ Merkezi monitoring ve logging

#### Dezavantajlar
- ❌ İnternet bağımlılığı
- ❌ Veri merkezi dışında
- ❌ Özel güvenlik gereksinimleri

#### Kullanım Senaryoları
- Küçük-orta ölçekli firmalar
- Hızlı kurulum isteyen müşteriler
- Standart modül seti yeterli olanlar

#### Teknik Detaylar
```csharp
// appsettings.json (Merkezi Sunucu)
{
  "Deployment": {
    "Mode": "SaaS",
    "ApiBaseUrl": "https://api.worklines.de",
    "FrontendUrl": "https://worklines.wixisoftware.com",
    "DatabaseMode": "MultiTenant",
    "ConnectionPooling": true
  },
  "Modules": {
    "Identity": { "Enabled": true, "Required": true },
    "Clients": { "Enabled": true, "LicenseRequired": "Basic" },
    "Documents": { "Enabled": true, "LicenseRequired": "Basic" },
    "Applications": { "Enabled": true, "LicenseRequired": "Premium" },
    "Support": { "Enabled": true, "LicenseRequired": "Basic" },
    "Appointments": { "Enabled": true, "LicenseRequired": "Premium" },
    "Payments": { "Enabled": true, "LicenseRequired": "Enterprise" },
    "Content": { "Enabled": true, "LicenseRequired": "Basic" },
    "Forms": { "Enabled": true, "LicenseRequired": "Basic" },
    "Email": { "Enabled": true, "LicenseRequired": "Basic" }
  }
}
```

---

### Senaryo 2: On-Premise Modeli (Müşteri Sunucusunda) 🏢

**Açıklama:** API ve veritabanı müşterinin kendi sunucusunda çalışır, frontend müşteri makinesinde.

#### Mimari
```
[Müşteri Makinesi - Frontend]
    ↓ Local Network / VPN
[Müşteri Sunucusu - API]
    ├── Identity Module
    ├── Clients Module
    ├── Documents Module
    └── [Diğer Modüller - Lisansa Göre]
    ↓
[Müşteri SQL Server]
    └── DB_FIRMA_XXX (Lokal)
```

#### Avantajlar
- ✅ Veri müşteri kontrolünde
- ✅ İnternet bağımlılığı yok (lokal network)
- ✅ Yüksek güvenlik (air-gapped)
- ✅ Özel güvenlik gereksinimlerine uyum

#### Dezavantajlar
- ❌ Yüksek kurulum maliyeti
- ❌ Manuel güncellemeler gerekli
- ❌ Müşteri IT desteği gerekli
- ❌ Ölçeklenebilirlik sınırlı

#### Kullanım Senaryoları
- Büyük kurumsal firmalar
- Yüksek güvenlik gereksinimleri
- Veri yerelleştirme zorunluluğu (GDPR, vb.)
- Özel modül kombinasyonları

#### Teknik Detaylar
```csharp
// appsettings.json (Müşteri Sunucusu)
{
  "Deployment": {
    "Mode": "OnPremise",
    "ApiBaseUrl": "https://localhost:5001",
    "FrontendUrl": "file:///C:/Worklines/frontend",
    "DatabaseMode": "SingleTenant",
    "ConnectionPooling": true,
    "LicenseServer": "https://license.wixisoftware.com/api/validate"
  },
  "Modules": {
    "Identity": { "Enabled": true, "Required": true },
    "Clients": { "Enabled": true, "LicenseKey": "CLIENTS-XXXX-XXXX" },
    "Documents": { "Enabled": true, "LicenseKey": "DOCS-XXXX-XXXX" },
    "Applications": { "Enabled": false, "LicenseKey": null },
    "Support": { "Enabled": true, "LicenseKey": "SUPPORT-XXXX-XXXX" },
    "Appointments": { "Enabled": false, "LicenseKey": null },
    "Payments": { "Enabled": false, "LicenseKey": null },
    "Content": { "Enabled": true, "LicenseKey": "CONTENT-XXXX-XXXX" },
    "Forms": { "Enabled": true, "LicenseKey": "FORMS-XXXX-XXXX" },
    "Email": { "Enabled": true, "LicenseKey": "EMAIL-XXXX-XXXX" }
  },
  "License": {
    "ServerUrl": "https://license.wixisoftware.com",
    "ValidationInterval": "24:00:00",
    "OfflineMode": true,
    "OfflineGracePeriod": "30:00:00:00"
  }
}
```

---

### Senaryo 3: Hybrid Modeli (Karma) 🔄

**Açıklama:** Bazı modüller cloud'da, bazıları on-premise'da çalışır.

#### Mimari
```
[Müşteri Makinesi - Frontend]
    ↓
[API Gateway]
    ├──→ [Cloud API] (Public Modüller)
    │   ├── Identity Module
    │   ├── Content Module
    │   └── Forms Module
    │
    └──→ [On-Premise API] (Sensitive Modüller)
        ├── Clients Module
        ├── Documents Module
        ├── Applications Module
        └── Payments Module
```

#### Avantajlar
- ✅ Esneklik (modül bazlı dağıtım)
- ✅ Güvenlik (sensitive data lokal)
- ✅ Performans (public modüller cloud'da)
- ✅ Maliyet optimizasyonu

#### Dezavantajlar
- ❌ Karmaşık yapılandırma
- ❌ İki sistem senkronizasyonu
- ❌ Yüksek bakım maliyeti

#### Kullanım Senaryoları
- Büyük kurumsal firmalar
- Karma güvenlik gereksinimleri
- Modül bazlı özelleştirme ihtiyacı

#### Teknik Detaylar
```csharp
// appsettings.json (Hybrid)
{
  "Deployment": {
    "Mode": "Hybrid",
    "CloudApiUrl": "https://api.worklines.de",
    "OnPremiseApiUrl": "https://internal.worklines.local",
    "ApiGatewayUrl": "https://gateway.worklines.de"
  },
  "Modules": {
    "Identity": { 
      "Enabled": true, 
      "Location": "Cloud",
      "Endpoint": "https://api.worklines.de/api/identity"
    },
    "Clients": { 
      "Enabled": true, 
      "Location": "OnPremise",
      "Endpoint": "https://internal.worklines.local/api/clients"
    },
    "Documents": { 
      "Enabled": true, 
      "Location": "OnPremise",
      "Endpoint": "https://internal.worklines.local/api/documents"
    },
    "Content": { 
      "Enabled": true, 
      "Location": "Cloud",
      "Endpoint": "https://api.worklines.de/api/content"
    }
  }
}
```

---

## 🌐 Uzaktan Erişim Modelleri

### Model 1: Direct API Access (Doğrudan API Erişimi)

**Açıklama:** Frontend doğrudan API'ye HTTPS üzerinden bağlanır.

#### Mimari
```
[Frontend] → HTTPS → [API Gateway] → [API] → [Database]
```

#### Güvenlik
- ✅ HTTPS/TLS 1.3
- ✅ JWT Authentication
- ✅ CORS Policy
- ✅ Rate Limiting
- ✅ IP Whitelisting (opsiyonel)

#### Kullanım
- SaaS modeli için ideal
- Standart web uygulamaları
- Mobil uygulamalar

#### Örnek Konfigürasyon
```json
{
  "RemoteAccess": {
    "Mode": "DirectApi",
    "Protocol": "HTTPS",
    "Port": 443,
    "Authentication": "JWT",
    "CorsOrigins": ["https://worklines.wixisoftware.com"],
    "RateLimit": {
      "Enabled": true,
      "RequestsPerMinute": 100
    }
  }
}
```

---

### Model 2: VPN Tunneling (VPN Tüneli)

**Açıklama:** Frontend VPN üzerinden güvenli bağlantı kurar.

#### Mimari
```
[Frontend] → VPN Client → [VPN Gateway] → [Internal API] → [Database]
```

#### Güvenlik
- ✅ VPN Encryption (IPSec/OpenVPN)
- ✅ Certificate-based authentication
- ✅ Network isolation
- ✅ Additional JWT layer

#### Kullanım
- On-premise deployment
- Yüksek güvenlik gereksinimleri
- Kurumsal network entegrasyonu

#### Örnek Konfigürasyon
```json
{
  "RemoteAccess": {
    "Mode": "VPN",
    "VpnType": "OpenVPN",
    "ServerAddress": "vpn.worklines.local",
    "Port": 1194,
    "CertificatePath": "/certs/client.crt",
    "KeyPath": "/certs/client.key",
    "CaPath": "/certs/ca.crt"
  }
}
```

---

### Model 3: API Gateway with mTLS (Mutual TLS)

**Açıklama:** API Gateway mutual TLS ile client authentication yapar.

#### Mimari
```
[Frontend] → mTLS → [API Gateway] → [Internal API] → [Database]
```

#### Güvenlik
- ✅ Mutual TLS (client + server certificates)
- ✅ Certificate pinning
- ✅ API Gateway rate limiting
- ✅ Request signing

#### Kullanım
- Enterprise müşteriler
- Yüksek güvenlik gereksinimleri
- B2B entegrasyonlar

#### Örnek Konfigürasyon
```json
{
  "RemoteAccess": {
    "Mode": "ApiGateway_mTLS",
    "GatewayUrl": "https://gateway.worklines.de",
    "ClientCertificate": {
      "Path": "/certs/client.pfx",
      "Password": "${CERT_PASSWORD}"
    },
    "ServerCertificateValidation": true,
    "CertificatePinning": {
      "Enabled": true,
      "PublicKeyHash": "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA="
    }
  }
}
```

---

### Model 4: Reverse Proxy with Authentication

**Açıklama:** Reverse proxy (Nginx/Traefik) authentication layer sağlar.

#### Mimari
```
[Frontend] → [Reverse Proxy] → [Auth Service] → [API] → [Database]
```

#### Güvenlik
- ✅ Reverse proxy authentication
- ✅ OAuth2/OIDC support
- ✅ Session management
- ✅ Request filtering

#### Kullanım
- SaaS modeli
- Çoklu authentication provider
- SSO entegrasyonu

#### Örnek Konfigürasyon
```yaml
# nginx.conf
server {
    listen 443 ssl;
    server_name api.worklines.de;
    
    ssl_certificate /certs/server.crt;
    ssl_certificate_key /certs/server.key;
    
    location /api/ {
        auth_request /auth;
        proxy_pass http://internal-api:5000;
        proxy_set_header X-Tenant-Id $tenant_id;
    }
    
    location = /auth {
        internal;
        proxy_pass http://auth-service:5001/validate;
    }
}
```

---

## 🔐 Modül Bazlı Lisanslama

### Lisans Tipleri

| Lisans Tipi | Açıklama | Modüller | Fiyatlandırma |
|-------------|----------|----------|---------------|
| **Basic** | Temel modüller | Identity, Clients, Documents, Support, Content, Forms, Email | €X/ay |
| **Premium** | Gelişmiş modüller | Basic + Applications, Appointments | €Y/ay |
| **Enterprise** | Tüm modüller | Premium + Payments + Custom modules | €Z/ay |
| **Custom** | Özel paket | Müşteri seçimi | Müzakere |

### Feature Flag Sistemi

```csharp
// wixi.License/Services/FeatureFlagService.cs
public interface IFeatureFlagService
{
    Task<bool> IsModuleEnabledAsync(string tenantCode, string moduleName);
    Task<List<string>> GetEnabledModulesAsync(string tenantCode);
    Task<bool> CheckFeatureAccessAsync(string tenantCode, string feature);
}

public class FeatureFlagService : IFeatureFlagService
{
    public async Task<bool> IsModuleEnabledAsync(string tenantCode, string moduleName)
    {
        var license = await GetLicenseAsync(tenantCode);
        return license.Modules.Contains(moduleName) && license.IsActive;
    }
    
    public async Task<List<string>> GetEnabledModulesAsync(string tenantCode)
    {
        var license = await GetLicenseAsync(tenantCode);
        return license.Modules.Where(m => license.IsModuleActive(m)).ToList();
    }
}
```

### Modül Aktivasyon/Deaktivasyon

```csharp
// wixi.WebAPI/Middleware/ModuleAccessMiddleware.cs
public class ModuleAccessMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IFeatureFlagService _featureFlags;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var tenantCode = context.User.FindFirst("customerCode")?.Value;
        var moduleName = GetModuleFromRoute(context.Request.Path);
        
        if (!await _featureFlags.IsModuleEnabledAsync(tenantCode, moduleName))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    error = "ModuleNotLicensed",
                    message = $"Module '{moduleName}' is not licensed for this tenant",
                    module = moduleName
                })
            );
            return;
        }
        
        await _next(context);
    }
}
```

---

## 🛡️ Güvenlik ve Erişim Kontrolü

### 1. Tenant İzolasyonu

```csharp
// Her request'te tenant kontrolü
public class TenantIsolationMiddleware
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        // JWT'den tenant bilgisi al
        var tenantCode = context.User.FindFirst("customerCode")?.Value;
        
        if (string.IsNullOrEmpty(tenantCode))
        {
            context.Response.StatusCode = 401;
            return;
        }
        
        // Tenant aktif mi?
        var tenant = await _tenantService.GetByCodeAsync(tenantCode);
        if (tenant == null || !tenant.IsActive)
        {
            context.Response.StatusCode = 403;
            return;
        }
        
        // License geçerli mi?
        var license = await _licenseService.GetLicenseAsync(tenantCode);
        if (license == null || license.IsExpired)
        {
            context.Response.StatusCode = 402; // Payment Required
            return;
        }
        
        // HttpContext'e tenant bilgisi ekle
        context.Items["Tenant"] = tenant;
        context.Items["License"] = license;
        
        await _next(context);
    }
}
```

### 2. Modül Erişim Kontrolü

```csharp
// Attribute-based module access control
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireModuleAttribute : AuthorizeAttribute
{
    public string ModuleName { get; }
    
    public RequireModuleAttribute(string moduleName)
    {
        ModuleName = moduleName;
        Policy = $"RequireModule:{moduleName}";
    }
}

// Usage
[RequireModule("Payments")]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    // ...
}
```

### 3. API Rate Limiting per Tenant

```csharp
// Tenant bazlı rate limiting
public class TenantRateLimitMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var tenantCode = context.User.FindFirst("customerCode")?.Value;
        var license = await _licenseService.GetLicenseAsync(tenantCode);
        
        // License tipine göre rate limit
        var rateLimit = license.LicenseType switch
        {
            "Basic" => 100,      // 100 req/min
            "Premium" => 500,   // 500 req/min
            "Enterprise" => 2000, // 2000 req/min
            _ => 50
        };
        
        // Rate limit kontrolü
        if (await _rateLimiter.IsRateLimitedAsync(tenantCode, rateLimit))
        {
            context.Response.StatusCode = 429;
            return;
        }
        
        await _next(context);
    }
}
```

---

## 💼 Profesyonel Uygulama Örnekleri

### Örnek 1: SaaS Deployment (Küçük Firma)

**Müşteri:** ABC Consulting GmbH  
**Lisans:** Basic  
**Modüller:** Identity, Clients, Documents, Support, Content, Forms, Email

**Kurulum:**
1. Müşteri kaydı → Tenant oluşturulur
2. Database oluşturulur: `DB_ABC_001`
3. Modüller aktif edilir
4. Frontend build → Müşteri makinesine kurulur
5. API URL: `https://api.worklines.de`
6. Erişim: HTTPS (Direct API)

**Konfigürasyon:**
```json
{
  "Tenant": {
    "Code": "ABC_001",
    "Name": "ABC Consulting GmbH",
    "License": "Basic",
    "Modules": ["Identity", "Clients", "Documents", "Support", "Content", "Forms", "Email"]
  },
  "Frontend": {
    "ApiUrl": "https://api.worklines.de",
    "InstallPath": "C:\\Worklines"
  }
}
```

---

### Örnek 2: On-Premise Deployment (Büyük Firma)

**Müşteri:** XYZ Corporation  
**Lisans:** Enterprise (Custom)  
**Modüller:** Tüm modüller + Custom module

**Kurulum:**
1. Müşteri sunucusuna API kurulumu
2. SQL Server lokal kurulumu
3. Database oluşturulur: `DB_XYZ_001`
4. Modüller lisans key'leri ile aktif edilir
5. Frontend → Müşteri makinesine kurulur
6. VPN yapılandırması
7. License server bağlantısı (offline mode destekli)

**Konfigürasyon:**
```json
{
  "Tenant": {
    "Code": "XYZ_001",
    "Name": "XYZ Corporation",
    "License": "Enterprise",
    "Modules": ["Identity", "Clients", "Documents", "Applications", "Support", 
                "Appointments", "Payments", "Content", "Forms", "Email", "CustomModule"]
  },
  "Deployment": {
    "Mode": "OnPremise",
    "ApiUrl": "https://internal.xyz.local:5001",
    "Database": "Server=localhost;Database=DB_XYZ_001;..."
  },
  "RemoteAccess": {
    "Mode": "VPN",
    "VpnServer": "vpn.xyz.local"
  },
  "License": {
    "ServerUrl": "https://license.wixisoftware.com",
    "OfflineMode": true,
    "ValidationInterval": "24:00:00"
  }
}
```

---

### Örnek 3: Hybrid Deployment (Kurumsal)

**Müşteri:** DEF Industries  
**Lisans:** Premium (Hybrid)  
**Cloud Modüller:** Identity, Content, Forms  
**On-Premise Modüller:** Clients, Documents, Applications, Payments

**Kurulum:**
1. Cloud API → Identity, Content, Forms modülleri
2. On-Premise API → Clients, Documents, Applications, Payments
3. API Gateway → Request routing
4. Frontend → Her iki API'ye bağlanır

**Konfigürasyon:**
```json
{
  "Tenant": {
    "Code": "DEF_001",
    "Name": "DEF Industries",
    "License": "Premium",
    "DeploymentMode": "Hybrid"
  },
  "Modules": {
    "Cloud": {
      "Url": "https://api.worklines.de",
      "Modules": ["Identity", "Content", "Forms"]
    },
    "OnPremise": {
      "Url": "https://internal.def.local:5001",
      "Modules": ["Clients", "Documents", "Applications", "Payments"]
    }
  },
  "ApiGateway": {
    "Url": "https://gateway.worklines.de",
    "Routing": {
      "/api/identity": "cloud",
      "/api/content": "cloud",
      "/api/forms": "cloud",
      "/api/clients": "onpremise",
      "/api/documents": "onpremise",
      "/api/applications": "onpremise",
      "/api/payments": "onpremise"
    }
  }
}
```

---

## 🔧 Teknik Detaylar

### 1. Modül Yükleme Stratejisi

```csharp
// wixi.WebAPI/Extensions/ModuleExtensions.cs
public static class ModuleExtensions
{
    public static IServiceCollection AddModule<TModule>(
        this IServiceCollection services,
        IConfiguration configuration,
        string moduleName) where TModule : class, IModule
    {
        // Module enabled kontrolü
        var isEnabled = configuration.GetValue<bool>($"Modules:{moduleName}:Enabled");
        if (!isEnabled)
        {
            return services; // Module pasif, servis ekleme
        }
        
        // License kontrolü
        var licenseRequired = configuration.GetValue<string>($"Modules:{moduleName}:LicenseRequired");
        // License validation logic...
        
        // Module servislerini ekle
        services.AddScoped<TModule>();
        services.AddScoped<IModule, TModule>();
        
        return services;
    }
}

// Program.cs
builder.Services.AddModule<ClientsModule>(builder.Configuration, "Clients");
builder.Services.AddModule<DocumentsModule>(builder.Configuration, "Documents");
// ...
```

### 2. Dynamic Route Registration

```csharp
// Sadece aktif modüller için route'ları register et
public static class ModuleRouteExtensions
{
    public static void MapModuleRoutes(this WebApplication app, IConfiguration configuration)
    {
        var modules = configuration.GetSection("Modules").GetChildren();
        
        foreach (var module in modules)
        {
            var isEnabled = module.GetValue<bool>("Enabled");
            if (!isEnabled) continue;
            
            var moduleName = module.Key;
            
            // Module controller'larını map et
            switch (moduleName)
            {
                case "Clients":
                    app.MapControllers().WithMetadata(new RequireModuleAttribute("Clients"));
                    break;
                case "Payments":
                    app.MapControllers().WithMetadata(new RequireModuleAttribute("Payments"));
                    break;
                // ...
            }
        }
    }
}
```

### 3. License Validation Service

```csharp
// wixi.License/Services/LicenseValidationService.cs
public class LicenseValidationService
{
    public async Task<LicenseValidationResult> ValidateLicenseAsync(
        string tenantCode, 
        string moduleName)
    {
        var license = await _licenseRepository.GetByTenantCodeAsync(tenantCode);
        
        if (license == null)
            return LicenseValidationResult.NotFound;
        
        if (license.IsExpired)
            return LicenseValidationResult.Expired;
        
        if (!license.Modules.Contains(moduleName))
            return LicenseValidationResult.ModuleNotLicensed;
        
        if (!license.IsActive)
            return LicenseValidationResult.Inactive;
        
        return LicenseValidationResult.Valid;
    }
}
```

### 4. Frontend Module Detection

```typescript
// Frontend: src/config/modules.ts
export interface ModuleConfig {
  name: string;
  enabled: boolean;
  licenseRequired?: string;
  endpoints: string[];
}

export const MODULES: ModuleConfig[] = [
  {
    name: 'Clients',
    enabled: true,
    licenseRequired: 'Basic',
    endpoints: ['/api/clients']
  },
  {
    name: 'Payments',
    enabled: true,
    licenseRequired: 'Enterprise',
    endpoints: ['/api/payments']
  }
  // ...
];

// Module availability check
export async function checkModuleAvailability(moduleName: string): Promise<boolean> {
  const module = MODULES.find(m => m.name === moduleName);
  if (!module || !module.enabled) return false;
  
  // API'den license kontrolü
  const response = await fetch('/api/license/modules');
  const enabledModules = await response.json();
  
  return enabledModules.includes(moduleName);
}
```

---

## 📊 Deployment Karar Matrisi

| Kriter | SaaS | On-Premise | Hybrid |
|--------|------|------------|--------|
| **Kurulum Süresi** | 1 gün | 1-2 hafta | 1 hafta |
| **Maliyet** | Düşük | Yüksek | Orta |
| **Güvenlik** | Orta | Yüksek | Yüksek |
| **Bakım** | Otomatik | Manuel | Karma |
| **Ölçeklenebilirlik** | Yüksek | Orta | Yüksek |
| **Özelleştirme** | Sınırlı | Yüksek | Yüksek |
| **İnternet Bağımlılığı** | Var | Yok | Kısmi |

---

## ✅ Checklist: Deployment Hazırlığı

### SaaS Deployment
- [ ] API sunucusu hazır (cloud)
- [ ] Database cluster yapılandırıldı
- [ ] Load balancer yapılandırıldı
- [ ] SSL sertifikaları kuruldu
- [ ] Monitoring ve logging aktif
- [ ] Backup stratejisi hazır
- [ ] Disaster recovery planı

### On-Premise Deployment
- [ ] Müşteri sunucusu hazır
- [ ] SQL Server kuruldu
- [ ] API deployment package hazır
- [ ] Frontend build package hazır
- [ ] VPN yapılandırması (gerekirse)
- [ ] License server bağlantısı test edildi
- [ ] Offline mode test edildi
- [ ] Kurulum dokümantasyonu hazır

### Hybrid Deployment
- [ ] Cloud API hazır
- [ ] On-premise API hazır
- [ ] API Gateway yapılandırıldı
- [ ] Route yapılandırması test edildi
- [ ] Authentication flow test edildi
- [ ] Data sync mekanizması (gerekirse)

---

## 📚 Ek Kaynaklar

- [Multi-Tenant Architecture Guide](./MULTI-TENANT-ANALYSIS-AND-RECOMMENDATIONS.md)
- [Module Licensing Strategy](./test.md)
- [Security Best Practices](../MONOLITH-SECURITY-ANALYSIS.md)

---

**Son Güncelleme:** 2024  
**Hazırlayan:** AI Assistant  
**Durum:** Production Ready

