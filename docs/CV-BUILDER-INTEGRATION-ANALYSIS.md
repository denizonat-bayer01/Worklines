# CV Builder Entegrasyon Analizi

## 📋 İçindekiler
1. [Genel Bakış](#genel-bakış)
2. [Mevcut Sistem Analizi](#mevcut-sistem-analizi)
3. [CvOlusturucu Projesi Analizi](#cvolusturucu-projesi-analizi)
4. [Entegrasyon Gereksinimleri](#entegrasyon-gereksinimleri)
5. [Teknik Mimari](#teknik-mimari)
6. [Veri Akışı](#veri-akışı)
7. [API Entegrasyonları](#api-entegrasyonları)
8. [Ödeme Akışı](#ödeme-akışı)
9. [ATS Uyumluluk Analizi](#ats-uyumluluk-analizi)
10. [Uygulama Adımları](#uygulama-adımları)
11. [Riskler ve Çözümler](#riskler-ve-çözümler)

---

## Genel Bakış

Bu doküman, **CvOlusturucu** projesinin mevcut **Worklines** sistemine entegrasyonu için detaylı bir analiz ve uygulama planı sunmaktadır.

### Entegrasyon Hedefi
- Client'ların yüklediği belgelerde CV tespiti
- ATS (Applicant Tracking System) uyumluluk yüzdesi gösterimi
- "CV Oluştur" butonu ile ödeme akışı (20 Euro)
- iyzico ile ödeme entegrasyonu
- Ödeme sonrası CV Builder modülüne yönlendirme

### ⚠️ ÖNEMLİ: Entegrasyon Yaklaşımı
**CvOlusturucu projesi ayrı bir servis değil, Worklines sistemine entegre edilecek bir modüldür.**
- Frontend component'leri V4 projesine taşınacak
- Backend fonksiyonları ASP.NET Core'a port edilecek
- Express.js backend'i kaldırılacak
- Tek bir monolith sistem olacak

---

## Mevcut Sistem Analizi

### Worklines Backend (ASP.NET Core)

#### Belge Yönetim Sistemi
- **Controller**: `DocumentsController`
- **Service**: `DocumentService`
- **Entity**: `Document` (wixi.Documents.Entities)
- **Endpoint**: `POST /api/v1.0/documents/client/{clientId}`
- **Özellikler**:
  - Belge yükleme ve versiyonlama
  - Belge tipi yönetimi (DocumentType)
  - Soft delete desteği
  - Client bazlı belge listeleme

#### Ödeme Sistemi
- **Controller**: `PaymentsController`
- **Service**: `PaymentService`
- **Provider**: iyzico
- **Endpoint**: `POST /api/v1.0/payments`
- **Özellikler**:
  - Kredi kartı ödemesi
  - EUR/TRY dönüşümü
  - Ödeme durumu takibi
  - Webhook desteği
  - Appointment/Application entegrasyonu

#### Veritabanı Yapısı
```sql
-- Documents Tablosu
- Id (PK)
- ClientId (FK)
- DocumentTypeId (FK)
- OriginalFileName
- StoredFileName
- FilePath
- FileExtension
- FileSizeBytes
- MimeType
- Status (Pending, Approved, Rejected)
- Version
- UploadedAt
- DeletedAt (Soft Delete)

-- Payments Tablosu
- Id (PK)
- PaymentNumber
- ClientId (FK)
- Amount
- PaidAmount
- Currency
- Status (Pending, Completed, Failed, Cancelled, Refunded)
- PaymentType
- RelatedEntityType
- RelatedEntityId
- IyzicoPaymentId
- CreatedAt
- PaidAt
```

### Worklines Frontend (React - V4)

#### Belge Yönetim Sayfası
- **Dosya**: `V4/src/pages/Client/Documents.tsx`
- **Özellikler**:
  - Belge yükleme formu
  - Belge listesi (kart görünümü)
  - Belge durumu gösterimi
  - Belge indirme
  - Belge silme

#### Ödeme Entegrasyonu
- **Service**: `PaymentService` (V4/src/ApiServices/services)
- **Özellikler**:
  - Ödeme oluşturma
  - Ödeme durumu sorgulama
  - iyzico form entegrasyonu

---

## CvOlusturucu Projesi Analizi

### ⚠️ ÖNEMLİ: Entegrasyon Yaklaşımı
**CvOlusturucu projesi ayrı bir servis olarak çalışmayacak!**
- ✅ Frontend component'leri **V4 projesine** taşınacak
- ✅ Backend fonksiyonları **ASP.NET Core'a** port edilecek
- ❌ Express.js backend'i **kaldırılacak**
- ❌ Ayrı veritabanı **kullanılmayacak** (SQL Server kullanılacak)
- ✅ Tek bir monolith sistem olacak

### Mevcut Teknoloji Stack (Referans)
- **Frontend**: React 18 + TypeScript + Vite
- **Backend**: Express.js + TypeScript (KALDIRILACAK)
- **ORM**: Drizzle ORM (KALDIRILACAK)
- **Database**: PostgreSQL (Neon) (KALDIRILACAK)
- **UI Framework**: Radix UI + Tailwind CSS (KORUNACAK)
- **Form Management**: React Hook Form + Zod (KORUNACAK)

### Entegrasyon Sonrası Yapı
```
Worklines V4 (Frontend)
├── src/
│   ├── components/
│   │   ├── cv-builder/              (YENİ - CvOlusturucu'dan taşınacak)
│   │   │   ├── PersonalInfoForm.tsx
│   │   │   ├── ExperienceForm.tsx
│   │   │   ├── EducationForm.tsx
│   │   │   ├── SkillsForm.tsx
│   │   │   └── CVPreview.tsx
│   │   └── ... (mevcut component'ler)
│   ├── pages/
│   │   ├── Client/
│   │   │   ├── Documents.tsx        (Güncellenecek - CV tespiti eklenecek)
│   │   │   └── CVBuilder.tsx        (YENİ - CvOlusturucu Builder.tsx'den)
│   │   └── ... (mevcut sayfalar)
│   └── ApiServices/
│       └── services/
│           └── CVBuilderService.ts (YENİ)

Worklines Backend (ASP.NET Core)
├── wixi.CVBuilder/                  (YENİ Modül)
│   ├── Entities/
│   │   └── CVData.cs
│   ├── DTOs/
│   │   ├── CVDataDto.cs
│   │   └── SaveCVDataDto.cs
│   ├── Services/
│   │   ├── ICVBuilderService.cs
│   │   └── CVBuilderService.cs
│   └── Controllers/
│       └── CVBuilderController.cs
├── wixi.Documents/                   (Güncellenecek)
│   └── Services/
│       └── DocumentAnalysisService.cs (YENİ - CV tespiti ve ATS skoru)
└── wixi.Payments/                    (Güncellenecek)
    └── Entities/
        └── Payment.cs                (PaymentType.CVBuilder eklenecek)
```

### CV Veri Yapısı
```typescript
interface CVData {
  personalInfo: {
    fullName: string;
    email: string;
    phone: string;
    location: string;
    summary?: string;
  };
  experience: Array<{
    id: string;
    company: string;
    position: string;
    startDate: string;
    endDate?: string;
    current: boolean;
    description?: string;
  }>;
  education: Array<{
    id: string;
    school: string;
    degree: string;
    field: string;
    startDate: string;
    endDate?: string;
    current: boolean;
  }>;
  skills: Array<{
    id: string;
    name: string;
  }>;
  languages: Array<{
    id: string;
    language: string;
    proficiency: string;
  }>;
  certificates: Array<{
    id: string;
    name: string;
    issuer: string;
    date: string;
  }>;
}
```

### Mevcut Özellikler (CvOlusturucu'dan Alınacak)
- ✅ ATS uyumlu CV şablonları → **V4'e taşınacak**
- ✅ Çoklu dil desteği (TR/DE) → **Mevcut LanguageContext ile entegre edilecek**
- ✅ Canlı önizleme → **V4'e taşınacak**
- ✅ Form validasyonu (Zod) → **V4'e taşınacak**
- ✅ Responsive tasarım → **V4'e taşınacak**

### Eklenecek Özellikler (Worklines Entegrasyonu)
- ✅ Kullanıcı kimlik doğrulama → **Mevcut JWT sistemi kullanılacak**
- ✅ CV verilerinin veritabanında saklanması → **SQL Server'a kaydedilecek**
- ✅ PDF export fonksiyonu → **ASP.NET Core'da implement edilecek**
- ✅ ATS uyumluluk skoru hesaplama → **Backend'de hesaplanacak**
- ✅ Ödeme entegrasyonu → **Mevcut Payment sistemi kullanılacak**

---

## Entegrasyon Gereksinimleri

### 1. CV Belge Tespiti
**Gereksinim**: Client'ın yüklediği belgelerde CV olup olmadığını tespit etme

**Yaklaşım**:
- Belge adı analizi (CV, Resume, Lebenslauf, vb. içeren dosyalar)
- MIME type kontrolü (PDF, DOC, DOCX)
- İçerik analizi (opsiyonel - OCR ile metin çıkarma)
- DocumentType tablosuna "CV" tipi ekleme

**Uygulama**:
```csharp
// Backend: DocumentService.cs
public async Task<DocumentAnalysisResult> AnalyzeDocumentAsync(int documentId)
{
    var document = await _context.Documents.FindAsync(documentId);
    
    // CV tespiti
    var isCV = DetectCV(document);
    
    if (isCV)
    {
        // ATS uyumluluk analizi
        var atsScore = await CalculateATSScoreAsync(document);
        return new DocumentAnalysisResult
        {
            IsCV = true,
            ATSScore = atsScore,
            Recommendations = GenerateRecommendations(atsScore)
        };
    }
    
    return new DocumentAnalysisResult { IsCV = false };
}
```

### 2. ATS Uyumluluk Skoru
**Gereksinim**: CV'nin ATS sistemlerine uyumluluk yüzdesini hesaplama

**Kriterler**:
- ✅ Standart format (PDF, DOCX)
- ✅ Yapılandırılmış içerik (başlıklar, bölümler)
- ✅ Anahtar kelimeler (keywords)
- ✅ Tarih formatları
- ✅ İletişim bilgileri
- ❌ Görsel öğeler (resim, grafik)
- ❌ Tablolar
- ❌ Özel fontlar

**Hesaplama Algoritması**:
```csharp
public async Task<int> CalculateATSScoreAsync(Document document)
{
    int score = 0;
    
    // Format kontrolü (20 puan)
    if (document.FileExtension == ".pdf" || document.FileExtension == ".docx")
        score += 20;
    
    // İçerik analizi (OCR veya dosya parse)
    var content = await ExtractTextAsync(document);
    
    // Yapı kontrolü (30 puan)
    if (HasStructuredSections(content)) score += 30;
    
    // Anahtar kelimeler (30 puan)
    if (HasKeywords(content)) score += 30;
    
    // İletişim bilgileri (20 puan)
    if (HasContactInfo(content)) score += 20;
    
    return Math.Min(score, 100);
}
```

### 3. "CV Oluştur" Butonu
**Gereksinim**: Belge listesinde CV belgelerinin yanında "CV Oluştur" butonu

**UI Gereksinimleri**:
- Buton: "CV Oluştur - 20€" veya "Create CV - 20€"
- ATS skoru gösterimi: Progress bar veya badge
- Tooltip: "ATS uyumlu CV oluşturun ve başvurularınızda öne çıkın"

**Frontend Implementasyonu**:
```tsx
// V4/src/pages/Client/Documents.tsx
{isCV && (
  <div className="flex items-center gap-2">
    <div className="flex-1">
      <div className="text-sm text-gray-600">ATS Uyumluluk</div>
      <Progress value={atsScore} className="h-2" />
      <div className="text-xs text-gray-500">{atsScore}%</div>
    </div>
    <Button 
      onClick={() => handleCreateCV(document.id)}
      className="bg-blue-600 hover:bg-blue-700"
    >
      CV Oluştur - 20€
    </Button>
  </div>
)}
```

### 4. Sipariş ve Ödeme Akışı
**Gereksinim**: CV oluşturma için ödeme akışı

**Mevcut Sistem Analizi**:
✅ **Payment** tablosu zaten sipariş görevi görüyor
✅ **PaymentItem** tablosu var - her payment'ın item'ları saklanıyor
✅ **RelatedEntityType** ve **RelatedEntityId** ile entity ilişkilendirmesi mevcut
✅ **PaymentType** enum'ı var (Appointment, Application, Service)

**Çözüm**: Ekstra Order tablosuna GEREK YOK! Mevcut Payment sistemi kullanılacak.

**Akış**:
1. Kullanıcı "CV Oluştur" butonuna tıklar
2. Ödeme sayfasına yönlendirilir (direkt Payment oluşturulur)
3. iyzico ödeme formu gösterilir
4. Ödeme başarılıysa CV Builder'a yönlendirilir

**Payment Yapılandırması**:
```csharp
// Payment entity zaten mevcut, sadece şunlar eklenecek:
- PaymentType.CVBuilder (enum'a eklenecek)
- RelatedEntityType = "CVBuilder" veya "Document"
- RelatedEntityId = documentId (referans CV belgesi)
- PaymentItem: Name = "CV Builder - ATS Uyumlu CV Oluşturma"
- CVBuilderSessionId: Payment tablosuna yeni kolon (opsiyonel)
```

**PaymentItem Yapılandırması**:
```csharp
// PaymentItem zaten mevcut
payment.Items.Add(new PaymentItem
{
    Name = "CV Builder - ATS Uyumlu CV Oluşturma",
    Description = "Profesyonel ATS uyumlu CV oluşturma hizmeti",
    Quantity = 1,
    UnitPrice = 20.00m,
    TotalPrice = 20.00m,
    RelatedEntityType = "CVBuilder", // veya "Document"
    RelatedEntityId = documentId
});
```

### 5. CV Builder Entegrasyonu
**Gereksinim**: Ödeme sonrası CV Builder modülüne yönlendirme

**Yaklaşım**:
- CV Builder'ı Worklines frontend'ine embed etme
- Veya ayrı bir subdomain'de çalıştırma
- Session yönetimi ile kullanıcı kimlik doğrulama
- CV verilerini Worklines veritabanında saklama

---

## Teknik Mimari

### Mimari Yaklaşım: Monolith Extension (Kesin)

**Karar**: CvOlusturucu projesi **tamamen Worklines sistemine entegre edilecek**.

**Uygulama Stratejisi**:
1. ✅ **Frontend Entegrasyonu**:
   - CvOlusturucu/client/src/components → V4/src/components/cv-builder/
   - CvOlusturucu/client/src/pages/Builder.tsx → V4/src/pages/Client/CVBuilder.tsx
   - Mevcut V4 routing yapısına ekleme
   - Mevcut V4 UI component'leri (shadcn/ui) ile uyumluluk

2. ✅ **Backend Entegrasyonu**:
   - Express.js backend'i **tamamen kaldırılacak**
   - Tüm backend fonksiyonlar **ASP.NET Core'a port edilecek**
   - Yeni modül: `wixi.CVBuilder`
   - SQL Server veritabanı kullanılacak (PostgreSQL değil)

3. ✅ **Veritabanı Entegrasyonu**:
   - Drizzle ORM **kaldırılacak**
   - Entity Framework Core kullanılacak
   - Mevcut SQL Server veritabanına tablolar eklenecek

4. ✅ **Dependency Yönetimi**:
   - CvOlusturucu package.json'daki dependency'ler V4 package.json'a eklenecek
   - Gereksiz dependency'ler temizlenecek
   - Mevcut V4 dependency'leri ile çakışma kontrolü

### Önerilen Mimari: Monolith Extension

```
┌─────────────────────────────────────────────────────────┐
│                    Worklines Frontend (V4)              │
│  ┌──────────────────┐  ┌──────────────────────────┐  │
│  │  Documents Page  │  │   CV Builder (Embedded)    │  │
│  │  - CV Detection  │  │   - Form Components      │  │
│  │  - ATS Score     │  │   - Preview Component     │  │
│  │  - Create CV Btn │  │   - PDF Export            │  │
│  └──────────────────┘  └──────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│              Worklines Backend (ASP.NET Core)          │
│  ┌──────────────────┐  ┌──────────────────────────┐  │
│  │ DocumentService  │  │    CVBuilderService       │  │
│  │ - Upload         │  │    - CreateCV            │  │
│  │ - Analyze        │  │    - SaveCVData          │  │
│  │ - DetectCV       │  │    - ExportPDF           │  │
│  │ - CalculateATS   │  │    - GetATSScore         │  │
│  └──────────────────┘  └──────────────────────────┘  │
│  ┌──────────────────┐                                  │
│  │ PaymentService   │  (MEVCUT - Güncellenecek)       │
│  │ - CreatePayment  │  - PaymentType.CVBuilder         │
│  │ - ProcessPayment │  - PaymentItem (CV Builder)     │
│  │ - CreateSession  │  - CVBuilderSessionId          │
│  └──────────────────┘                                  │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    SQL Server Database                  │
│  - Documents                                            │
│  - Payments (MEVCUT - CVBuilderSessionId eklenecek)  │
│  - PaymentItems (MEVCUT - CV Builder item'ları)        │
│  - CVData (NEW - Payment ile ilişkili)                 │
│  - DocumentAnalysis (NEW)                               │
└─────────────────────────────────────────────────────────┘
```

---

## Veri Akışı

### 1. CV Tespiti ve ATS Skoru Hesaplama

```
Client Uploads Document
    │
    ▼
DocumentService.UploadDocumentAsync()
    │
    ▼
DocumentService.AnalyzeDocumentAsync()
    │
    ├─► DetectCV() → IsCV: true/false
    │
    └─► If IsCV:
            │
            ├─► ExtractText() → Document Content
            │
            ├─► CalculateATSScore() → Score (0-100)
            │
            └─► SaveAnalysisResult() → DocumentAnalysis
```

### 2. CV Oluşturma ve Ödeme Akışı

```
User Clicks "CV Oluştur" Button
    │
    ▼
Frontend: handleCreateCV(documentId)
    │
    ▼
Frontend: Redirect to Payment Page
    │   - documentId: CV belgesi ID
    │   - amount: 20.00 EUR
    │
    ▼
POST /api/v1.0/payments
    │
    ├─► PaymentService.CreatePaymentAsync()
    │   - PaymentType: "CVBuilder" (enum)
    │   - RelatedEntityType: "CVBuilder" veya "Document"
    │   - RelatedEntityId: documentId
    │   - Amount: 20.00 EUR
    │   - PaymentItem:
    │     - Name: "CV Builder - ATS Uyumlu CV Oluşturma"
    │     - RelatedEntityType: "CVBuilder"
    │     - RelatedEntityId: documentId
    │
    ▼
iyzico Payment Form
    │
    ├─► User Enters Card Details
    │
    ├─► iyzico Processes Payment
    │
    └─► Payment Callback
            │
            ├─► Payment Status: Completed
            │
            ├─► CVBuilderService.CreateSession()
            │   - PaymentId: payment.Id
            │   - DocumentId: documentId
            │   - CVBuilderSessionId: {guid}
            │   - Save to CVData table
            │
            └─► Redirect to CV Builder
                    │
                    └─► /cv-builder?session={sessionId}&paymentId={paymentId}
```

### 3. CV Builder Kullanımı

```
User in CV Builder
    │
    ├─► Fill Forms (PersonalInfo, Experience, etc.)
    │
    ├─► Live Preview Updates
    │
    ├─► Save Draft → POST /api/v1.0/cv-builder/save
    │   - Save to CVData table
    │
    ├─► Export PDF → POST /api/v1.0/cv-builder/export
    │   - Generate PDF
    │   - Save to Documents table
    │   - Link to Order
    │
    └─► Complete Order
            │
            └─► Order.Status = Completed
```

---

## API Entegrasyonları

### 1. Mevcut Sistem Güncellemeleri

#### PaymentType Enum Güncellemesi
```csharp
// wixi.Payments/Entities/Payment.cs
public enum PaymentType
{
    Appointment = 1,   // Randevu ödemesi
    Application = 2,   // Başvuru ödemesi
    Service = 3,       // Hizmet ödemesi
    CVBuilder = 4      // CV Builder ödemesi (YENİ)
}
```

#### Payment Entity Güncellemesi (Opsiyonel)
```csharp
// wixi.Payments/Entities/Payment.cs
// CV Builder session ID için yeni kolon (opsiyonel)
public Guid? CVBuilderSessionId { get; set; }
```

### 2. Yeni Backend Endpoints

#### Document Analysis
```csharp
// GET /api/v1.0/documents/{documentId}/analysis
[HttpGet("{documentId}/analysis")]
public async Task<IActionResult> AnalyzeDocument(int documentId)
{
    var analysis = await _documentService.AnalyzeDocumentAsync(documentId);
    return Ok(new { success = true, data = analysis });
}
```

#### Payment Management (Mevcut - Güncellenecek)
```csharp
// POST /api/v1.0/payments (MEVCUT - Sadece PaymentType.CVBuilder eklenecek)
[HttpPost]
public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
{
    // dto.PaymentType = "CVBuilder"
    // dto.RelatedEntityType = "CVBuilder" veya "Document"
    // dto.RelatedEntityId = documentId
    var payment = await _paymentService.CreatePaymentAsync(dto);
    return Ok(new { success = true, data = payment });
}

// GET /api/v1.0/payments/{paymentId} (MEVCUT)
[HttpGet("{paymentId}")]
public async Task<IActionResult> GetPayment(long paymentId)
{
    var payment = await _paymentService.GetPaymentAsync(paymentId);
    return Ok(new { success = true, data = payment });
}

// GET /api/v1.0/payments/cv-builder/{documentId} (YENİ)
[HttpGet("cv-builder/{documentId}")]
public async Task<IActionResult> GetCVBuilderPayments(int documentId)
{
    var payments = await _context.Payments
        .Include(p => p.Items)
        .Where(p => p.Type == PaymentType.CVBuilder && 
                    p.Items.Any(i => i.RelatedEntityId == documentId))
        .ToListAsync();
    return Ok(new { success = true, data = payments });
}
```

#### CV Builder
```csharp
// POST /api/v1.0/cv-builder/save
[HttpPost("save")]
public async Task<IActionResult> SaveCVData([FromBody] SaveCVDataDto dto)
{
    var cvData = await _cvBuilderService.SaveCVDataAsync(dto);
    return Ok(new { success = true, data = cvData });
}

// GET /api/v1.0/cv-builder/{sessionId}
[HttpGet("{sessionId}")]
public async Task<IActionResult> GetCVData(string sessionId)
{
    var cvData = await _cvBuilderService.GetCVDataAsync(sessionId);
    return Ok(new { success = true, data = cvData });
}

// POST /api/v1.0/cv-builder/export
[HttpPost("export")]
public async Task<IActionResult> ExportCV([FromBody] ExportCVDto dto)
{
    var pdfDocument = await _cvBuilderService.ExportToPDFAsync(dto);
    return Ok(new { success = true, data = pdfDocument });
}
```

### 3. Frontend API Services

```typescript
// V4/src/ApiServices/services/CVBuilderService.ts
export const CVBuilderService = {
  async analyzeDocument(documentId: number): Promise<DocumentAnalysis> {
    const res = await fetch(`${BASE_URL}/api/v1.0/documents/${documentId}/analysis`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return res.json();
  },

  async createPayment(documentId: number): Promise<Payment> {
    // Mevcut PaymentService kullanılacak, sadece parametreler değişecek
    const res = await fetch(`${BASE_URL}/api/v1.0/payments`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify({
        amount: 20.00,
        currency: 'EUR',
        paymentType: 'CVBuilder',
        relatedEntityType: 'CVBuilder',
        relatedEntityId: documentId,
        description: 'CV Builder - ATS Uyumlu CV Oluşturma',
        // ... diğer payment bilgileri
      }),
    });
    return res.json();
  },

  async saveCVData(sessionId: string, cvData: CVData): Promise<void> {
    const res = await fetch(`${BASE_URL}/api/v1.0/cv-builder/save`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify({ sessionId, cvData }),
    });
  },

  async exportCV(sessionId: string): Promise<Document> {
    const res = await fetch(`${BASE_URL}/api/v1.0/cv-builder/export`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify({ sessionId }),
    });
    return res.json();
  },
};
```

---

## Ödeme Akışı

### Ödeme Senaryosu

1. **Ödeme Oluşturma (Direkt)**
   ```typescript
   // Sipariş tablosu yok, direkt payment oluşturuluyor
   const payment = await PaymentService.createPayment({
     amount: 20.00,
     currency: 'EUR',
     paymentType: 'CVBuilder',
     relatedEntityType: 'CVBuilder',
     relatedEntityId: documentId,
     description: 'CV Builder - ATS Uyumlu CV Oluşturma',
     // ... kart bilgileri
   });
   // Response: { paymentNumber: "PAY-20241118-001", paymentId: 123, status: "Pending" }
   ```

2. **Ödeme Sayfası**
   ```tsx
   // V4/src/pages/Client/CVBuilderPayment.tsx
   // Mevcut PaymentForm component'i kullanılabilir
   <PaymentForm
     amount={20.00}
     currency="EUR"
     paymentType="CVBuilder"
     relatedEntityType="CVBuilder"
     relatedEntityId={documentId}
     description="CV Builder - ATS Uyumlu CV Oluşturma"
     onSuccess={(payment) => {
       // Redirect to CV Builder
       navigate(`/cv-builder?session=${payment.cvBuilderSessionId}&paymentId=${payment.id}`);
     }}
   />
   ```

3. **iyzico Ödeme**
   - Mevcut `PaymentService` kullanılır
   - `PaymentType: CVBuilder`
   - `RelatedEntityType: "CVBuilder"`
   - `RelatedEntityId: documentId`
   - `PaymentItem` otomatik oluşturulur

4. **Ödeme Callback**
   ```csharp
   // PaymentService.cs - CreatePaymentAsync()
   if (payment.Status == PaymentStatus.Completed)
   {
       // CV Builder için session oluştur
       if (dto.PaymentType == PaymentType.CVBuilder)
       {
           var sessionId = Guid.NewGuid();
           payment.CVBuilderSessionId = sessionId; // Yeni kolon
           
           // CVData kaydı oluştur (opsiyonel - ilk kayıt için)
           var cvData = new CVData
           {
               PaymentId = payment.Id,
               ClientId = clientId, // Payment'dan alınacak
               DocumentId = dto.RelatedEntityId, // documentId
               SessionId = sessionId,
               CreatedAt = DateTime.UtcNow,
               UpdatedAt = DateTime.UtcNow
           };
           _context.CVData.Add(cvData);
           await _context.SaveChangesAsync();
       }
   }
   ```

5. **CV Builder'a Yönlendirme**
   ```typescript
   // Frontend: Payment success callback
   if (payment.status === 'Completed' && payment.paymentType === 'CVBuilder') {
     navigate(`/cv-builder?session=${payment.cvBuilderSessionId}&paymentId=${payment.id}`);
   }
   ```

---

## ATS Uyumluluk Analizi

### ATS Skoru Hesaplama Kriterleri

| Kriter | Puan | Açıklama |
|--------|------|----------|
| **Format Uyumluluğu** | 20 | PDF veya DOCX formatı |
| **Yapılandırılmış İçerik** | 30 | Başlıklar, bölümler, listeler |
| **Anahtar Kelimeler** | 30 | İş tanımına uygun keywords |
| **İletişim Bilgileri** | 20 | Email, telefon, adres |
| **TOPLAM** | **100** | |

### ATS Skoru Kategorileri

- **90-100**: Mükemmel - ATS sistemleri tarafından mükemmel şekilde okunur
- **70-89**: İyi - ATS sistemleri tarafından iyi okunur, küçük iyileştirmeler önerilir
- **50-69**: Orta - ATS sistemleri tarafından okunabilir, önemli iyileştirmeler gerekli
- **0-49**: Zayıf - ATS sistemleri tarafından zor okunur, CV oluşturucu kullanılmalı

### Öneriler Sistemi

```csharp
public List<string> GenerateRecommendations(int atsScore)
{
    var recommendations = new List<string>();
    
    if (atsScore < 50)
    {
        recommendations.Add("CV'niz ATS sistemleri için optimize edilmemiş. CV Builder kullanarak profesyonel bir CV oluşturun.");
    }
    else if (atsScore < 70)
    {
        recommendations.Add("CV'nizde bazı iyileştirmeler yapılabilir. ATS uyumlu format kullanın.");
        recommendations.Add("Görsel öğeler ve tablolar ATS sistemleri tarafından okunamaz.");
    }
    else if (atsScore < 90)
    {
        recommendations.Add("CV'niz iyi durumda, ancak daha fazla anahtar kelime ekleyerek skorunuzu artırabilirsiniz.");
    }
    
    return recommendations;
}
```

---

## Uygulama Adımları

### Faz 1: Backend Hazırlık (1 hafta)

#### 1.1. Veritabanı Şeması
```sql
-- Payment Tablosuna Yeni Kolon (Opsiyonel - Session ID için)
ALTER TABLE wixi_Payments
ADD CVBuilderSessionId UNIQUEIDENTIFIER NULL;

-- CVData Tablosu (Payment ile ilişkili)
CREATE TABLE wixi_CVData (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentId BIGINT NOT NULL, -- Payment ile ilişki
    ClientId INT NOT NULL,
    DocumentId INT NULL, -- Referans CV belgesi (opsiyonel)
    SessionId UNIQUEIDENTIFIER UNIQUE NOT NULL,
    PersonalInfo NVARCHAR(MAX), -- JSON
    Experience NVARCHAR(MAX), -- JSON
    Education NVARCHAR(MAX), -- JSON
    Skills NVARCHAR(MAX), -- JSON
    Languages NVARCHAR(MAX), -- JSON
    Certificates NVARCHAR(MAX), -- JSON
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    FOREIGN KEY (PaymentId) REFERENCES wixi_Payments(Id),
    FOREIGN KEY (ClientId) REFERENCES wixi_Clients(Id),
    FOREIGN KEY (DocumentId) REFERENCES wixi_Documents(Id)
);

-- DocumentAnalysis Tablosu
CREATE TABLE wixi_DocumentAnalysis (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DocumentId INT NOT NULL,
    IsCV BIT NOT NULL,
    ATSScore INT NULL,
    Recommendations NVARCHAR(MAX), -- JSON
    AnalyzedAt DATETIME2 NOT NULL,
    FOREIGN KEY (DocumentId) REFERENCES wixi_Documents(Id)
);
```

#### 1.2. Entity Sınıfları
```csharp
// wixi.Payments/Entities/Payment.cs - Güncelleme
public enum PaymentType
{
    Appointment = 1,
    Application = 2,
    Service = 3,
    CVBuilder = 4  // YENİ
}

// Payment entity'ye yeni kolon (opsiyonel)
public Guid? CVBuilderSessionId { get; set; }

// wixi.CVBuilder/Entities/CVData.cs (YENİ)
public class CVData
{
    public int Id { get; set; }
    public long PaymentId { get; set; } // Payment ile ilişki
    public int ClientId { get; set; }
    public int? DocumentId { get; set; } // Referans CV belgesi
    public Guid SessionId { get; set; }
    public string PersonalInfo { get; set; } // JSON
    public string Experience { get; set; } // JSON
    public string Education { get; set; } // JSON
    public string Skills { get; set; } // JSON
    public string Languages { get; set; } // JSON
    public string Certificates { get; set; } // JSON
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public virtual Payment Payment { get; set; }
    public virtual Client Client { get; set; }
    public virtual Document? Document { get; set; }
}
```

#### 1.3. Service Sınıfları
- ✅ `PaymentService`: Mevcut - sadece CVBuilder desteği eklenecek
- `CVBuilderService`: CV oluşturma ve yönetimi (YENİ)
- `DocumentAnalysisService`: CV tespiti ve ATS skoru hesaplama (YENİ)

#### 1.4. Controller'lar
- ✅ `PaymentsController`: Mevcut - CVBuilder endpoint'leri eklenecek
- `CVBuilderController`: CV Builder API endpoints (YENİ)
- `DocumentAnalysisController`: Belge analiz endpoints (YENİ)

### Faz 2: CV Tespiti ve ATS Skoru (1 hafta)

#### 2.1. CV Tespiti
- Belge adı analizi
- MIME type kontrolü
- İçerik analizi (OCR - opsiyonel)

#### 2.2. ATS Skoru Hesaplama
- Metin çıkarma (PDF/DOCX)
- Yapı analizi
- Keyword analizi
- İletişim bilgisi tespiti

#### 2.3. Frontend Entegrasyonu
- Belge listesinde ATS skoru gösterimi
- "CV Oluştur" butonu
- Tooltip ve açıklamalar

### Faz 3: Ödeme Entegrasyonu (3-5 gün)

#### 3.1. PaymentType Enum Güncellemesi
- PaymentType.CVBuilder ekleme
- Migration oluşturma

#### 3.2. PaymentService Güncellemesi
- CVBuilder payment type desteği
- PaymentItem'da CV Builder bilgileri
- Ödeme başarılı olduğunda session oluşturma

#### 3.3. Frontend Ödeme Formu
- Mevcut PaymentForm component'i kullanımı
- CVBuilder parametreleri ile çağırma

#### 3.4. Ödeme Sonrası Yönlendirme
- Session oluşturma (PaymentService içinde)
- CV Builder'a yönlendirme
- URL parametreleri (sessionId, paymentId)

### Faz 4: CV Builder Entegrasyonu (2-3 hafta)

#### 4.1. Frontend Component Taşıma
- ✅ CvOlusturucu/client/src/components → V4/src/components/cv-builder/
  - PersonalInfoForm.tsx
  - ExperienceForm.tsx
  - EducationForm.tsx
  - SkillsForm.tsx
  - CVPreview.tsx
- ✅ CvOlusturucu/client/src/pages/Builder.tsx → V4/src/pages/Client/CVBuilder.tsx
- ✅ Dependency kontrolü ve ekleme (package.json)
- ✅ Import path'lerini güncelleme
- ✅ Mevcut V4 UI component'leri ile uyumluluk kontrolü

#### 4.2. Backend Modül Oluşturma
- ✅ Yeni modül: `wixi.CVBuilder` projesi oluşturma
- ✅ Entity sınıfları (CVData.cs)
- ✅ DTO sınıfları (CVDataDto, SaveCVDataDto)
- ✅ Service interface ve implementation (ICVBuilderService, CVBuilderService)
- ✅ Controller (CVBuilderController)
- ✅ DbContext'e CVData DbSet ekleme

#### 4.3. Backend API Implementation
- ✅ CV verilerini kaydetme endpoint (POST /api/v1.0/cv-builder/save)
- ✅ CV verilerini yükleme endpoint (GET /api/v1.0/cv-builder/{sessionId})
- ✅ CV verilerini güncelleme endpoint (PUT /api/v1.0/cv-builder/{sessionId})
- ✅ PDF export endpoint (POST /api/v1.0/cv-builder/export)

#### 4.4. PDF Export Implementation
- ✅ PDF generation library seçimi (QuestPDF önerilir - ATS uyumlu)
- ✅ CV şablonu oluşturma (ATS uyumlu format)
- ✅ PDF indirme endpoint
- ✅ PDF'i Documents tablosuna kaydetme

#### 4.5. Frontend-Backend Entegrasyonu
- ✅ CVBuilderService.ts oluşturma (API calls)
- ✅ Routing yapılandırması (App.tsx)
- ✅ Authentication entegrasyonu (JWT token)
- ✅ Error handling ve toast notifications

### Faz 5: Test ve Optimizasyon (1 hafta)

#### 5.1. Unit Testler
- Service testleri
- API endpoint testleri

#### 5.2. Integration Testler
- End-to-end akış testleri
- Ödeme akışı testleri

#### 5.3. Performance Optimizasyonu
- ATS skoru hesaplama optimizasyonu
- PDF generation optimizasyonu
- Database query optimizasyonu

---

## Riskler ve Çözümler

### Risk 1: CV Tespiti Doğruluğu
**Risk**: Yanlış pozitif/negatif CV tespiti

**Çözüm**:
- Çoklu kriter kullanımı (isim + içerik + format)
- Kullanıcıya manuel onay seçeneği
- Machine learning modeli (gelecekte)

### Risk 2: ATS Skoru Hesaplama Performansı
**Risk**: Büyük dosyalarda yavaş analiz

**Çözüm**:
- Async processing
- Background job (Hangfire, Quartz.NET)
- Caching mekanizması

### Risk 3: PDF Export Kalitesi
**Risk**: ATS uyumlu PDF oluşturma zorluğu

**Çözüm**:
- Test edilmiş PDF library kullanımı
- ATS test araçları ile doğrulama
- Kullanıcı geri bildirimi toplama

### Risk 4: Ödeme Başarısızlığı
**Risk**: Ödeme başarılı ama order complete edilmedi

**Çözüm**:
- Webhook ile güvenilir callback
- Retry mekanizması
- Admin panel'de manuel müdahale

### Risk 5: Session Yönetimi
**Risk**: Session kaybı veya güvenlik açığı

**Çözüm**:
- JWT token kullanımı
- Session timeout yönetimi
- Güvenli session ID generation

---

## Sonuç ve Öneriler

### Öncelikli Adımlar
1. ✅ Veritabanı şeması oluşturma
2. ✅ CV tespiti ve ATS skoru hesaplama
3. ✅ "CV Oluştur" butonu ve UI
4. ✅ Sipariş ve ödeme akışı
5. ✅ CV Builder entegrasyonu

### Gelecek Geliştirmeler
- Machine learning ile daha doğru CV tespiti
- Çoklu CV şablonları
- CV önizleme ve düzenleme
- CV paylaşım özellikleri
- Analytics ve raporlama

### Teknik Notlar
- CV Builder frontend'i React component olarak V4'e entegre edilebilir
- Backend fonksiyonları ASP.NET Core'a port edilmeli
- Express.js backend'i kaldırılabilir (opsiyonel)
- PDF export için QuestPDF veya iTextSharp kullanılabilir

---

**Doküman Versiyonu**: 1.0  
**Son Güncelleme**: 2024-11-18  
**Hazırlayan**: AI Assistant  
**Onay Durumu**: Beklemede

