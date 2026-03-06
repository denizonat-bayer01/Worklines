# Lisans Yönetim Sistemi - Analiz Dokümanı

## 1. Genel Bakış

Sistem, harici bir lisans API'si kullanarak lisans kontrolü yapacak ve lisans durumuna göre admin paneli ve public alanların erişimini yönetecektir.

## 2. Sistem Mimarisi

### 2.1. Lisans API Entegrasyonu

**Endpoint:** `https://api.wixisoftware.com/api/v1/licenses/validate`

**Request:**
```json
{
  "licenseKey": "CN9E-BCSK-4TP3-BYVN",
  "machineCode": "string",
  "clientVersion": "string"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isValid": true,
    "expireDate": "2025-12-07T00:00:00",
    "modules": [],
    "maxUser": null,
    "tenantId": 1,
    "tenantCompanyName": "Worklines Pro Consulting",
    "reason": null
  }
}
```

### 2.2. Sistem Akışı

```
┌─────────────────────────────────────────────────────────────┐
│                    Kullanıcı Sisteme Giriş Yapmaya Çalışır  │
└───────────────────────────┬─────────────────────────────────┘
                            │
                            ▼
                    ┌───────────────┐
                    │ Lisans Kontrol│
                    └───────┬───────┘
                            │
            ┌───────────────┴───────────────┐
            │                               │
            ▼                               ▼
    ┌───────────────┐              ┌───────────────┐
    │ Lisans Geçerli│              │ Lisans Geçersiz│
    └───────┬───────┘              └───────┬───────┘
            │                               │
            ▼                               ▼
    ┌───────────────┐              ┌───────────────┐
    │ Normal İşlem  │              │ Anahtar Giriş │
    │   Devamimage.png Eder  │              │    Ekranı     │
    └───────────────┘              └───────┬───────┘
                                           │
                                           ▼
                                  ┌───────────────┐
                                  │ Anahtar Girilir│
                                  └───────┬───────┘
                                          │
                                          ▼
                                  ┌───────────────┐
                                  │ API'den Kontrol│
                                  └───────┬───────┘
                                          │
                          ┌───────────────┴───────────────┐
                          │                               │
                          ▼                               ▼
                  ┌───────────────┐              ┌───────────────┐
                  │ Lisans Geçerli│              │ Lisans Geçersiz│
                  └───────┬───────┘              └───────┬───────┘
                          │                               │
                          ▼                               ▼
                  ┌───────────────┐              ┌───────────────┐
                  │ DB'ye Kaydet  │              │ Hata Göster   │
                  │ Sisteme Giriş │              │ Tekrar Dene   │
                  └───────────────┘              └───────────────┘
```

## 3. Database Yapısı

### 3.1. Yeni Tablo: `wixi_LicenseSettings`

```sql
CREATE TABLE wixi_LicenseSettings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LicenseKey NVARCHAR(100) NOT NULL UNIQUE,
    IsValid BIT NOT NULL DEFAULT 0,
    ExpireDate DATETIME2 NULL,
    TenantId INT NULL,
    TenantCompanyName NVARCHAR(200) NULL,
    MachineCode NVARCHAR(100) NULL,
    ClientVersion NVARCHAR(50) NULL,
    LastValidatedAt DATETIME2 NULL,
    ValidationResult NVARCHAR(MAX) NULL, -- JSON response
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedBy NVARCHAR(100) NULL,
    RowVersion ROWVERSION
);
```

### 3.2. Index'ler

```sql
CREATE INDEX IX_LicenseSettings_LicenseKey ON wixi_LicenseSettings(LicenseKey);
CREATE INDEX IX_LicenseSettings_IsValid ON wixi_LicenseSettings(IsValid);
```

## 4. Backend Yapısı

### 4.1. Yeni Entity: `LicenseSettings`

**Dosya:** `wixi.backendV2/wixi.Content/Entities/LicenseSettings.cs`

```csharp
public class LicenseSettings
{
    public int Id { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public DateTime? ExpireDate { get; set; }
    public int? TenantId { get; set; }
    public string? TenantCompanyName { get; set; }
    public string? MachineCode { get; set; }
    public string? ClientVersion { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public string? ValidationResult { get; set; } // JSON
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[]? RowVersion { get; set; }
}
```

### 4.2. Yeni Service: `LicenseService`

**Dosya:** `wixi.backendV2/wixi.WebAPI/Services/LicenseService.cs`

**Metodlar:**
- `ValidateLicenseAsync(string licenseKey)` - API'den lisans kontrolü
- `GetCurrentLicenseAsync()` - Aktif lisansı getir
- `SaveLicenseAsync(LicenseValidationResult result)` - Lisansı DB'ye kaydet
- `IsLicenseValidAsync()` - Lisans geçerli mi kontrol et
- `CheckLicenseExpiryAsync()` - Lisans süresi dolmuş mu kontrol et

### 4.3. Yeni Controller: `AdminLicenseController`

**Endpoint'ler:**
- `POST /api/v1.0/admin/license/validate` - Lisans anahtarı doğrula ve kaydet
- `GET /api/v1.0/admin/license/status` - Mevcut lisans durumunu getir

### 4.4. Yeni Controller: `PublicLicenseController`

**Endpoint'ler:**
- `GET /api/v1.0/license/status` - Public lisans durumu (sadece geçerli/geçersiz)

### 4.5. Middleware: `LicenseValidationMiddleware`

**Görevler:**
- Her request'te lisans durumunu kontrol et
- Admin route'ları için: Lisans geçersizse `/admin/license-key` yönlendir
- Public route'ları için: Lisans geçersizse `/maintenance` yönlendir
- Health check ve static file'lar için bypass

### 4.6. DTO'lar

**LicenseValidationRequestDto:**
```csharp
public class LicenseValidationRequestDto
{
    public string LicenseKey { get; set; } = string.Empty;
    public string? MachineCode { get; set; }
    public string? ClientVersion { get; set; }
}
```

**LicenseValidationResponseDto:**
```csharp
public class LicenseValidationResponseDto
{
    public bool Success { get; set; }
    public LicenseDataDto? Data { get; set; }
}

public class LicenseDataDto
{
    public bool IsValid { get; set; }
    public DateTime? ExpireDate { get; set; }
    public List<string>? Modules { get; set; }
    public int? MaxUser { get; set; }
    public int? TenantId { get; set; }
    public string? TenantCompanyName { get; set; }
    public string? Reason { get; set; }
}
```

**LicenseStatusDto:**
```csharp
public class LicenseStatusDto
{
    public bool IsValid { get; set; }
    public DateTime? ExpireDate { get; set; }
    public bool IsExpired { get; set; }
    public int DaysRemaining { get; set; }
    public string? TenantCompanyName { get; set; }
}
```

## 5. Frontend Yapısı

### 5.1. Yeni Sayfa: `LicenseKeyEntry.tsx`

**Lokasyon:** `V4/src/pages/Admin/LicenseKeyEntry.tsx`

**Özellikler:**
- Lisans anahtarı giriş formu
- API'den doğrulama
- Başarılı doğrulama sonrası admin paneline yönlendirme
- Hata mesajları gösterimi

### 5.2. Yeni Sayfa: `Maintenance.tsx` (Güncellenecek)

**Lokasyon:** `V4/src/pages/Maintenance.tsx`

**Özellikler:**
- Lisans süresi dolmuş mesajı
- Admin giriş linki (eğer admin ise)

### 5.3. Yeni Service: `LicenseService.ts`

**Dosya:** `V4/src/ApiServices/services/LicenseService.ts`

**Metodlar:**
- `validateLicense(licenseKey: string)` - Lisans doğrula
- `getLicenseStatus()` - Lisans durumunu getir
- `getPublicLicenseStatus()` - Public lisans durumu

### 5.4. Protected Route Güncellemeleri

**Admin Routes:**
- Lisans geçersizse `/admin/license-key` yönlendir
- Lisans geçerliyse normal işlem

**Public Routes:**
- Lisans geçersizse `/maintenance` göster
- Lisans geçerliyse normal sayfalar

### 5.5. App.tsx Güncellemeleri

- License check middleware ekle
- Route guard'lar ekle

## 6. Güvenlik Önlemleri

### 6.1. Backend
- Lisans anahtarı şifrelenmiş olarak saklanabilir (opsiyonel)
- API key'i environment variable'dan al
- Rate limiting (çok fazla istek engelleme)
- Machine code validation

### 6.2. Frontend
- Lisans anahtarı localStorage'da saklanmamalı
- Her sayfa yüklemesinde lisans kontrolü
- Session-based kontrol

## 7. Cache Stratejisi

### 7.1. Backend Cache
- Lisans durumu 5 dakika cache'lenebilir
- Expire date yaklaştığında cache süresini kısalt
- Cache invalidation: Yeni lisans girildiğinde

## 8. Hata Senaryoları

### 8.1. API Erişilemez
- Son bilinen geçerli durumu kullan
- X gün sonra tekrar dene
- Admin'e uyarı göster

### 8.2. Lisans Süresi Dolmuş
- Admin: Anahtar giriş ekranı
- Public: Maintenance sayfası

### 8.3. Geçersiz Anahtar
- Hata mesajı göster
- Tekrar deneme imkanı

## 9. İmplementasyon Adımları

### Adım 1: Database
1. `wixi_LicenseSettings` tablosunu oluştur
2. Migration oluştur
3. İlk lisans anahtarını ekle (CN9E-BCSK-4TP3-BYVN)

### Adım 2: Backend - Entity & DbContext
1. `LicenseSettings` entity oluştur
2. `WixiDbContext`'e ekle
3. Configuration ekle

### Adım 3: Backend - Service
1. `LicenseService` oluştur
2. API entegrasyonu yap
3. Database işlemleri

### Adım 4: Backend - Controllers
1. `AdminLicenseController` oluştur
2. `PublicLicenseController` oluştur
3. Endpoint'leri implement et

### Adım 5: Backend - Middleware
1. `LicenseValidationMiddleware` oluştur
2. Route kontrolü ekle
3. Bypass listesi ekle

### Adım 6: Frontend - Service
1. `LicenseService.ts` oluştur
2. API çağrıları

### Adım 7: Frontend - Pages
1. `LicenseKeyEntry.tsx` oluştur
2. `Maintenance.tsx` güncelle
3. Route'ları ekle

### Adım 8: Frontend - Route Guards
1. Admin route guard'ı ekle
2. Public route guard'ı ekle
3. App.tsx güncelle

### Adım 9: Test
1. Lisans geçerli senaryo
2. Lisans geçersiz senaryo
3. API erişilemez senaryo
4. Lisans süresi dolmuş senaryo

## 10. API Entegrasyon Detayları

### 10.1. Request Headers
```
Content-Type: application/json
Accept: application/json
```

### 10.2. Machine Code
- Server hostname veya unique identifier kullanılabilir
- Environment variable'dan alınabilir

### 10.3. Client Version
- Backend version bilgisi
- appsettings.json'dan alınabilir

### 10.4. Error Handling
- Network errors
- API errors
- Invalid response format
- Timeout handling

## 11. Performans Optimizasyonları

### 11.1. Cache
- Memory cache kullan
- 5 dakika TTL
- Expire date yaklaştığında 1 dakika

### 11.2. Background Job
- Periyodik lisans kontrolü (her 24 saat)
- Expire date yaklaştığında uyarı

### 11.3. Lazy Loading
- İlk sayfa yüklemesinde kontrol
- Sonraki sayfalarda cache'den

## 12. Monitoring & Logging

### 12.1. Log Events
- Lisans doğrulama başarılı/başarısız
- API çağrıları
- Lisans süresi doldu
- Geçersiz anahtar girişimi

### 12.2. Metrics
- API response time
- Validation success rate
- License expiry warnings

## 13. İlk Kurulum

### 13.1. Database
```sql
-- İlk lisans anahtarını ekle
INSERT INTO wixi_LicenseSettings (
    LicenseKey,
    IsValid,
    IsActive,
    CreatedAt,
    UpdatedAt
) VALUES (
    'CN9E-BCSK-4TP3-BYVN',
    0, -- İlk başta false, API'den doğrulandıktan sonra true
    1,
    GETUTCDATE(),
    GETUTCDATE()
);
```

### 13.2. Configuration
```json
{
  "License": {
    "ApiUrl": "https://api.wixisoftware.com/api/v1/licenses/validate",
    "MachineCode": "SERVER-001",
    "ClientVersion": "2.0.0",
    "CacheDurationMinutes": 5,
    "ValidationIntervalHours": 24
  }
}
```

## 14. Test Senaryoları

### Senaryo 1: İlk Kurulum
1. Sistem başlatılır
2. Lisans kontrolü yapılır (DB'de yok)
3. Admin paneline giriş yapılmaya çalışılır
4. Anahtar giriş ekranı gösterilir
5. Anahtar girilir ve doğrulanır
6. Admin paneline erişim sağlanır

### Senaryo 2: Geçerli Lisans
1. Sistem başlatılır
2. Lisans kontrolü yapılır (geçerli)
3. Normal işlemler devam eder

### Senaryo 3: Süresi Dolmuş Lisans
1. Sistem başlatılır
2. Lisans kontrolü yapılır (süresi dolmuş)
3. Admin: Anahtar giriş ekranı
4. Public: Maintenance sayfası

### Senaryo 4: API Erişilemez
1. API'ye erişilemez
2. Son bilinen geçerli durum kullanılır
3. X gün sonra tekrar denenir

## 15. Güvenlik Notları

1. **Lisans Anahtarı Güvenliği:**
   - Database'de şifrelenmiş saklanabilir
   - Log'larda görünmemeli
   - HTTPS kullanılmalı

2. **API Güvenliği:**
   - API key varsa environment variable'dan al
   - Rate limiting uygula
   - Timeout ayarla

3. **Frontend Güvenliği:**
   - Lisans anahtarı client-side'da saklanmamalı
   - Her request'te backend'den kontrol
   - XSS koruması

## 16. Gelecek Geliştirmeler

1. Multi-tenant desteği
2. Modül bazlı lisans kontrolü
3. Kullanıcı sayısı limiti
4. Lisans yenileme bildirimleri
5. Otomatik lisans yenileme

