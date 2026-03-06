# 📋 Belge Takip ve Üyelik Sistemi - Analiz ve Eksiklikler

> **Proje:** Wixi Worklines - Belge Yönetim ve Başvuru Takip Sistemi  
> **Tarih:** 4 Kasım 2025  
> **Durum:** Analiz Tamamlandı ⚠️ Eksiklikler Tespit Edildi

## 📖 İçindekiler

1. [Proje Mantığı](#proje-mantığı)
2. [Frontend Analizi](#frontend-analizi)
3. [Backend Analizi](#backend-analizi)
4. [Eksik Tablolar ve Entity'ler](#eksik-tablolar-ve-entityler)
5. [Önerilen Veritabanı Şeması](#önerilen-veritabanı-şeması)
6. [Implementation Planı](#implementation-planı)
7. [API Endpoints](#api-endpoints)

---

## 🎯 Proje Mantığı

### Genel Amaç
Bir üyelik ve belge takip sistemi oluşturmak. Sistem şu şekilde çalışıyor:

1. **Üyeler (Clients)** sisteme giriş yapıyor
2. **Belgelerini yüklüyorlar** (pasaport, diploma, CV vb.)
3. **Admin panelinde** bu belgeler inceleniyor
4. Belgeler **onaylanıyor veya reddediliyor**
5. Reddedilen belgeler için **geri bildirim** veriliyor
6. Üye **yeniden yükleme** yapabiliyor
7. **Başvuru süreci** adım adım takip ediliyor
8. Üyeler **Dashboard'dan** süreçlerini görebiliyor

### İş Akışı

```
┌─────────────────────────────────────────────────────────────┐
│                    KULLANICI TARAFINDAN                      │
└─────────────────────────────────────────────────────────────┘
                            ↓
        ┌──────────────────────────────────────┐
        │  1. Kayıt & Profil Oluşturma         │
        │     - Kişisel Bilgiler               │
        │     - Eğitim Bilgileri               │
        │     - İletişim Bilgileri             │
        └───────────────┬──────────────────────┘
                        ↓
        ┌──────────────────────────────────────┐
        │  2. Belge Yükleme                    │
        │     - Eğitim Tipine Göre Belgeler    │
        │     - Pasaport, Diploma, CV vb.      │
        │     - PDF/Image Format               │
        └───────────────┬──────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│                     ADMIN TARAFINDAN                         │
└─────────────────────────────────────────────────────────────┘
                        ↓
        ┌──────────────────────────────────────┐
        │  3. Belge İnceleme                   │
        │     - Belgeleri Görüntüleme          │
        │     - Doğruluk Kontrolü              │
        │     - Onay/Red Kararı                │
        └───────────────┬──────────────────────┘
                        ↓
            ┌───────────┴───────────┐
            ↓                       ↓
    ┌──────────────┐        ┌──────────────┐
    │  4a. ONAY    │        │  4b. RED     │
    │              │        │              │
    │  - Onaylandı │        │  - Mesaj     │
    │  - Sonraki   │        │  - Yeniden   │
    │    Adım      │        │    Yükle     │
    └──────┬───────┘        └──────┬───────┘
           │                       │
           │                       │
           └───────────┬───────────┘
                       ↓
        ┌──────────────────────────────────────┐
        │  5. Başvuru Süreci Başlatılıyor      │
        │     - Denklik İşlemleri              │
        │     - İş Bulma & Çalışma İzni        │
        │     - Vize İşlemleri                 │
        └───────────────┬──────────────────────┘
                        ↓
        ┌──────────────────────────────────────┐
        │  6. Süreç Takibi (Dashboard)         │
        │     - Ana Adımlar (MainStep)         │
        │     - Alt Adımlar (SubStep)          │
        │     - İlerleme Durumu                │
        │     - Dosya Numaraları               │
        └──────────────────────────────────────┘
```

---

## 🎨 Frontend Analizi

### 1. Dashboard.tsx - Başvuru Takip Ekranı

**Amaç:** Kullanıcıların başvuru süreçlerini takip etmesi

**Veri Modeli:**
```typescript
interface MainStep {
  id: number;
  title: string;           // "Denklik İşlemleri"
  icon: React.ReactElement;
  progress: number;        // 0-100
  status: 'completed' | 'in-progress' | 'pending';
  subSteps: SubStep[];
}

interface SubStep {
  name: string;            // "Belgeler Yüklendi"
  status: 'completed' | 'in-progress' | 'pending';
  date?: string;           // "12/08/2023"
  info?: string;           // Ek bilgi mesajı
  fileNumber?: string;     // "DEU-2023-45678"
}
```

**Örnek Süreç Adımları:**
1. **Denklik İşlemleri**
   - Belgeler Yüklendi
   - Denklik Başvurusu Yapıldı
   - Denklik Harc Ödemesi Yapıldı
   - Denklik Belgesi Hazır

2. **İş Bulma ve Çalışma Müsadesi**
   - İş Sözleşmesi Sunuldu
   - İş Sözleşmesi İmzalandı
   - Çalışma Müsadesi Başvurusu Yapıldı
   - Çalışma Müsadesi Hazır

3. **Vize İşlemleri**
   - Vize Başvurusu Yapıldı
   - Vize Başvurusu Cevabı Geldi
   - Vize Ön Onay Geldi
   - Konsolosluk Randevu Talebi Oluşturuldu
   - Konsolosluk Randevusu Gerçekleşti
   - Vize Çıktı

**Backend İhtiyacı:**
- ❌ `Application` entity (Başvuru)
- ❌ `ApplicationStep` entity (Ana adımlar)
- ❌ `ApplicationSubStep` entity (Alt adımlar)
- ❌ `ApplicationHistory` entity (Durum değişikliği geçmişi)
- ❌ `ApplicationTemplate` entity (Süreç şablonları)

---

### 2. Documents.tsx - Belge Yükleme Ekranı

**Amaç:** Eğitim durumuna göre belge yükleme

**Eğitim Tipleri:**
1. **Üniversite Mezunu** (`university`)
   - Pasaport (Renkli Fotokopi - PDF)
   - CV (Almanca veya İngilizce)
   - İkamet Belgesi (E-Devlet PDF)
   - Üniversite Diploması
   - İkinci/Üçüncü Üniversite Diploması (Varsa)
   - YÖK Mezun Belgesi (E-Devlet)
   - Supplement - Transkript (E-Devlet)
   - Formül B - Evlilik Belgesi (Evli Kadınlar İçin)
   - Lise Diploması/Mezun Belgesi (E-Devlet)
   - Diğer Sertifikalar (Varsa)

2. **Meslek Lisesi Mezunu** (`vocational`)
   - Pasaport (Renkli Fotokopi - PDF)
   - CV (Almanca veya İngilizce)
   - Meslek Lisesi Diploması
   - Lise Mezun Belgesi (E-Devlet)
   - SGK Hizmet Dökümü (E-Devlet)
   - İkamet Belgesi (E-Devlet PDF)
   - Formül B - Evlilik Belgesi
   - Diğer Sertifikalar (Varsa)

3. **Kalfalık/Ustalık Belgesi** (`masterCraftsman`)
   - Pasaport (Renkli Fotokopi - PDF)
   - CV (Almanca veya İngilizce)
   - Kalfalık Belgesi
   - Ustalık Belgesi (Varsa)
   - Kalfalık Belgesi Transkripti
   - Ustalık Belgesi Transkripti (Varsa)
   - Kalfalık Belgesi E-Devlet Çıktısı
   - Ustalık Belgesi E-Devlet Çıktısı (Varsa)
   - İkamet Belgesi (E-Devlet PDF)
   - Formül B - Evlilik Belgesi
   - Diğer Sertifikalar (Varsa)

**Veri Modeli:**
```typescript
interface UploadedFile {
  id: string;
  name: string;              // Dosya adı
  size: string;              // "2.5 MB"
  date: string;              // Yükleme tarihi
  category: string;          // Eğitim tipi
  documentType: string;      // Belge türü
}
```

**Backend İhtiyacı:**
- ❌ `Client` entity (Müşteri profili)
- ❌ `EducationType` entity (Eğitim tipleri)
- ❌ `DocumentType` entity (Belge türleri)
- ❌ `Document` entity (Yüklenen belgeler)
- ❌ `FileStorage` entity (Dosya metadata)
- ❌ File Upload Service (Dosya yükleme servisi)

---

### 3. Profile.tsx - Profil ve Belge Durumu Ekranı

**Amaç:** Kullanıcı profili ve yüklenen belgelerin durumunu gösterme

**Veri Modeli:**
```typescript
interface Document {
  id: string;
  name: string;              // "Passport.pdf"
  type: string;              // "description", "school", "badge"
  uploadDate: string;        // "Sep 15, 2023"
  status: 'accepted' | 'rejected' | 'pending' | 'missing-info';
  icon: string;
  message?: string;          // Red/Eksik bilgi mesajı
}

interface Education {
  degree: string;            // "Master of Science in Data Science"
  institution: string;       // "Stanford University"
  graduationDate: string;    // "May 2022"
}
```

**Belge Durumları:**
- ✅ **Accepted**: Belge onaylandı
- ❌ **Rejected**: Belge reddedildi (mesaj var)
- ⏳ **Pending**: İnceleme aşamasında
- ⚠️ **Missing Info**: Eksik bilgi var (mesaj var)

**Backend İhtiyacı:**
- ❌ `DocumentReview` entity (Belge inceleme/onay)
- ❌ `DocumentStatus` enum
- ❌ `ReviewMessage` entity (Geri bildirim mesajları)
- ❌ `EducationInfo` entity (Eğitim geçmişi)

---

### 4. Support.tsx - Destek Merkezi

**Amaç:** Kullanıcıların destek talebi oluşturması ve FAQ

**Veri Modeli:**
```typescript
interface SupportMessage {
  name: string;
  email: string;
  subject: string;
  message: string;
}

interface FAQ {
  question: string;
  answer: string;
  category: string;          // "Genel", "Belgeler", "Süreç"
}
```

**Backend İhtiyacı:**
- ❌ `SupportTicket` entity (Destek talepleri)
- ❌ `SupportMessage` entity (Destek mesajları)
- ❌ `FAQ` entity (Sık sorulan sorular)
- ❌ `TicketStatus` enum

---

## 🔧 Backend Analizi

### Mevcut Entity'ler

```
wixi.Entities/Concrete/
├── AppUser.cs                    ✅ Kullanıcı (Identity)
├── AppRole.cs                    ✅ Roller
├── TokenBlacklist.cs             ✅ Token yönetimi
├── ContactFormSubmission.cs      ✅ İletişim formu
├── EmployeeFormSubmission.cs     ✅ Çalışan formu
├── EmployerFormSubmission.cs     ✅ İşveren formu
├── EmailLog.cs                   ✅ Email kayıtları
├── EmailTemplate.cs              ✅ Email şablonları
├── SmtpSettings.cs               ✅ SMTP ayarları
├── NewsItem.cs                   ✅ Haberler
├── TeamMember.cs                 ✅ Ekip üyeleri
├── ContentSettings.cs            ✅ İçerik ayarları
├── SystemSettings.cs             ✅ Sistem ayarları
├── Translation.cs                ✅ Çeviriler
├── UserPreference.cs             ✅ Kullanıcı tercihleri
└── ApplicationLog.cs             ✅ Uygulama logları
```

### ❌ Eksik Entity'ler

Belge takip ve başvuru yönetimi için gerekli entity'ler:

```
EKSIK TABLOLAR:
├── Client.cs                     ❌ Müşteri profili
├── EducationType.cs              ❌ Eğitim tipleri
├── EducationInfo.cs              ❌ Eğitim geçmişi
├── DocumentType.cs               ❌ Belge türleri
├── Document.cs                   ❌ Yüklenen belgeler
├── DocumentReview.cs             ❌ Belge inceleme/onay
├── FileStorage.cs                ❌ Dosya metadata
├── Application.cs                ❌ Başvurular
├── ApplicationTemplate.cs        ❌ Süreç şablonları
├── ApplicationStep.cs            ❌ Ana süreç adımları
├── ApplicationSubStep.cs         ❌ Alt süreç adımları
├── ApplicationHistory.cs         ❌ Durum geçmişi
├── Notification.cs               ❌ Bildirimler
├── SupportTicket.cs              ❌ Destek talepleri
├── SupportMessage.cs             ❌ Destek mesajları
└── FAQ.cs                        ❌ SSS
```

---

## 📊 Eksik Tablolar ve Entity'ler

### 1. **Client (Müşteri Profili)**

```csharp
public class Client
{
    public int Id { get; set; }
    public int UserId { get; set; }  // FK to AppUser
    public virtual AppUser User { get; set; }
    
    // Personal Info
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; }
    
    // Client-specific
    public string ClientCode { get; set; }  // WP-84321
    public DateTime RegistrationDate { get; set; }
    public ClientStatus Status { get; set; }
    
    // Relations
    public virtual ICollection<Document> Documents { get; set; }
    public virtual ICollection<Application> Applications { get; set; }
    public virtual ICollection<EducationInfo> EducationHistory { get; set; }
    public virtual ICollection<SupportTicket> SupportTickets { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum ClientStatus
{
    Active,
    Inactive,
    Suspended,
    Completed
}
```

---

### 2. **EducationType (Eğitim Tipleri)**

```csharp
public class EducationType
{
    public int Id { get; set; }
    public string Code { get; set; }          // "university", "vocational", "masterCraftsman"
    public string Name { get; set; }          // "Üniversite Mezunu"
    public string NameEn { get; set; }        // "University Graduate"
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    
    // Relations
    public virtual ICollection<DocumentType> RequiredDocuments { get; set; }
    public virtual ICollection<Client> Clients { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

---

### 3. **EducationInfo (Eğitim Geçmişi)**

```csharp
public class EducationInfo
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public virtual Client Client { get; set; }
    
    public EducationLevel Level { get; set; }  // HighSchool, Bachelor, Master, PhD
    public string Degree { get; set; }         // "Master of Science in Data Science"
    public string Institution { get; set; }    // "Stanford University"
    public string? FieldOfStudy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? GraduationDate { get; set; }
    public string? Country { get; set; }
    public bool IsCurrent { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum EducationLevel
{
    HighSchool,
    VocationalSchool,
    Associate,
    Bachelor,
    Master,
    PhD,
    Apprenticeship,    // Kalfalık
    Mastership         // Ustalık
}
```

---

### 4. **DocumentType (Belge Türleri)**

```csharp
public class DocumentType
{
    public int Id { get; set; }
    public string Code { get; set; }          // "passport", "diploma", "cv"
    public string Name { get; set; }          // "Pasaport (Renkli Fotokopi - PDF)"
    public string NameEn { get; set; }
    public string? Description { get; set; }
    public bool IsRequired { get; set; }      // Zorunlu mu?
    public bool IsActive { get; set; }
    
    // Education type relationship
    public int? EducationTypeId { get; set; }
    public virtual EducationType? EducationType { get; set; }
    
    // Validation rules
    public string? AllowedFileTypes { get; set; }  // ".pdf,.jpg,.png"
    public long? MaxFileSizeBytes { get; set; }    // 10485760 (10MB)
    public bool RequiresApproval { get; set; }
    
    // Display
    public int DisplayOrder { get; set; }
    public string? IconName { get; set; }
    public string? Note { get; set; }         // "20€ ücret karşılığında..."
    
    // Relations
    public virtual ICollection<Document> Documents { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

---

### 5. **Document (Yüklenen Belgeler)**

```csharp
public class Document
{
    public long Id { get; set; }
    public int ClientId { get; set; }
    public virtual Client Client { get; set; }
    
    public int DocumentTypeId { get; set; }
    public virtual DocumentType DocumentType { get; set; }
    
    // File info
    public string OriginalFileName { get; set; }
    public string StoredFileName { get; set; }     // GUID-based name
    public string FilePath { get; set; }
    public string FileExtension { get; set; }
    public long FileSizeBytes { get; set; }
    public string? MimeType { get; set; }
    
    // Status
    public DocumentStatus Status { get; set; }
    public int Version { get; set; }               // Kaçıncı yükleme
    
    // Metadata
    public string? UploadedFromIp { get; set; }
    public string? UserAgent { get; set; }
    
    // Relations
    public virtual DocumentReview? Review { get; set; }
    
    // Timestamps
    public DateTime UploadedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }       // Soft delete
}

public enum DocumentStatus
{
    Pending,       // Yüklendi, inceleme bekliyor
    UnderReview,   // İnceleniyor
    Accepted,      // Onaylandı
    Rejected,      // Reddedildi
    MissingInfo,   // Eksik bilgi var
    Expired        // Süresi doldu
}
```

---

### 6. **DocumentReview (Belge İnceleme)**

```csharp
public class DocumentReview
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public virtual Document Document { get; set; }
    
    public int ReviewerId { get; set; }          // Admin UserId
    public virtual AppUser Reviewer { get; set; }
    
    public DocumentStatus Decision { get; set; }
    public string? ReviewNote { get; set; }      // Admin notu (internal)
    public string? FeedbackMessage { get; set; } // Kullanıcıya gösterilen mesaj
    
    // Timestamps
    public DateTime ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

---

### 7. **FileStorage (Dosya Metadata)**

```csharp
public class FileStorage
{
    public long Id { get; set; }
    public string FileName { get; set; }
    public string StoredFileName { get; set; }
    public string FilePath { get; set; }
    public string FileExtension { get; set; }
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; }
    
    // Storage info
    public FileStorageType StorageType { get; set; }  // Local, Azure, AWS
    public string? StorageUrl { get; set; }
    public string? ContainerName { get; set; }
    
    // Security
    public string? FileHash { get; set; }            // SHA256
    public bool IsPublic { get; set; }
    
    // Relations (polymorphic)
    public string EntityType { get; set; }           // "Document", "ProfilePhoto"
    public long EntityId { get; set; }
    
    // Timestamps
    public DateTime UploadedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public enum FileStorageType
{
    Local,
    AzureBlob,
    AWSS3
}
```

---

### 8. **Application (Başvuru)**

```csharp
public class Application
{
    public long Id { get; set; }
    public int ClientId { get; set; }
    public virtual Client Client { get; set; }
    
    public int ApplicationTemplateId { get; set; }
    public virtual ApplicationTemplate Template { get; set; }
    
    // Application info
    public string ApplicationNumber { get; set; }    // "APP-2023-12345"
    public ApplicationType Type { get; set; }
    public ApplicationStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    
    // Dates
    public DateTime StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }
    
    // Relations
    public virtual ICollection<ApplicationStep> Steps { get; set; }
    public virtual ICollection<ApplicationHistory> History { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum ApplicationType
{
    Recognition,        // Denklik İşlemleri
    WorkPermit,         // Çalışma İzni
    Visa,               // Vize İşlemleri
    FullProcess         // Tam Süreç
}

public enum ApplicationStatus
{
    Draft,
    Submitted,
    InProgress,
    Completed,
    Cancelled,
    OnHold
}
```

---

### 9. **ApplicationTemplate (Süreç Şablonları)**

```csharp
public class ApplicationTemplate
{
    public int Id { get; set; }
    public string Name { get; set; }           // "Denklik İşlem Süreci"
    public string NameEn { get; set; }
    public string? Description { get; set; }
    public ApplicationType Type { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    
    // Template steps
    public virtual ICollection<ApplicationStepTemplate> StepTemplates { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

---

### 10. **ApplicationStep (Başvuru Adımları)**

```csharp
public class ApplicationStep
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public virtual Application Application { get; set; }
    
    public int StepTemplateId { get; set; }
    public virtual ApplicationStepTemplate Template { get; set; }
    
    // Step info
    public string Title { get; set; }              // "Denklik İşlemleri"
    public int StepOrder { get; set; }
    public StepStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    
    // Dates
    public DateTime? StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    
    // Relations
    public virtual ICollection<ApplicationSubStep> SubSteps { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ApplicationStepTemplate
{
    public int Id { get; set; }
    public int ApplicationTemplateId { get; set; }
    public virtual ApplicationTemplate Template { get; set; }
    
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public string? Description { get; set; }
    public int StepOrder { get; set; }
    public string? IconName { get; set; }
    public bool IsRequired { get; set; }
    
    // Sub-step templates
    public virtual ICollection<ApplicationSubStepTemplate> SubStepTemplates { get; set; }
}

public enum StepStatus
{
    NotStarted,
    InProgress,
    Completed,
    Blocked,
    Skipped
}
```

---

### 11. **ApplicationSubStep (Alt Adımlar)**

```csharp
public class ApplicationSubStep
{
    public long Id { get; set; }
    public long ApplicationStepId { get; set; }
    public virtual ApplicationStep Step { get; set; }
    
    public int SubStepTemplateId { get; set; }
    public virtual ApplicationSubStepTemplate Template { get; set; }
    
    // Sub-step info
    public string Name { get; set; }               // "Belgeler Yüklendi"
    public int SubStepOrder { get; set; }
    public StepStatus Status { get; set; }
    
    // Additional info
    public string? FileNumber { get; set; }        // "DEU-2023-45678"
    public string? InfoMessage { get; set; }       // "Başvuru işleme alındı"
    public DateTime? CompletionDate { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ApplicationSubStepTemplate
{
    public int Id { get; set; }
    public int StepTemplateId { get; set; }
    public virtual ApplicationStepTemplate StepTemplate { get; set; }
    
    public string Name { get; set; }
    public string NameEn { get; set; }
    public string? Description { get; set; }
    public int SubStepOrder { get; set; }
    public bool IsRequired { get; set; }
}
```

---

### 12. **ApplicationHistory (Durum Geçmişi)**

```csharp
public class ApplicationHistory
{
    public long Id { get; set; }
    public long ApplicationId { get; set; }
    public virtual Application Application { get; set; }
    
    public string Action { get; set; }             // "StatusChanged", "StepCompleted"
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public string? Description { get; set; }
    
    // User info
    public int? UserId { get; set; }               // Değişikliği yapan (admin/system)
    public virtual AppUser? User { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
}
```

---

### 13. **Notification (Bildirimler)**

```csharp
public class Notification
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public virtual AppUser User { get; set; }
    
    public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string? ActionUrl { get; set; }
    
    // Status
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    
    // Priority
    public NotificationPriority Priority { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
}

public enum NotificationType
{
    DocumentApproved,
    DocumentRejected,
    DocumentMissingInfo,
    StepCompleted,
    ApplicationStatusChanged,
    NewMessage,
    System
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}
```

---

### 14. **SupportTicket (Destek Talepleri)**

```csharp
public class SupportTicket
{
    public long Id { get; set; }
    public int ClientId { get; set; }
    public virtual Client Client { get; set; }
    
    public string TicketNumber { get; set; }       // "TKT-2023-12345"
    public string Subject { get; set; }
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketCategory Category { get; set; }
    
    // Assignment
    public int? AssignedToId { get; set; }         // Admin UserId
    public virtual AppUser? AssignedTo { get; set; }
    
    // Dates
    public DateTime CreatedAt { get; set; }
    public DateTime? FirstResponseAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Relations
    public virtual ICollection<SupportMessage> Messages { get; set; }
}

public enum TicketStatus
{
    Open,
    InProgress,
    WaitingForCustomer,
    Resolved,
    Closed,
    Reopened
}

public enum TicketPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum TicketCategory
{
    General,
    Documents,
    Application,
    Technical,
    Billing,
    Other
}
```

---

### 15. **SupportMessage (Destek Mesajları)**

```csharp
public class SupportMessage
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public virtual SupportTicket Ticket { get; set; }
    
    public int SenderId { get; set; }
    public virtual AppUser Sender { get; set; }
    
    public string Message { get; set; }
    public bool IsInternal { get; set; }           // Sadece admin görür
    public bool IsFromClient { get; set; }
    
    // Attachments
    public string? AttachmentFileName { get; set; }
    public string? AttachmentPath { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

---

### 16. **FAQ (Sık Sorulan Sorular)**

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
    
    // Stats
    public int ViewCount { get; set; }
    public int HelpfulCount { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum FAQCategory
{
    General,
    Documents,
    Process,
    Communication,
    Technical,
    Billing
}
```

---

## 🗄️ Önerilen Veritabanı Şeması

### İlişkiler (Relationships)

```
AppUser (Identity)
    ↓ 1:1
Client
    ↓ 1:N
    ├── Documents
    │       ↓ 1:1
    │   DocumentReview
    │
    ├── EducationInfo
    │
    ├── Applications
    │       ↓ 1:N
    │   ApplicationSteps
    │       ↓ 1:N
    │   ApplicationSubSteps
    │
    └── SupportTickets
            ↓ 1:N
        SupportMessages

DocumentType
    ↓ 1:N
Documents

EducationType
    ↓ 1:N
    ├── DocumentTypes (required docs)
    └── Clients

ApplicationTemplate
    ↓ 1:N
ApplicationStepTemplate
    ↓ 1:N
ApplicationSubStepTemplate
```

### Database Diagram (Metin)

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   AppUser    │────→│   Client     │────→│  Document    │
│  (Identity)  │ 1:1 │              │ 1:N │              │
└──────────────┘     └──────────────┘     └──────┬───────┘
                            │                    │ 1:1
                            │ 1:N                ↓
                            │            ┌──────────────┐
                            │            │DocumentReview│
                            ↓            └──────────────┘
                     ┌──────────────┐
                     │ Application  │
                     └──────┬───────┘
                            │ 1:N
                            ↓
                     ┌──────────────┐
                     │AppStep       │
                     └──────┬───────┘
                            │ 1:N
                            ↓
                     ┌──────────────┐
                     │AppSubStep    │
                     └──────────────┘
```

---

## 📋 Implementation Planı

### Faz 1: Temel Entity'ler (1 Hafta)

**Öncelik: YÜKSEK** 🔴

1. ✅ **Client Entity** oluştur
2. ✅ **EducationType Entity** oluştur
3. ✅ **EducationInfo Entity** oluştur
4. ✅ **DocumentType Entity** oluştur
5. ✅ **Document Entity** oluştur
6. ✅ **DocumentReview Entity** oluştur
7. ✅ **FileStorage Entity** oluştur
8. ✅ Migration oluştur ve çalıştır
9. ✅ Seed data hazırla

**Checklist:**
- [ ] Entity class'ları oluştur
- [ ] DbContext'e ekle
- [ ] Entity configurations yaz (Fluent API)
- [ ] Migration oluştur
- [ ] Seed data SQL'i hazırla
- [ ] Test et

---

### Faz 2: Başvuru Sistemi (1 Hafta)

**Öncelik: YÜKSEK** 🔴

1. ✅ **Application Entity** oluştur
2. ✅ **ApplicationTemplate Entity** oluştur
3. ✅ **ApplicationStep Entity** oluştur
4. ✅ **ApplicationStepTemplate Entity** oluştur
5. ✅ **ApplicationSubStep Entity** oluştur
6. ✅ **ApplicationSubStepTemplate Entity** oluştur
7. ✅ **ApplicationHistory Entity** oluştur
8. ✅ Migration oluştur
9. ✅ Şablon süreçleri seed data olarak ekle

**Checklist:**
- [ ] Entity class'ları oluştur
- [ ] İlişkileri tanımla
- [ ] Migration oluştur
- [ ] Süreç şablonları hazırla (Seed)
  - Denklik İşlemleri
  - İş Bulma & Çalışma İzni
  - Vize İşlemleri
- [ ] Test et

---

### Faz 3: Destek Sistemi (3-4 Gün)

**Öncelik: ORTA** 🟡

1. ✅ **SupportTicket Entity** oluştur
2. ✅ **SupportMessage Entity** oluştur
3. ✅ **FAQ Entity** oluştur
4. ✅ **Notification Entity** oluştur
5. ✅ Migration oluştur
6. ✅ FAQ seed data

**Checklist:**
- [ ] Entity class'ları oluştur
- [ ] Migration oluştur
- [ ] FAQ seed data hazırla
- [ ] Test et

---

### Faz 4: API & Business Logic (2 Hafta)

**Öncelik: YÜKSEK** 🔴

#### 4.1 Document Management API
- [ ] POST `/api/documents/upload` - Belge yükle
- [ ] GET `/api/documents/client/{clientId}` - Client belgeleri
- [ ] GET `/api/documents/{id}` - Belge detay
- [ ] DELETE `/api/documents/{id}` - Belge sil
- [ ] GET `/api/documents/types/{educationType}` - Eğitim tipine göre gerekli belgeler

#### 4.2 Document Review API (Admin)
- [ ] GET `/api/admin/documents/pending` - Onay bekleyen belgeler
- [ ] POST `/api/admin/documents/{id}/review` - Belge onay/red
- [ ] PUT `/api/admin/documents/{id}/request-info` - Eksik bilgi talebi

#### 4.3 Application API
- [ ] GET `/api/applications/client/{clientId}` - Client başvuruları
- [ ] GET `/api/applications/{id}` - Başvuru detay
- [ ] POST `/api/applications/create` - Yeni başvuru
- [ ] PUT `/api/applications/{id}/step/{stepId}/update` - Adım güncelle

#### 4.4 Support API
- [ ] POST `/api/support/tickets` - Destek talebi oluştur
- [ ] GET `/api/support/tickets` - Kullanıcı talepleri
- [ ] GET `/api/support/tickets/{id}` - Talep detay
- [ ] POST `/api/support/tickets/{id}/messages` - Mesaj gönder
- [ ] GET `/api/support/faq` - SSS listele

#### 4.5 Client Profile API
- [ ] GET `/api/clients/profile` - Profil bilgileri
- [ ] PUT `/api/clients/profile` - Profil güncelle
- [ ] GET `/api/clients/education` - Eğitim geçmişi
- [ ] POST `/api/clients/education` - Eğitim ekle

**Checklist:**
- [ ] Interface'ler oluştur (IDocumentService, etc.)
- [ ] Manager class'ları yaz
- [ ] Controller'lar oluştur
- [ ] DTO'lar hazırla
- [ ] Validation ekle (FluentValidation)
- [ ] Authorization ayarla
- [ ] Swagger documentation
- [ ] Test et

---

### Faz 5: File Upload Service (1 Hafta)

**Öncelik: YÜKSEK** 🔴

1. ✅ File Upload Service oluştur
2. ✅ Local storage implementation
3. ✅ File validation (type, size)
4. ✅ Unique file naming (GUID)
5. ✅ Secure file access
6. ⏳ Cloud storage (Azure Blob) - opsiyonel

**Checklist:**
- [ ] IFileStorageService interface
- [ ] LocalFileStorageService implementation
- [ ] File validation rules
- [ ] File path management
- [ ] Download endpoint oluştur
- [ ] Test et

---

### Faz 6: Notification System (3-4 Gün)

**Öncelik: ORTA** 🟡

1. ✅ Notification Service oluştur
2. ✅ Email notification
3. ✅ In-app notification
4. ⏳ Push notification (opsiyonel)

**Checklist:**
- [ ] INotificationService interface
- [ ] NotificationManager implementation
- [ ] Email templates oluştur
- [ ] Background job entegrasyonu (Hangfire)
- [ ] Test et

---

### Faz 7: Admin Panel API (1 Hafta)

**Öncelik: ORTA** 🟡

#### Admin Endpoints
- [ ] GET `/api/admin/clients` - Tüm clientlar
- [ ] GET `/api/admin/clients/{id}` - Client detay
- [ ] GET `/api/admin/applications` - Tüm başvurular
- [ ] PUT `/api/admin/applications/{id}/status` - Durum güncelle
- [ ] GET `/api/admin/dashboard/stats` - İstatistikler
- [ ] GET `/api/admin/documents/pending-count` - Bekleyen belge sayısı

**Checklist:**
- [ ] Admin controller'lar oluştur
- [ ] Admin authorization
- [ ] Dashboard statistics service
- [ ] Test et

---

## 🔌 API Endpoints

### Client Endpoints

```
┌─────────────────────────────────────────────────────────┐
│                   CLIENT API ENDPOINTS                   │
└─────────────────────────────────────────────────────────┘

📄 DOCUMENTS
  POST   /api/documents/upload
  GET    /api/documents/my-documents
  GET    /api/documents/{id}
  DELETE /api/documents/{id}
  GET    /api/documents/types
  GET    /api/documents/types/{educationType}

📋 APPLICATIONS
  GET    /api/applications/my-applications
  GET    /api/applications/{id}
  GET    /api/applications/{id}/steps
  GET    /api/applications/{id}/history

👤 PROFILE
  GET    /api/clients/profile
  PUT    /api/clients/profile
  GET    /api/clients/education
  POST   /api/clients/education
  PUT    /api/clients/education/{id}
  DELETE /api/clients/education/{id}

💬 SUPPORT
  POST   /api/support/tickets
  GET    /api/support/tickets
  GET    /api/support/tickets/{id}
  POST   /api/support/tickets/{id}/messages
  GET    /api/support/faq

🔔 NOTIFICATIONS
  GET    /api/notifications
  PUT    /api/notifications/{id}/read
  PUT    /api/notifications/read-all
```

### Admin Endpoints

```
┌─────────────────────────────────────────────────────────┐
│                   ADMIN API ENDPOINTS                    │
└─────────────────────────────────────────────────────────┘

👥 CLIENT MANAGEMENT
  GET    /api/admin/clients
  GET    /api/admin/clients/{id}
  PUT    /api/admin/clients/{id}/status

📄 DOCUMENT REVIEW
  GET    /api/admin/documents/pending
  GET    /api/admin/documents/{id}
  POST   /api/admin/documents/{id}/review
  PUT    /api/admin/documents/{id}/request-info

📋 APPLICATION MANAGEMENT
  GET    /api/admin/applications
  GET    /api/admin/applications/{id}
  PUT    /api/admin/applications/{id}/step/{stepId}/update
  PUT    /api/admin/applications/{id}/status
  POST   /api/admin/applications/{id}/history

💬 SUPPORT MANAGEMENT
  GET    /api/admin/support/tickets
  GET    /api/admin/support/tickets/{id}
  PUT    /api/admin/support/tickets/{id}/assign
  PUT    /api/admin/support/tickets/{id}/status
  POST   /api/admin/support/tickets/{id}/reply

📊 DASHBOARD & STATS
  GET    /api/admin/dashboard/stats
  GET    /api/admin/dashboard/recent-activities
  GET    /api/admin/documents/pending-count
  GET    /api/admin/applications/active-count

⚙️ SYSTEM MANAGEMENT
  GET    /api/admin/templates/application
  POST   /api/admin/templates/application
  PUT    /api/admin/templates/application/{id}
  GET    /api/admin/document-types
  POST   /api/admin/document-types
  PUT    /api/admin/document-types/{id}
```

---

## 📊 Örnek API Response Models

### Document Upload Response
```json
{
  "success": true,
  "message": "Belge başarıyla yüklendi",
  "data": {
    "id": 123,
    "documentTypeId": 1,
    "documentTypeName": "Pasaport",
    "fileName": "passport.pdf",
    "fileSize": 2457600,
    "status": "Pending",
    "uploadedAt": "2025-11-04T10:30:00Z"
  }
}
```

### Application Status Response
```json
{
  "success": true,
  "data": {
    "applicationId": 456,
    "applicationNumber": "APP-2023-12345",
    "type": "FullProcess",
    "status": "InProgress",
    "progressPercentage": 63,
    "steps": [
      {
        "id": 1,
        "title": "Denklik İşlemleri",
        "status": "Completed",
        "progress": 100,
        "subSteps": [
          {
            "id": 1,
            "name": "Belgeler Yüklendi",
            "status": "Completed",
            "completionDate": "2023-08-12"
          },
          {
            "id": 2,
            "name": "Denklik Başvurusu Yapıldı",
            "status": "Completed",
            "fileNumber": "DEU-2023-45678",
            "completionDate": "2023-08-15"
          }
        ]
      },
      {
        "id": 2,
        "title": "İş Bulma ve Çalışma Müsadesi İşlemleri",
        "status": "InProgress",
        "progress": 50,
        "subSteps": [
          {
            "id": 3,
            "name": "İş Sözleşmesi Sunuldu",
            "status": "Completed",
            "completionDate": "2023-10-01"
          },
          {
            "id": 4,
            "name": "Çalışma Müsadesi Başvurusu Yapıldı",
            "status": "InProgress",
            "infoMessage": "Başvuru işleme alındı"
          }
        ]
      }
    ]
  }
}
```

### Document Review Response
```json
{
  "success": true,
  "data": {
    "documentId": 123,
    "status": "Rejected",
    "reviewedBy": "Admin User",
    "feedbackMessage": "Belge bulanık ve okunamıyor. Lütfen yüksek çözünürlüklü tarama yükleyin.",
    "reviewedAt": "2023-09-16T14:30:00Z"
  }
}
```

---

## 🎯 Özet ve Sonuç

### Mevcut Durum
- ✅ Temel authentication/authorization var
- ✅ Email sistemi var (template + log)
- ✅ Form submission sistemi var
- ❌ **Belge yönetimi YOK**
- ❌ **Başvuru takip sistemi YOK**
- ❌ **Client profil yönetimi YOK**
- ❌ **Destek ticket sistemi YOK**

### Eksikler
Bu proje mantığı için backend'de toplam **16 yeni entity** gerekiyor:

1. ✅ Client
2. ✅ EducationType
3. ✅ EducationInfo
4. ✅ DocumentType
5. ✅ Document
6. ✅ DocumentReview
7. ✅ FileStorage
8. ✅ Application
9. ✅ ApplicationTemplate
10. ✅ ApplicationStep
11. ✅ ApplicationStepTemplate
12. ✅ ApplicationSubStep
13. ✅ ApplicationSubStepTemplate
14. ✅ ApplicationHistory
15. ✅ Notification
16. ✅ SupportTicket
17. ✅ SupportMessage
18. ✅ FAQ

### İş Yükü Tahmini

| Faz | Süre | Öncelik |
|-----|------|---------|
| **Faz 1:** Temel Entity'ler | 1 hafta | 🔴 YÜKSEK |
| **Faz 2:** Başvuru Sistemi | 1 hafta | 🔴 YÜKSEK |
| **Faz 3:** Destek Sistemi | 3-4 gün | 🟡 ORTA |
| **Faz 4:** API & Business Logic | 2 hafta | 🔴 YÜKSEK |
| **Faz 5:** File Upload Service | 1 hafta | 🔴 YÜKSEK |
| **Faz 6:** Notification System | 3-4 gün | 🟡 ORTA |
| **Faz 7:** Admin Panel API | 1 hafta | 🟡 ORTA |

**Toplam Tahmini Süre:** 6-7 hafta (tam zamanlı)

### Öncelikli Yapılacaklar

1. **İlk Adım:** Entity'leri oluştur ve migration çalıştır
2. **İkinci Adım:** File upload servisi
3. **Üçüncü Adım:** Document management API
4. **Dördüncü Adım:** Application tracking API
5. **Beşinci Adım:** Admin review sistemi

---

## 📞 Sonraki Adımlar

1. ✅ Bu analiz dökümanı oluşturuldu
2. ⏳ Entity class'larını oluştur
3. ⏳ Migration çalıştır
4. ⏳ Seed data hazırla
5. ⏳ API implementasyonuna başla

---

**📌 Not:** Bu dokümanda tüm entity'ler, ilişkiler ve API endpoint'ler detaylı olarak tanımlanmıştır. Implementation sırasında bu dokümanı referans alabilirsiniz.

**Son Güncelleme:** 4 Kasım 2025  
**Maintainer:** Wixi Backend Team

