# 🧩 Tek API ile Çoklu Veritabanı Yönetimi Mimarisi

**(Frontend Müşteri Makinesinde Çalışır, API Merkezî Sunucuda)**

------------------------------------------------------------------------

## 📌 1. Mimari Genel Bakış

Bu mimaride yalnızca **tek bir merkezi API** bulunur ve tüm iş mantığı
burada çalışır.\
Her müşterinin verisi kendi **bağımsız SQL veritabanında** tutulur. API,
gelen JWT token'dan hangi müşterinin istek attığını algılar ve doğru
veritabanına bağlanır.

Bu yapı:

-   Frontend müşterinin makinesine kurulur.
-   API tek merkezde bulunur.
-   Müşteriler arası veri izolasyonu sağlanır.
-   Güncellemeler yalnızca API üzerinde yapılır.

------------------------------------------------------------------------

## 📌 2. Sistem Bileşenleri

    [Müşteri Makinesi] → React Frontend
                    |
                    V
               İnternet / VPN
                    |
                    V
    [Merkezi Sunucu] → ASP.NET Core API
                    |
                    V
    [Müşteri Sunucusu / Lokal] → SQL Server (Her müşteri için bir DB)

------------------------------------------------------------------------

## 📌 3. Bileşenlerin Detayları

### 🖥️ 3.1. Frontend (React)

-   Müşteri makinesine kuruludur.
-   İş mantığı yoktur.
-   API'ye JWT ile istek atar.
-   Konfigürasyon dosyasında yalnızca API URL bulunur.

### 🔧 3.2. Backend (ASP.NET Core API)

Görevleri: 1. JWT token'dan müşteri kodunu okumak\
2. Doğru connection string'i seçmek\
3. Bağlantıyı oluşturup sorguları çalıştırmak\
4. Güvenlik ve lisans denetimini merkezî olarak yapmak

### 🗄️ 3.3. Veritabanı Yapısı

Her firma için ayrı veritabanı:

    DB_FIRMA_001
    DB_FIRMA_002
    DB_FIRMA_003

Tablolar izole, veri güvenliği yüksek.

------------------------------------------------------------------------

## 📌 4. Dinamik Veritabanı Yönlendirme (Multi-DB Routing)

JWT içinde:

    "customerCode": "FIRMA_001"

API tarafında:

``` csharp
var cust = user.Claims.First(x => x.Type == "customerCode").Value;
var connString = _connectionService.Get(cust);
using var conn = new SqlConnection(connString);
```

------------------------------------------------------------------------

## 📌 5. Müşteriye Kurulacak Yapı

Kurulanlar: - React build klasörü - config.json (API URL içerir) -
İsteğe bağlı bir launcher uygulaması

Kurulmayanlar: - API - Kod - Sunucu bileşenleri

------------------------------------------------------------------------

## 📌 6. Veri Akışı

1.  Kullanıcı Frontend'i açar\
2.  Login isteği API'ye gider\
3.  API JWT üretir → müşteri kodu içerir\
4.  Frontend bu JWT ile istek atar\
5.  API doğru veritabanını seçer\
6.  Sorguyu çalıştırır\
7.  Sonuç Frontend'e döner

------------------------------------------------------------------------

## 📌 7. Örnek Akış Diyagramı

    [React Frontend]
           |
           | Login → 
           |
           V
    [API - Auth] -- JWT --> [Frontend]
           |
           | Veri isteği (JWT ile)
           V
    [API - DB Router]
           |
           | customerCode → DB seç
           |
           V
    [SQL Customer DB]
           |
           | Sonuç
           V
    [API]
           |
           V
    [Frontend]

------------------------------------------------------------------------

## 📌 8. Avantajlar

### ✔ Tek API -- tek kod tabanı

### ✔ Müşteri bazlı izolasyon

### ✔ Güvenli lisanslama

### ✔ Müşteri sadece UI alır

### ✔ Bakım maliyeti düşük

### ✔ SaaS / On-Prem uyumlu

------------------------------------------------------------------------

## 📌 9. Önerilen Teknolojiler

### Frontend

-   React + Vite\
-   React Query\
-   JWT Client

### Backend

-   ASP.NET Core (.NET 8)\
-   Dapper + EF Core hibrit\
-   Multi-tenant middleware\
-   SQLConnectionFactory\
-   Global Exception

### Database

-   SQL Server\
-   Her müşteri için ayrı DB\
-   Migration API tarafından yönetilebilir

------------------------------------------------------------------------

## 📌 10. Proje Klasör Yapısı

    /src
       /frontend
       /backend
          /Connections
             - ConnectionFactory.cs
             - ConnectionManager.cs
          /Modules
             /Products
             /Customers
          /Infrastructure
             - JwtHelper.cs
             - TenantResolver.cs

------------------------------------------------------------------------

## 📌 11. Geliştirme Yol Haritası

1.  JWT tenant alanını ekle\
2.  DB yönlendirme modülünü kur\
3.  Tenant Connection Manager yaz\
4.  Tenant bazlı migration oluştur\
5.  Frontend → login → JWT akışını bağla\
6.  API endpoint'lerini tenant destekli yaz\
7.  Lisanslama alt modülünü ekle\
8.  QA ve dağıtım

------------------------------------------------------------------------

## 📌 12. Sonuç

Bu mimari ile:

-   Çoklu müşteri yönetimi\
-   Çoklu veritabanı desteği\
-   Tek API üzerinden sistem yönetimi\
-   SaaS modeli\
-   On-prem müşteri kurulumu

tam anlamıyla uygulanabilir hale gelir.

Sistem hem güvenli hem de genişletilebilir bir yapıya sahip olur.

------------------------------------------------------------------------

## 📌 13. ⚠️ Kritik Eksiklikler ve Öneriler

### 🔴 Yüksek Öncelikli Eksiklikler

#### 13.1. Tenant Yönetim Sistemi Yok
**Durum:** ❌ Henüz implement edilmedi

**Gerekenler:**
- Tenant entity (CustomerCode, ConnectionString, License bilgisi)
- Master database (tenant yönetimi için)
- Tenant service (CRUD operations)
- Tenant validation logic

**Öneri:**
```csharp
// wixi.Core/Entities/Tenant.cs
public class Tenant
{
    public int Id { get; set; }
    public string CustomerCode { get; set; } // FIRMA_001
    public string Name { get; set; }
    public string ConnectionString { get; set; } // Encrypted
    public bool IsActive { get; set; }
    public LicenseInfo License { get; set; }
}
```

#### 13.2. Tenant Resolution Middleware Yok
**Durum:** ❌ JWT'den tenant bilgisi okunmuyor

**Gerekenler:**
- JWT'den customerCode claim okuma
- Tenant bilgisini HttpContext'e ekleme
- Tenant validation (aktif mi, lisans geçerli mi?)
- Connection string resolve etme

**Öneri:**
```csharp
// wixi.WebAPI/Middleware/TenantResolutionMiddleware.cs
// JWT'den customerCode okur
// Tenant bilgisini HttpContext'e ekler
// Connection string'i resolve eder
```

#### 13.3. Connection Factory Yok
**Durum:** ❌ Hard-coded connection string

**Gerekenler:**
- Tenant → Connection String mapping
- Connection string cache (performance)
- Connection pooling per tenant
- Connection validation

**Öneri:**
```csharp
// wixi.DataAccess/Connection/TenantConnectionFactory.cs
public interface ITenantConnectionFactory
{
    string GetConnectionString(string customerCode);
    SqlConnection CreateConnection(string customerCode);
    Task<bool> TestConnectionAsync(string customerCode);
}
```

#### 13.4. JWT'de customerCode Claim Yok
**Durum:** ❌ Mevcut JWT'de customerCode yok

**Gerekenler:**
- Login sırasında tenant bilgisini al
- JWT'ye customerCode claim ekle
- Token validation'da customerCode kontrolü

**Öneri:**
```csharp
// wixi.Identity/Services/JwtService.cs
// Login sırasında:
claims.Add(new Claim("customerCode", user.Tenant.CustomerCode));
```

#### 13.5. DbContext Factory Pattern Yok
**Durum:** ❌ Tek DbContext, tek connection string

**Gerekenler:**
- Her tenant için ayrı DbContext instance
- DbContext factory pattern
- Connection lifecycle yönetimi

**Öneri:**
```csharp
// wixi.DataAccess/Connection/TenantDbContextFactory.cs
public interface ITenantDbContextFactory
{
    WixiDbContext CreateDbContext(string customerCode);
}
```

### 🟡 Orta Öncelikli Eksiklikler

#### 13.6. Lisanslama Modülü Yok
**Durum:** ❌ License kontrolü yok

**Gerekenler:**
- License entity (type, maxUsers, expiration, features)
- License validation middleware
- Feature flag sistemi
- Kullanım limitleri kontrolü

#### 13.7. Tenant Bazlı Migration Stratejisi Yok
**Durum:** ❌ Migration nasıl yapılacak belirsiz

**Gerekenler:**
- Tenant provisioning service (yeni tenant oluşturma)
- Migration runner per tenant
- Migration versioning
- Rollback stratejisi

#### 13.8. Configuration Management Yok
**Durum:** ❌ Connection string'ler nerede saklanacak?

**Gerekenler:**
- Tenant bazlı config store (DB veya config file)
- Connection string encryption
- Config cache mekanizması
- Config update stratejisi

#### 13.9. Tenant Context Service Yok
**Durum:** ❌ Her request'te tenant bilgisi nasıl alınacak?

**Gerekenler:**
- ITenantContext interface
- HttpContext'ten tenant bilgisi alma
- Dependency injection ile kullanım

**Öneri:**
```csharp
// wixi.Core/Services/ITenantContext.cs
public interface ITenantContext
{
    string CustomerCode { get; }
    Tenant Tenant { get; }
    string ConnectionString { get; }
}
```

### 🟢 Düşük Öncelikli (Uzun Vadeli)

#### 13.10. Monitoring ve Logging
- Tenant bazlı log aggregation
- Performance metrics per tenant
- Error tracking per tenant
- Usage analytics

#### 13.11. Backup ve Disaster Recovery
- Tenant bazlı backup stratejisi
- Restore prosedürleri
- Failover mekanizması
- Backup automation

#### 13.12. Ölçeklenebilirlik
- Horizontal scaling stratejisi
- Database sharding (1000+ tenant için)
- Load balancing
- Distributed caching (Redis)

------------------------------------------------------------------------

## 📌 14. 🔐 Güvenlik Eksiklikleri

### 14.1. Tenant İzolasyonu
**Risk:** Bir tenant başka tenant'ın verisine erişebilir

**Çözüm:**
- ✅ Her sorguda tenant kontrolü
- ✅ Connection string validation
- ✅ Row-Level Security (SQL Server'da)

### 14.2. Connection String Güvenliği
**Risk:** Connection string'ler plain text saklanıyor

**Çözüm:**
- ✅ Encryption (AES-256)
- ✅ Key management (Azure Key Vault veya benzeri)
- ✅ Connection string rotation

### 14.3. JWT Güvenliği
**Risk:** JWT'de tenant bilgisi yok, manipüle edilebilir

**Çözüm:**
- ✅ JWT'ye customerCode claim ekle
- ✅ Token validation her request'te
- ✅ Token blacklist kontrolü (zaten var ✅)

### 14.4. Audit Logging
**Risk:** Hangi tenant hangi işlemi yaptı bilinmiyor

**Çözüm:**
- ✅ Her işlemde tenant bilgisi loglanmalı
- ✅ Tenant bazlı audit log sorgulama
- ✅ Compliance için saklama politikaları

------------------------------------------------------------------------

## 📌 15. 📊 Performans Eksiklikleri

### 15.1. Connection Pooling
**Eksik:** Tenant bazlı connection pooling yok

**Çözüm:**
- Her tenant için ayrı connection pool
- Pool size optimization
- Connection timeout yönetimi

### 15.2. Caching
**Eksik:** Tenant bilgileri ve connection string'ler cache'lenmiyor

**Çözüm:**
- IMemoryCache ile tenant bilgileri
- Connection string cache
- Configuration cache

### 15.3. Database Indexing
**Eksik:** Tenant bazlı index stratejisi yok

**Çözüm:**
- Tenant bazlı index optimization
- Query performance monitoring
- Partitioning (büyük tenant'lar için)

------------------------------------------------------------------------

## 📌 16. 🏗️ Önerilen Proje Yapısı

```
wixi.backendV2/
├── wixi.Core/
│   ├── Entities/
│   │   ├── Tenant.cs          ⚠️ EKSİK
│   │   └── License.cs         ⚠️ EKSİK
│   ├── Services/
│   │   ├── ITenantContext.cs  ⚠️ EKSİK
│   │   ├── TenantContext.cs   ⚠️ EKSİK
│   │   ├── ITenantService.cs  ⚠️ EKSİK
│   │   └── TenantService.cs  ⚠️ EKSİK
│   └── Configuration/
│       └── TenantOptions.cs   ⚠️ EKSİK
│
├── wixi.DataAccess/
│   ├── Connection/
│   │   ├── ITenantConnectionFactory.cs     ⚠️ EKSİK
│   │   ├── TenantConnectionFactory.cs      ⚠️ EKSİK
│   │   ├── ITenantDbContextFactory.cs      ⚠️ EKSİK
│   │   └── TenantDbContextFactory.cs       ⚠️ EKSİK
│   └── Migrations/
│       └── TenantMigrations/              ⚠️ EKSİK
│
├── wixi.WebAPI/
│   ├── Middleware/
│   │   ├── TenantResolutionMiddleware.cs  ⚠️ EKSİK
│   │   └── TenantValidationMiddleware.cs   ⚠️ EKSİK
│   └── Controllers/
│       └── TenantController.cs (Admin)     ⚠️ EKSİK
│
└── wixi.License/ (Yeni modül)              ⚠️ EKSİK
    ├── Entities/
    │   └── License.cs
    ├── Services/
    │   ├── ILicenseService.cs
    │   └── LicenseService.cs
    └── Features/
        └── FeatureFlagService.cs
```

------------------------------------------------------------------------

## 📌 17. 🗄️ Veritabanı Yapısı Önerileri

### 17.1. Master Database (Tenant Yönetimi)
**Durum:** ❌ Henüz oluşturulmadı

**Gereken Tablolar:**
```sql
-- Tenants tablosu
CREATE TABLE Tenants (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerCode NVARCHAR(50) UNIQUE NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    ConnectionString NVARCHAR(500) NOT NULL, -- Encrypted
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2
);

-- Licenses tablosu
CREATE TABLE Licenses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT FOREIGN KEY REFERENCES Tenants(Id),
    LicenseType NVARCHAR(50), -- Basic, Premium, Enterprise
    MaxUsers INT,
    MaxStorageGB INT,
    ExpirationDate DATETIME2,
    Features NVARCHAR(MAX), -- JSON
    IsActive BIT DEFAULT 1
);
```

### 17.2. Tenant Database (Her Müşteri İçin)
**Durum:** ✅ Mevcut WixiDbContext yapısı kullanılabilir

**Not:** Aynı schema, farklı database. Her tenant için ayrı DB.

------------------------------------------------------------------------

## 📌 18. 🎯 Uygulama Roadmap

### Faz 1: Temel Multi-Tenant (2-3 Hafta) 🔴
1. ✅ Tenant entity ve service oluştur
2. ✅ Master database oluştur
3. ✅ Tenant resolution middleware yaz
4. ✅ Connection factory implement et
5. ✅ JWT'ye customerCode claim ekle
6. ✅ Tenant context service oluştur

### Faz 2: Veritabanı Yönetimi (2 Hafta) 🟡
1. ✅ DbContext factory pattern uygula
2. ✅ Repository pattern güncelle (tenant-aware)
3. ✅ Migration stratejisi oluştur
4. ✅ Connection pooling optimization

### Faz 3: Lisanslama (2 Hafta) 🟡
1. ✅ License entity ve service
2. ✅ Feature flag sistemi
3. ✅ Kullanım limitleri
4. ✅ License validation middleware

### Faz 4: Production Ready (2 Hafta) 🟢
1. ✅ Monitoring ve logging
2. ✅ Performance optimization
3. ✅ Security hardening
4. ✅ Documentation
5. ✅ Testing (Unit, Integration, Load)

------------------------------------------------------------------------

## 📌 19. ⚠️ Dikkat Edilmesi Gerekenler

### 19.1. Tenant Isolation
- ❌ **ASLA** bir tenant'ın başka tenant'ın verisine erişmemeli
- ✅ Her sorguda tenant kontrolü yapılmalı
- ✅ Connection string validation zorunlu

### 19.2. Performance
- ❌ Connection string lookup her request'te DB'ye gitmemeli (cache kullan)
- ✅ Connection pooling doğru yapılandırılmalı
- ✅ Async operations kullanılmalı

### 19.3. Security
- ❌ Connection string'ler plain text saklanmamalı
- ✅ Encryption kullanılmalı (AES-256)
- ✅ JWT validation her request'te yapılmalı

### 19.4. Error Handling
- ✅ Tenant bulunamazsa → 403 Forbidden
- ✅ Connection hatası → 503 Service Unavailable
- ✅ License expired → 402 Payment Required
- ✅ Invalid tenant → 400 Bad Request

------------------------------------------------------------------------

## 📌 20. 📚 Detaylı Dokümanlar

### 20.1. Multi-Tenant Analiz Dokümanı

👉 **`MULTI-TENANT-ANALYSIS-AND-RECOMMENDATIONS.md`**

Bu dosyada:
- ✅ Detaylı kod örnekleri
- ✅ Design pattern'ler
- ✅ Security best practices
- ✅ Performance optimization stratejileri
- ✅ Testing yaklaşımları
- ✅ Ölçeklenebilirlik senaryoları

### 20.2. Deployment ve Uzaktan Erişim Senaryoları

👉 **`DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md`**

Bu dosyada:
- ✅ SaaS, On-Premise, Hybrid deployment modelleri
- ✅ Uzaktan erişim stratejileri (VPN, mTLS, API Gateway)
- ✅ Modül bazlı lisanslama sistemi
- ✅ Güvenlik ve erişim kontrolü
- ✅ Profesyonel uygulama örnekleri
- ✅ Teknik detaylar ve konfigürasyonlar

### 20.3. Hızlı Başlangıç Rehberi

👉 **`QUICK-START-DEPLOYMENT-GUIDE.md`**

Bu dosyada:
- ✅ Adım adım kurulum senaryoları
- ✅ Pratik örnekler
- ✅ Modül aktivasyon/deaktivasyon
- ✅ Troubleshooting rehberi
- ✅ Deployment checklist

------------------------------------------------------------------------