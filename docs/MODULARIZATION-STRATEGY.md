# 🏗️ Modülerleştirme Stratejisi - Mevcut Sistemleri Modüllere Geçirme

## 🎯 Genel Bakış

Mevcut monolith yapıdaki tüm sistemleri modüler, taşınabilir ve bakımı kolay modüllere geçirme stratejisi.

---

## 📊 Mevcut Durum Analizi

### Mevcut Domain'ler

| Domain | Entity Sayısı | Service Sayısı | Controller Sayısı | Durum |
|--------|---------------|----------------|-------------------|-------|
| **Identity** | 2 (AppUser, AppRole) | 1 (AuthManager) | 1 (AuthController) | ✅ Mevcut |
| **Client** | 4 (Client, EducationType, EducationInfo, PendingClientCode) | 1 (ClientService) | 1 (ClientsController) | ✅ Mevcut |
| **Document** | 4 (Document, DocumentType, DocumentReview, FileStorage) | 2 (DocumentService, DocumentReviewService) | 2 (DocumentsController, AdminDocumentReviewController) | ✅ Mevcut |
| **Application** | 6 (Application, ApplicationStep, ApplicationSubStep, ApplicationTemplate, etc.) | 1 (ApplicationService) | 1 (ApplicationsController) | ✅ Mevcut |
| **Support** | 4 (SupportTicket, SupportMessage, FAQ, Notification) | 1 (SupportService, NotificationService) | 1 (SupportController) | ✅ Mevcut |
| **Content** | 5 (NewsItem, TeamMember, Translation, ContentSettings, SystemSettings) | 4 (TranslationService, ContentSettingsService, SystemSettingsService, UserPreferenceService) | 5 (NewsController, TeamMembersController, TranslationsController, etc.) | ✅ Mevcut |
| **Email** | 3 (EmailLog, EmailTemplate, SmtpSettings) | 3 (SmtpEmailSender, SmtpSettingsService, EmailTemplateService) | 2 (AdminEmailController, AdminEmailTemplatesController) | ✅ Mevcut |
| **Form** | 3 (ContactFormSubmission, EmployeeFormSubmission, EmployerFormSubmission) | 0 (Controller'da direkt) | 1 (FormSubmissionsController, AdminFormSubmissionsController) | ✅ Mevcut |
| **Core** | 2 (ApplicationLog, TokenBlacklist) | 0 | 0 | ✅ Mevcut |

**Toplam:**
- ✅ 33 Entity
- ✅ 14 Service
- ✅ 20+ Controller
- ✅ 1 Monolith yapı

---

## 🏗️ Hedef Modüler Yapı

### Modül Sınıflandırması

#### 1. Core Modules (Çekirdek Modüller)
- **wixi.Core** - Temel utilities, exceptions, helpers
- **wixi.Identity** - Authentication, Authorization, User management

#### 2. Business Modules (İş Modülleri)
- **wixi.Clients** - Müşteri yönetimi
- **wixi.Documents** - Belge yönetimi
- **wixi.Applications** - Başvuru takibi
- **wixi.Support** - Destek sistemi
- **wixi.Appointments** - Randevu sistemi (YENİ)
- **wixi.Payments** - Ödeme sistemi (YENİ)

#### 3. Content Modules (İçerik Modülleri)
- **wixi.Content** - News, Team Members, Translations
- **wixi.Forms** - Form gönderimleri

#### 4. Infrastructure Modules (Altyapı Modülleri)
- **wixi.Email** - Email servisi
- **wixi.FileStorage** - Dosya yönetimi
- **wixi.DataAccess** - Database access (Shared)

---

## 📦 Detaylı Modül Yapısı

### 1. wixi.Identity Module

**Mevcut:**
- Entities: AppUser, AppRole
- Services: AuthManager
- Controllers: AuthController

**Hedef:**
```
wixi.Identity/
├── Entities/
│   ├── AppUser.cs
│   ├── AppRole.cs
│   └── TokenBlacklist.cs (Core'dan taşı)
├── Interfaces/
│   ├── IAuthService.cs
│   └── ITokenService.cs
├── Services/
│   ├── AuthService.cs
│   └── TokenService.cs
├── DTOs/
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   └── AuthResponseDto.cs
└── Validators/
    └── LoginValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Identity/*` → `wixi.Identity/Entities/`
- `wixi.Entities/Concrete/Core/TokenBlacklist.cs` → `wixi.Identity/Entities/`
- `wixi.Business/Abstract/IAuthService.cs` → `wixi.Identity/Interfaces/IAuthService.cs`
- `wixi.Business/Concrete/AuthManager.cs` → `wixi.Identity/Services/AuthService.cs`
- `wixi.WebAPI/Controllers/AuthController.cs` → `wixi.Identity/Controllers/AuthController.cs` (veya wixi.WebAPI/Controllers'da kalabilir)

---

### 2. wixi.Clients Module

**Mevcut:**
- Entities: Client, EducationType, EducationInfo, PendingClientCode
- Services: ClientService
- Controllers: ClientsController

**Hedef:**
```
wixi.Clients/
├── Entities/
│   ├── Client.cs
│   ├── EducationType.cs
│   ├── EducationInfo.cs
│   └── PendingClientCode.cs
├── Interfaces/
│   └── IClientService.cs
├── Services/
│   └── ClientService.cs
├── DTOs/
│   ├── ClientCreateDto.cs
│   ├── ClientResponseDto.cs
│   └── ClientUpdateDto.cs
└── Validators/
    └── ClientValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Client/*` → `wixi.Clients/Entities/`
- `wixi.Business/Abstract/IClientService.cs` → `wixi.Clients/Interfaces/IClientService.cs`
- `wixi.Business/Concrete/ClientService.cs` → `wixi.Clients/Services/ClientService.cs`
- `wixi.WebAPI/Controllers/ClientsController.cs` → `wixi.Clients/Controllers/` (veya wixi.WebAPI/Controllers'da kalabilir)

---

### 3. wixi.Documents Module

**Mevcut:**
- Entities: Document, DocumentType, DocumentReview, FileStorage
- Services: DocumentService, DocumentReviewService
- Controllers: DocumentsController, AdminDocumentReviewController

**Hedef:**
```
wixi.Documents/
├── Entities/
│   ├── Document.cs
│   ├── DocumentType.cs
│   ├── DocumentReview.cs
│   └── FileStorage.cs
├── Interfaces/
│   ├── IDocumentService.cs
│   └── IDocumentReviewService.cs
├── Services/
│   ├── DocumentService.cs
│   └── DocumentReviewService.cs
├── DTOs/
│   ├── DocumentUploadDto.cs
│   ├── DocumentResponseDto.cs
│   └── DocumentReviewDto.cs
└── Validators/
    └── DocumentValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Document/*` → `wixi.Documents/Entities/`
- `wixi.Business/Abstract/IDocumentService.cs` → `wixi.Documents/Interfaces/IDocumentService.cs`
- `wixi.Business/Abstract/IDocumentReviewService.cs` → `wixi.Documents/Interfaces/IDocumentReviewService.cs`
- `wixi.Business/Concrete/DocumentService.cs` → `wixi.Documents/Services/DocumentService.cs`
- `wixi.Business/Concrete/DocumentReviewService.cs` → `wixi.Documents/Services/DocumentReviewService.cs`
- `wixi.WebAPI/Controllers/DocumentsController.cs` → `wixi.Documents/Controllers/` (veya wixi.WebAPI/Controllers'da kalabilir)
- `wixi.WebAPI/Controllers/Admin/DocumentReviewController.cs` → `wixi.Documents/Controllers/Admin/` (veya wixi.WebAPI/Controllers'da kalabilir)

---

### 4. wixi.Applications Module

**Mevcut:**
- Entities: Application, ApplicationStep, ApplicationSubStep, ApplicationTemplate, etc.
- Services: ApplicationService
- Controllers: ApplicationsController

**Hedef:**
```
wixi.Applications/
├── Entities/
│   ├── Application.cs
│   ├── ApplicationStep.cs
│   ├── ApplicationSubStep.cs
│   ├── ApplicationTemplate.cs
│   ├── ApplicationStepTemplate.cs
│   ├── ApplicationSubStepTemplate.cs
│   └── ApplicationHistory.cs
├── Interfaces/
│   └── IApplicationService.cs
├── Services/
│   └── ApplicationService.cs
├── DTOs/
│   ├── ApplicationCreateDto.cs
│   ├── ApplicationResponseDto.cs
│   └── ApplicationUpdateDto.cs
└── Validators/
    └── ApplicationValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Application/*` → `wixi.Applications/Entities/`
- `wixi.Business/Abstract/IApplicationService.cs` → `wixi.Applications/Interfaces/IApplicationService.cs`
- `wixi.Business/Concrete/ApplicationService.cs` → `wixi.Applications/Services/ApplicationService.cs`
- `wixi.WebAPI/Controllers/ApplicationsController.cs` → `wixi.Applications/Controllers/` (veya wixi.WebAPI/Controllers'da kalabilir)

---

### 5. wixi.Support Module

**Mevcut:**
- Entities: SupportTicket, SupportMessage, FAQ, Notification
- Services: SupportService, NotificationService
- Controllers: SupportController

**Hedef:**
```
wixi.Support/
├── Entities/
│   ├── SupportTicket.cs
│   ├── SupportMessage.cs
│   ├── FAQ.cs
│   └── Notification.cs
├── Interfaces/
│   ├── ISupportService.cs
│   └── INotificationService.cs
├── Services/
│   ├── SupportService.cs
│   └── NotificationService.cs
├── DTOs/
│   ├── SupportTicketCreateDto.cs
│   ├── SupportTicketResponseDto.cs
│   └── NotificationDto.cs
└── Validators/
    └── SupportTicketValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Support/*` → `wixi.Support/Entities/`
- `wixi.Business/Abstract/ISupportService.cs` → `wixi.Support/Interfaces/ISupportService.cs`
- `wixi.Business/Abstract/INotificationService.cs` → `wixi.Support/Interfaces/INotificationService.cs`
- `wixi.Business/Concrete/SupportService.cs` → `wixi.Support/Services/SupportService.cs`
- `wixi.Business/Concrete/NotificationService.cs` → `wixi.Support/Services/NotificationService.cs`
- `wixi.WebAPI/Controllers/SupportController.cs` → `wixi.Support/Controllers/` (veya wixi.WebAPI/Controllers'da kalabilir)

---

### 6. wixi.Content Module

**Mevcut:**
- Entities: NewsItem, TeamMember, Translation, ContentSettings, SystemSettings
- Services: TranslationService, ContentSettingsService, SystemSettingsService, UserPreferenceService
- Controllers: NewsController, TeamMembersController, TranslationsController, AdminContentSettingsController, AdminSettingsController

**Hedef:**
```
wixi.Content/
├── Entities/
│   ├── NewsItem.cs
│   ├── TeamMember.cs
│   ├── Translation.cs
│   ├── ContentSettings.cs
│   ├── SystemSettings.cs
│   └── UserPreference.cs
├── Interfaces/
│   ├── INewsService.cs
│   ├── ITeamMemberService.cs
│   ├── ITranslationService.cs
│   ├── IContentSettingsService.cs
│   ├── ISystemSettingsService.cs
│   └── IUserPreferenceService.cs
├── Services/
│   ├── NewsService.cs
│   ├── TeamMemberService.cs
│   ├── TranslationService.cs
│   ├── ContentSettingsService.cs
│   ├── SystemSettingsService.cs
│   └── UserPreferenceService.cs
├── DTOs/
│   ├── NewsCreateDto.cs
│   ├── TeamMemberDto.cs
│   └── TranslationDto.cs
└── Validators/
    └── NewsValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Content/*` → `wixi.Content/Entities/`
- `wixi.Business/Abstract/ITranslationService.cs` → `wixi.Content/Interfaces/ITranslationService.cs`
- `wixi.Business/Abstract/IContentSettingsService.cs` → `wixi.Content/Interfaces/IContentSettingsService.cs`
- `wixi.Business/Abstract/ISystemSettingsService.cs` → `wixi.Content/Interfaces/ISystemSettingsService.cs`
- `wixi.Business/Abstract/IUserPreferenceService.cs` → `wixi.Content/Interfaces/IUserPreferenceService.cs`
- `wixi.Business/Concrete/TranslationService.cs` → `wixi.Content/Services/TranslationService.cs`
- `wixi.Business/Concrete/ContentSettingsService.cs` → `wixi.Content/Services/ContentSettingsService.cs`
- `wixi.Business/Concrete/SystemSettingsService.cs` → `wixi.Content/Services/SystemSettingsService.cs`
- `wixi.Business/Concrete/UserPreferenceService.cs` → `wixi.Content/Services/UserPreferenceService.cs`
- Tüm Content controller'ları → `wixi.Content/Controllers/` (veya wixi.WebAPI/Controllers'da kalabilir)

---

### 7. wixi.Email Module

**Mevcut:**
- Entities: EmailLog, EmailTemplate, SmtpSettings
- Services: SmtpEmailSender, SmtpSettingsService, EmailTemplateService
- Controllers: AdminEmailController, AdminEmailTemplatesController

**Hedef:**
```
wixi.Email/
├── Entities/
│   ├── EmailLog.cs
│   ├── EmailTemplate.cs
│   └── SmtpSettings.cs
├── Interfaces/
│   ├── IEmailSender.cs
│   ├── ISmtpSettingsService.cs
│   ├── IEmailTemplateService.cs
│   └── ISmtpHealthCheck.cs
├── Services/
│   ├── SmtpEmailSender.cs
│   ├── SmtpSettingsService.cs
│   ├── EmailTemplateService.cs
│   └── SmtpHealthCheck.cs
├── DTOs/
│   ├── EmailSendDto.cs
│   ├── SmtpSettingsDto.cs
│   └── EmailTemplateDto.cs
└── Validators/
    └── EmailTemplateValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Email/*` → `wixi.Email/Entities/`
- `wixi.Business/Abstract/IEmailSender.cs` → `wixi.Email/Interfaces/IEmailSender.cs`
- `wixi.Business/Abstract/ISmtpSettingsService.cs` → `wixi.Email/Interfaces/ISmtpSettingsService.cs`
- `wixi.Business/Abstract/IEmailTemplateService.cs` → `wixi.Email/Interfaces/IEmailTemplateService.cs`
- `wixi.Business/Abstract/ISmtpHealthCheck.cs` → `wixi.Email/Interfaces/ISmtpHealthCheck.cs`
- `wixi.Business/Concrete/SmtpEmailSender.cs` → `wixi.Email/Services/SmtpEmailSender.cs`
- `wixi.Business/Concrete/SmtpSettingsService.cs` → `wixi.Email/Services/SmtpSettingsService.cs`
- `wixi.Business/Concrete/EmailTemplateService.cs` → `wixi.Email/Services/EmailTemplateService.cs`
- `wixi.Business/Concrete/SmtpHealthCheck.cs` → `wixi.Email/Services/SmtpHealthCheck.cs`
- `wixi.WebAPI/Controllers/AdminEmailController.cs` → `wixi.Email/Controllers/Admin/` (veya wixi.WebAPI/Controllers'da kalabilir)
- `wixi.WebAPI/Controllers/AdminEmailTemplatesController.cs` → `wixi.Email/Controllers/Admin/` (veya wixi.WebAPI/Controllers'da kalabilir)

---

### 8. wixi.Forms Module

**Mevcut:**
- Entities: ContactFormSubmission, EmployeeFormSubmission, EmployerFormSubmission
- Services: Yok (Controller'da direkt işlem)
- Controllers: FormSubmissionsController, AdminFormSubmissionsController

**Hedef:**
```
wixi.Forms/
├── Entities/
│   ├── ContactFormSubmission.cs
│   ├── EmployeeFormSubmission.cs
│   └── EmployerFormSubmission.cs
├── Interfaces/
│   └── IFormSubmissionService.cs
├── Services/
│   └── FormSubmissionService.cs
├── DTOs/
│   ├── ContactFormDto.cs
│   ├── EmployeeFormDto.cs
│   └── EmployerFormDto.cs
└── Validators/
    └── FormSubmissionValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Form/*` → `wixi.Forms/Entities/`
- `wixi.WebAPI/Controllers/FormSubmissionsController.cs` → `wixi.Forms/Controllers/`
- `wixi.WebAPI/Controllers/AdminFormSubmissionsController.cs` → `wixi.Forms/Controllers/Admin/`

---

### 9. wixi.FileStorage Module

**Mevcut:**
- Services: LocalFileStorageService
- Infrastructure: File upload/download

**Hedef:**
```
wixi.FileStorage/
├── Interfaces/
│   └── IFileStorageService.cs
├── Services/
│   ├── LocalFileStorageService.cs
│   └── CloudFileStorageService.cs (gelecek)
├── DTOs/
│   └── FileUploadDto.cs
└── Validators/
    └── FileValidator.cs
```

**Taşınacak Dosyalar:**
- `wixi.Business/Abstract/IFileStorageService.cs` → `wixi.FileStorage/Interfaces/IFileStorageService.cs`
- `wixi.Business/Concrete/LocalFileStorageService.cs` → `wixi.FileStorage/Services/LocalFileStorageService.cs`

---

### 10. wixi.Core Module (Güncellenecek)

**Mevcut:**
- Entities: ApplicationLog
- Utilities: Security, Helpers

**Hedef:**
```
wixi.Core/
├── Entities/
│   └── ApplicationLog.cs (Shared logging)
├── Utilities/
│   ├── Security/
│   ├── Helpers/
│   └── Extensions/
├── Exceptions/
│   ├── BusinessException.cs
│   └── ValidationException.cs
└── Interfaces/
    └── ISecretProtector.cs
```

**Taşınacak Dosyalar:**
- `wixi.Entities/Concrete/Core/ApplicationLog.cs` → `wixi.Core/Entities/`
- `wixi.Core/Utilities/*` → Zaten burada
- `wixi.Core/Exceptions/*` → Zaten burada

---

## 📐 Standart Modül Yapısı

### 🎯 Klasör Organizasyonu

Her modül aşağıdaki standart yapıda olacak:

```
wixi.ModuleName/
├── Entities/              # Domain entities
│   └── *.cs
├── Interfaces/            # Service interfaces (AYRI KLASÖR) ✅
│   └── I*.cs
├── Services/              # Service implementations (AYRI KLASÖR) ✅
│   └── *.Service.cs
├── DTOs/                  # Data Transfer Objects
│   └── *.Dto.cs
├── Controllers/           # API Controllers (opsiyonel)
│   └── *.Controller.cs
├── Validators/            # FluentValidation validators
│   └── *.Validator.cs
├── Extensions/            # Extension methods (DI registration)
│   └── ServiceCollectionExtensions.cs
└── wixi.ModuleName.csproj
```

### 📁 Neden Ayrı Klasörler?

**Interfaces/ ve Services/ Ayrımı:**
- ✅ **Okunabilirlik**: Interface'ler ve implementation'lar net ayrılmış
- ✅ **Dependency Inversion**: Interface'ler üst seviyede, implementation'lar alt seviyede
- ✅ **Test Edilebilirlik**: Mock'lar interface'lere bağımlı
- ✅ **Clean Architecture**: Domain logic ve infrastructure ayrımı
- ✅ **Bakım Kolaylığı**: Interface değişiklikleri ve implementation değişiklikleri ayrı takip edilebilir
- ✅ **SOLID Principles**: Interface Segregation ve Dependency Inversion prensipleri

### 📝 Namespace Yapısı

```csharp
// Interface
namespace wixi.Identity.Interfaces
{
    public interface IAuthService 
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}

// Implementation
namespace wixi.Identity.Services
{
    public class AuthService : IAuthService
    {
        // Implementation
    }
}

// Entity
namespace wixi.Identity.Entities
{
    public class AppUser 
    {
        // Entity properties
    }
}

// DTO
namespace wixi.Identity.DTOs
{
    public class LoginRequest 
    {
        // DTO properties
    }
}
```

### 🔄 Dependency Injection Registration

```csharp
// wixi.Identity/Extensions/ServiceCollectionExtensions.cs
namespace wixi.Identity.Extensions
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Interfaces ve Services ayrı klasörlerden geliyor
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            
            return services;
        }
    }
}

// wixi.WebAPI/Program.cs
builder.Services.AddIdentityModule(builder.Configuration);
```

---

## 🚀 Migration Stratejisi

### Yaklaşım: Incremental Migration (Aşamalı Geçiş)

**Strateji:** Big Bang değil, adım adım geçiş

#### Phase 1: Preparation (Hazırlık) - 1 Hafta

1. **Modül Yapısı Oluştur**
   ```bash
   # Her modül için class library oluştur
   dotnet new classlib -n wixi.Identity -f net8.0
   dotnet new classlib -n wixi.Clients -f net8.0
   dotnet new classlib -n wixi.Documents -f net8.0
   # ... diğer modüller
   ```

2. **Solution'a Ekle**
   ```bash
   dotnet sln add wixi.Identity/wixi.Identity.csproj
   dotnet sln add wixi.Clients/wixi.Clients.csproj
   # ... diğer modüller
   ```

3. **Dependencies Belirle**
   - Her modül `wixi.Core`'a bağımlı
   - Her modül `wixi.DataAccess`'e bağımlı (Shared DbContext)
   - `wixi.WebAPI` tüm modüllere bağımlı

#### Phase 2: Entity Migration (Entity Taşıma) - 2 Hafta

**Sıra:**
1. ✅ **wixi.Identity** (1 gün)
2. ✅ **wixi.Clients** (1 gün)
3. ✅ **wixi.Documents** (2 gün)
4. ✅ **wixi.Applications** (2 gün)
5. ✅ **wixi.Support** (1 gün)
6. ✅ **wixi.Content** (2 gün)
7. ✅ **wixi.Email** (1 gün)
8. ✅ **wixi.Forms** (1 gün)

**Adımlar:**
1. Entity'leri yeni modüle taşı
2. Namespace'leri güncelle
3. DbContext'te entity mapping'leri güncelle
4. Migration oluştur ve test et
5. Build ve test

#### Phase 3: Service Migration (Servis Taşıma) - 2 Hafta

**Sıra:** Entity migration ile aynı sıra

**Adımlar:**
1. Service interface'lerini taşı
2. Service implementation'ları taşı
3. Dependencies'i güncelle
4. DI registration'ı güncelle
5. Test et

#### Phase 4: Controller Migration (Controller Taşıma) - 1 Hafta

**Adımlar:**
1. Controller'ları yeni modüle taşı
2. Route'ları güncelle
3. Dependencies'i güncelle
4. Test et

#### Phase 5: Cleanup (Temizlik) - 3 Gün

1. Eski dosyaları sil
2. Eski namespace'leri temizle
3. Unused references'ları kaldır
4. Documentation güncelle

---

## 📋 Detaylı Migration Planı

### Adım 1: wixi.Identity Module

#### 1.1 Proje Oluştur
```bash
cd wixi.backend
dotnet new classlib -n wixi.Identity -f net8.0
dotnet sln add wixi.Identity/wixi.Identity.csproj
```

#### 1.2 Dependencies Ekle
```bash
cd wixi.Identity
dotnet add reference ../wixi.Core/wixi.Core.csproj
dotnet add reference ../wixi.DataAccess/wixi.DataAccess.csproj
dotnet add package Microsoft.AspNetCore.Identity
dotnet add package Microsoft.EntityFrameworkCore
```

#### 1.3 Entity'leri Taşı
```bash
# Entities klasörü oluştur
mkdir Entities

# Dosyaları kopyala
cp ../wixi.Entities/Concrete/Identity/AppUser.cs Entities/
cp ../wixi.Entities/Concrete/Identity/AppRole.cs Entities/
cp ../wixi.Entities/Concrete/Core/TokenBlacklist.cs Entities/

# Namespace'leri güncelle
# wixi.Entities.Concrete.Identity → wixi.Identity.Entities
```

#### 1.4 Service'leri Taşı
```bash
# Interfaces ve Services klasörleri oluştur
mkdir Interfaces
mkdir Services

# Interface'i taşı/kopyala
# IAuthService.cs → Interfaces/IAuthService.cs

# AuthManager.cs'i kopyala ve refactor et
cp ../wixi.Business/Concrete/AuthManager.cs Services/AuthService.cs

# Namespace'leri güncelle
# wixi.Business.Concrete → wixi.Identity.Services
# wixi.Business.Abstract → wixi.Identity.Interfaces
```

#### 1.5 Controller'ı Taşı
```bash
# Controllers klasörü oluştur
mkdir Controllers

# AuthController.cs'i kopyala
cp ../wixi.WebAPI/Controllers/AuthController.cs Controllers/

# Namespace'leri güncelle
```

#### 1.6 DbContext Güncelle
```csharp
// wixi.DataAccess/Contexts/WixiDbContext.cs
// Entity'lerin namespace'lerini güncelle
using wixi.Identity.Entities; // Yeni namespace
```

#### 1.7 DI Registration
```csharp
// wixi.WebAPI/Program.cs
builder.Services.AddScoped<IAuthService, AuthService>();
```

#### 1.8 Test
```bash
dotnet build
dotnet test
```

---

### Adım 2: wixi.Clients Module

Aynı adımları takip et:
1. Proje oluştur
2. Dependencies ekle
3. Entity'leri taşı
4. Service'leri taşı
5. Controller'ı taşı
6. DbContext güncelle
7. DI registration
8. Test

---

## 🔧 Teknik Detaylar

### Shared DbContext

**Sorun:** Tüm modüller aynı database'i kullanıyor.

**Çözüm:** Shared DbContext pattern

```csharp
// wixi.DataAccess/Contexts/WixiDbContext.cs
public class WixiDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    // Identity Module
    public DbSet<AppUser> Users { get; set; }
    public DbSet<AppRole> Roles { get; set; }
    public DbSet<TokenBlacklist> TokenBlacklists { get; set; }
    
    // Clients Module
    public DbSet<Client> Clients { get; set; }
    public DbSet<EducationType> EducationTypes { get; set; }
    
    // Documents Module
    public DbSet<Document> Documents { get; set; }
    
    // ... diğer modüller
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Her modülün kendi configuration'ı
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppUser).Assembly); // Identity
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Client).Assembly); // Clients
        // ... diğer modüller
    }
}
```

### Module Registration

**Sorun:** Her modülü ayrı ayrı register etmek zor.

**Çözüm:** Extension method pattern

```csharp
// wixi.Identity/Extensions/ServiceCollectionExtensions.cs
public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        // ... diğer servisler
        return services;
    }
}

// wixi.WebAPI/Program.cs
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddClientsModule(builder.Configuration);
builder.Services.AddDocumentsModule(builder.Configuration);
// ... diğer modüller
```

### Controller Registration

**Sorun:** Controller'ları manuel register etmek zor.

**Çözüm:** Assembly scanning

```csharp
// wixi.WebAPI/Program.cs
builder.Services.AddControllers()
    .AddApplicationPart(typeof(AuthController).Assembly) // Identity
    .AddApplicationPart(typeof(ClientsController).Assembly) // Clients
    // ... diğer modüller
    .AddControllersAsServices();
```

---

## 📊 Migration Checklist

### Phase 1: Preparation
- [ ] Modül projeleri oluştur
- [ ] Solution'a ekle
- [ ] Dependencies belirle
- [ ] Migration planı dokümante et

### Phase 2: Entity Migration
- [ ] wixi.Identity entities
- [ ] wixi.Clients entities
- [ ] wixi.Documents entities
- [ ] wixi.Applications entities
- [ ] wixi.Support entities
- [ ] wixi.Content entities
- [ ] wixi.Email entities
- [ ] wixi.Forms entities

### Phase 3: Service Migration
- [ ] wixi.Identity services
- [ ] wixi.Clients services
- [ ] wixi.Documents services
- [ ] wixi.Applications services
- [ ] wixi.Support services
- [ ] wixi.Content services
- [ ] wixi.Email services
- [ ] wixi.Forms services

### Phase 4: Controller Migration
- [ ] wixi.Identity controllers
- [ ] wixi.Clients controllers
- [ ] wixi.Documents controllers
- [ ] wixi.Applications controllers
- [ ] wixi.Support controllers
- [ ] wixi.Content controllers
- [ ] wixi.Email controllers
- [ ] wixi.Forms controllers

### Phase 5: Cleanup
- [ ] Eski dosyaları sil
- [ ] Unused references kaldır
- [ ] Namespace'leri temizle
- [ ] Documentation güncelle

---

## ⚠️ Riskler ve Çözümler

### Risk 1: Breaking Changes
**Sorun:** Mevcut API'ler çalışmayabilir.

**Çözüm:**
- Incremental migration
- Her adımda test
- Feature flags kullan
- Rollback planı hazırla

### Risk 2: Circular Dependencies
**Sorun:** Modüller birbirine bağımlı olabilir.

**Çözüm:**
- Dependency graph çiz
- Shared interfaces kullan
- Event-driven communication
- wixi.Core'da shared interfaces

### Risk 3: Database Migration
**Sorun:** Entity taşıma database'i etkileyebilir.

**Çözüm:**
- Migration'ları dikkatli yap
- Test database'de test et
- Backup al
- Rollback migration hazırla

### Risk 4: Performance
**Sorun:** Modüler yapı performance'ı etkileyebilir.

**Çözüm:**
- Shared DbContext kullan
- Lazy loading'i kontrol et
- Caching stratejisi
- Performance testleri

---

## 🎯 Sonuç

### Avantajlar
- ✅ Modüler yapı
- ✅ Taşınabilirlik
- ✅ Bakım kolaylığı
- ✅ Test edilebilirlik
- ✅ Takım bağımsızlığı

### Dezavantajlar
- ❌ Initial migration effort
- ❌ Learning curve
- ❌ Potansiyel breaking changes

### Öneri
**Incremental migration** ile adım adım geçiş yap. Her modülü ayrı ayrı migrate et, test et, sonra diğerine geç.

---

## 📅 Tahmini Süre

| Phase | Süre | Açıklama |
|-------|------|----------|
| Phase 1: Preparation | 1 hafta | Proje yapısı, dependencies |
| Phase 2: Entity Migration | 2 hafta | Entity'leri taşıma |
| Phase 3: Service Migration | 2 hafta | Service'leri taşıma |
| Phase 4: Controller Migration | 1 hafta | Controller'ları taşıma |
| Phase 5: Cleanup | 3 gün | Temizlik ve düzenleme |
| **Toplam** | **~6 hafta** | Part-time çalışma |

---

## 🚀 Hızlı Başlangıç

### 1. İlk Modül: wixi.Identity
```bash
# Proje oluştur
dotnet new classlib -n wixi.Identity -f net8.0
dotnet sln add wixi.Identity/wixi.Identity.csproj

# Dependencies ekle
cd wixi.Identity
dotnet add reference ../wixi.Core/wixi.Core.csproj
dotnet add reference ../wixi.DataAccess/wixi.DataAccess.csproj

# Klasör yapısı oluştur
mkdir Entities
mkdir Interfaces
mkdir Services
mkdir DTOs
mkdir Validators
mkdir Extensions
mkdir Controllers  # Opsiyonel

# Entity'leri taşı
# Interface'leri taşı (Interfaces/ klasörüne)
# Service'leri taşı (Services/ klasörüne)
# Test et
```

### 2. Diğer Modüller
Aynı pattern'i takip et:
1. Proje oluştur
2. Klasör yapısı oluştur (Entities, Interfaces, Services, DTOs, Validators, Extensions)
3. Entity'leri taşı
4. Interface'leri taşı (Interfaces/ klasörüne)
5. Service'leri taşı (Services/ klasörüne)
6. DTOs'ları taşı
7. Validator'ları taşı
8. Extension method oluştur (DI registration)
9. Test et

---

## 📋 Örnek: wixi.Identity Modülü Yapısı

### Klasör Yapısı
```
wixi.Identity/
├── Entities/
│   ├── AppUser.cs
│   ├── AppRole.cs
│   └── TokenBlacklist.cs
├── Interfaces/
│   ├── IAuthService.cs
│   └── ITokenService.cs
├── Services/
│   ├── AuthService.cs
│   └── TokenService.cs
├── DTOs/
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   └── RegisterRequest.cs
├── Validators/
│   ├── LoginRequestValidator.cs
│   └── RegisterRequestValidator.cs
├── Extensions/
│   └── ServiceCollectionExtensions.cs
├── Controllers/
│   └── AuthController.cs  # Opsiyonel - WebAPI'de de olabilir
└── wixi.Identity.csproj
```

### ServiceCollectionExtensions.cs
```csharp
// wixi.Identity/Extensions/ServiceCollectionExtensions.cs
using wixi.Identity.Interfaces;
using wixi.Identity.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace wixi.Identity.Extensions
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Interface'ler Interfaces/ klasöründen
            // Implementation'lar Services/ klasöründen
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            
            return services;
        }
    }
}
```

### AuthService.cs
```csharp
// wixi.Identity/Services/AuthService.cs
using wixi.Identity.Interfaces;
using wixi.Identity.Entities;
using wixi.Identity.DTOs;

namespace wixi.Identity.Services
{
    public class AuthService : IAuthService
    {
        // Implementation
    }
}
```

### IAuthService.cs
```csharp
// wixi.Identity/Interfaces/IAuthService.cs
using wixi.Identity.DTOs;

namespace wixi.Identity.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}
```

---

## 📚 İlgili Dokümanlar

### 🔐 Güvenlik ve Mikroservis Analizi

Detaylı güvenlik risk analizi, mikroservislere geçiş stratejisi ve eksiklikler için:

👉 **[MODULARIZATION-SECURITY-ANALYSIS.md](MODULARIZATION-SECURITY-ANALYSIS.md)**

**İçerik:**
- 🔒 Güvenlik risk analizi (Shared DbContext, Inter-module Communication, Authorization)
- 🚀 Mikroservislere geçiş kolaylığı analizi
- ⚠️ Kritik eksiklikler (Rate Limiting, Security Headers, Audit Logging)
- 📊 Mikroservislere geçiş stratejisi (5 Phase, 6-9 ay)
- 🎯 Güvenlik önerileri ve çözümleri

---

**Son Güncelleme:** Ocak 2025  
**Maintainer:** Wixi Backend Team

