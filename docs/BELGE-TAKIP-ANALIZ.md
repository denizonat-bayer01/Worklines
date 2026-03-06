# 📋 Belge Takip Sistemi - Implementasyon Planı

> **Branch:** takip-sistemi  
> **Tarih:** 4 Kasım 2025  
> **Durum:** Implementation Başlıyor 🚀

## 🎯 Proje Hedefi

Üyelerin belge yüklemesi, admin onayı ve başvuru takibi için backend sistemi oluşturmak.

---

## 📊 Eksik Entity'ler (16 Adet)

### ✅ FAZA 1: Müşteri ve Belge Yönetimi (7 Entity)

#### 1. Client (Müşteri Profili)
```csharp
public class Client
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ClientCode { get; set; }  // "WP-84321"
    public DateTime RegistrationDate { get; set; }
    public ClientStatus Status { get; set; }
}

public enum ClientStatus { Active, Inactive, Suspended, Completed }
```

#### 2. EducationType (Eğitim Tipleri)
```csharp
public class EducationType
{
    public int Id { get; set; }
    public string Code { get; set; }  // "university", "vocational", "masterCraftsman"
    public string Name { get; set; }
    public string NameEn { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
```

#### 3. EducationInfo (Eğitim Geçmişi)
```csharp
public class EducationInfo
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public EducationLevel Level { get; set; }
    public string Degree { get; set; }
    public string Institution { get; set; }
    public DateTime? GraduationDate { get; set; }
}

public enum EducationLevel 
{ 
    HighSchool, VocationalSchool, Bachelor, Master, PhD, 
    Apprenticeship, Mastership 
}
```

#### 4. DocumentType (Belge Türleri)
```csharp
public class DocumentType
{
    public int Id { get; set; }
    public string Code { get; set; }  // "passport", "diploma", "cv"
    public string Name { get; set; }
    public string NameEn { get; set; }
    public bool IsRequired { get; set; }
    public int? EducationTypeId { get; set; }
    public string? AllowedFileTypes { get; set; }  // ".pdf,.jpg,.png"
    public long? MaxFileSizeBytes { get; set; }
}
```

#### 5. Document (Yüklenen Belgeler)
```csharp
public class Document
{
    public long Id { get; set; }
    public int ClientId { get; set; }
    public int DocumentTypeId { get; set; }
    public string OriginalFileName { get; set; }
    public string StoredFileName { get; set; }
    public string FilePath { get; set; }
    public long FileSizeBytes { get; set; }
    public DocumentStatus Status { get; set; }
    public int Version { get; set; }
    public DateTime UploadedAt { get; set; }
}

public enum DocumentStatus 
{ 
    Pending, UnderReview, Accepted, Rejected, MissingInfo, Expired 
}
```

#### 6. DocumentReview (Belge İnceleme)
```csharp
public class DocumentReview
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public int ReviewerId { get; set; }
    public DocumentStatus Decision { get; set; }
    public string? ReviewNote { get; set; }
    public string? FeedbackMessage { get; set; }
    public DateTime ReviewedAt { get; set; }
}
```

#### 7. FileStorage (Dosya Metadata)
```csharp
public class FileStorage
{
    public long Id { get; set; }
    public string FileName { get; set; }
    public string StoredFileName { get; set; }
    public string FilePath { get; set; }
    public long FileSizeBytes { get; set; }
    public FileStorageType StorageType { get; set; }
    public string EntityType { get; set; }  // "Document", "ProfilePhoto"
    public long EntityId { get; set; }
    public DateTime UploadedAt { get; set; }
}

public enum FileStorageType { Local, AzureBlob, AWSS3 }
```

---

### ✅ FAZA 2: Başvuru Takip Sistemi (7 Entity)

#### 8. Application (Başvuru)
```csharp
public class Application
{
    public long Id { get; set; }
    public int ClientId { get; set; }
    public int ApplicationTemplateId { get; set; }
    public string ApplicationNumber { get; set; }  // "APP-2023-12345"
    public ApplicationType Type { get; set; }
    public ApplicationStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime StartDate { get; set; }
}

public enum ApplicationType { Recognition, WorkPermit, Visa, FullProcess }
public enum ApplicationStatus { Draft, Submitted, InProgress, Completed, Cancelled }
```

#### 9. ApplicationTemplate (Süreç Şablonları)
```csharp
public class ApplicationTemplate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameEn { get; set; }
    public ApplicationType Type { get; set; }
    public bool IsActive { get; set; }
}
```

#### 10. ApplicationStep (Başvuru Adımları)
```csharp
public class ApplicationStep
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public int StepTemplateId { get; set; }
    public string Title { get; set; }
    public int StepOrder { get; set; }
    public StepStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
}

public enum StepStatus { NotStarted, InProgress, Completed, Blocked, Skipped }
```

#### 11. ApplicationStepTemplate (Adım Şablonları)
```csharp
public class ApplicationStepTemplate
{
    public int Id { get; set; }
    public int ApplicationTemplateId { get; set; }
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public int StepOrder { get; set; }
    public string? IconName { get; set; }
}
```

#### 12. ApplicationSubStep (Alt Adımlar)
```csharp
public class ApplicationSubStep
{
    public long Id { get; set; }
    public long ApplicationStepId { get; set; }
    public int SubStepTemplateId { get; set; }
    public string Name { get; set; }
    public int SubStepOrder { get; set; }
    public StepStatus Status { get; set; }
    public string? FileNumber { get; set; }
    public DateTime? CompletionDate { get; set; }
}
```

#### 13. ApplicationSubStepTemplate (Alt Adım Şablonları)
```csharp
public class ApplicationSubStepTemplate
{
    public int Id { get; set; }
    public int StepTemplateId { get; set; }
    public string Name { get; set; }
    public string NameEn { get; set; }
    public int SubStepOrder { get; set; }
}
```

#### 14. ApplicationHistory (Durum Geçmişi)
```csharp
public class ApplicationHistory
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public string Action { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

### ✅ FAZA 3: Destek ve Bildirim (4 Entity)

#### 15. SupportTicket (Destek Talepleri)
```csharp
public class SupportTicket
{
    public long Id { get; set; }
    public int ClientId { get; set; }
    public string TicketNumber { get; set; }
    public string Subject { get; set; }
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public int? AssignedToId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum TicketStatus { Open, InProgress, WaitingForCustomer, Resolved, Closed }
public enum TicketPriority { Low, Normal, High, Urgent }
```

#### 16. SupportMessage (Destek Mesajları)
```csharp
public class SupportMessage
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public int SenderId { get; set; }
    public string Message { get; set; }
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### 17. FAQ (Sık Sorulan Sorular)
```csharp
public class FAQ
{
    public int Id { get; set; }
    public string Question { get; set; }
    public string QuestionEn { get; set; }
    public string Answer { get; set; }
    public string AnswerEn { get; set; }
    public FAQCategory Category { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public enum FAQCategory { General, Documents, Process, Communication }
```

#### 18. Notification (Bildirimler)
```csharp
public class Notification
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum NotificationType 
{ 
    DocumentApproved, DocumentRejected, StepCompleted, 
    ApplicationStatusChanged, NewMessage 
}
```

---

## 📁 Klasör Yapısı

```
wixi.Entities/Concrete/
├── Client/
│   ├── Client.cs
│   ├── EducationType.cs
│   └── EducationInfo.cs
│
├── Document/
│   ├── DocumentType.cs
│   ├── Document.cs
│   ├── DocumentReview.cs
│   └── FileStorage.cs
│
├── Application/
│   ├── Application.cs
│   ├── ApplicationTemplate.cs
│   ├── ApplicationStep.cs
│   ├── ApplicationStepTemplate.cs
│   ├── ApplicationSubStep.cs
│   ├── ApplicationSubStepTemplate.cs
│   └── ApplicationHistory.cs
│
└── Support/
    ├── SupportTicket.cs
    ├── SupportMessage.cs
    ├── FAQ.cs
    └── Notification.cs
```

---

## 📋 Implementation Adımları

### ✅ Adım 1: Klasör Yapısını Oluştur
```bash
cd wixi.Entities/Concrete
mkdir Client Document Application Support
```

### ✅ Adım 2: Entity Class'ları Oluştur
- Her entity için yukarıdaki kod şablonlarını kullan
- Navigation properties ekle
- Timestamps ekle (CreatedAt, UpdatedAt)

### ✅ Adım 3: DbContext'e Ekle
```csharp
// WixiDbContext.cs
public DbSet<Client> Clients { get; set; }
public DbSet<EducationType> EducationTypes { get; set; }
public DbSet<Document> Documents { get; set; }
public DbSet<DocumentType> DocumentTypes { get; set; }
// ... diğerleri
```

### ✅ Adım 4: Entity Configurations (Fluent API)
```csharp
// Örnek: ClientConfiguration.cs
public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ClientCode).IsRequired().HasMaxLength(50);
        builder.HasOne(c => c.User).WithOne().HasForeignKey<Client>(c => c.UserId);
        // ... diğer konfigürasyonlar
    }
}
```

### ✅ Adım 5: Migration Oluştur
```bash
dotnet ef migrations add AddDocumentTrackingSystem
dotnet ef database update
```

### ✅ Adım 6: Seed Data Hazırla
- EducationType: Üniversite, Meslek Lisesi, Kalfalık
- DocumentType: Pasaport, Diploma, CV vb. (her eğitim tipine göre)
- ApplicationTemplate: Denklik, İş, Vize süreçleri
- FAQ: Sık sorulan sorular

---

## 🔌 API Endpoints (FAZA 4)

### Document Management
```
POST   /api/documents/upload
GET    /api/documents/my-documents
DELETE /api/documents/{id}
GET    /api/documents/types/{educationType}
```

### Document Review (Admin)
```
GET    /api/admin/documents/pending
POST   /api/admin/documents/{id}/review
```

### Application Tracking
```
GET    /api/applications/my-applications
GET    /api/applications/{id}/steps
```

### Profile
```
GET    /api/clients/profile
PUT    /api/clients/profile
GET    /api/clients/education
```

### Support
```
POST   /api/support/tickets
GET    /api/support/tickets
GET    /api/support/faq
```

---

## ⏱️ Zaman Tahmini

| Faz | Süre |
|-----|------|
| **Faz 1:** Entity'ler + Migration | 2-3 gün |
| **Faz 2:** Başvuru Entity'leri | 2 gün |
| **Faz 3:** Destek Entity'leri | 1 gün |
| **Faz 4:** API Implementation | 1 hafta |
| **Faz 5:** File Upload Service | 2-3 gün |
| **Faz 6:** Test & Fix | 2-3 gün |
| **TOPLAM** | **2-3 hafta** |

---

## 🚀 Başlayalım!

İlk adım: **FAZA 1 - Client ve Document Entity'lerini oluşturmak**

Hazır mısınız? 💪

---

**Son Güncelleme:** 4 Kasım 2025  
**Branch:** takip-sistemi  
**Status:** 🟢 Ready to Start

