# 💳 Ödeme Sistemi Modülü - iyzico Entegrasyonu Analizi

## 🎯 Genel Bakış

Randevu sistemi gibi, ödeme sistemini de modüler ve taşınabilir bir yapıda kuruyoruz. iyzico entegrasyonu ile ödeme altyapısı oluşturulacak.

---

## 📊 Mevcut Durum Analizi

### Frontend
- ❌ Ödeme sayfası YOK
- ❌ Ödeme formu YOK
- ❌ Ödeme geçmişi YOK
- ✅ Randevu sistemi var (ödeme entegrasyonu için hazır)

### Backend
- ❌ Payment entity YOK
- ❌ Payment service YOK
- ❌ iyzico entegrasyonu YOK
- ❌ Webhook handler YOK
- ✅ Randevu sistemi var (ödeme ile entegre edilebilir)

### Referanslar
- ✅ "Denklik Harc Ödemesi" adımları var (ApplicationSubStepTemplate)
- ✅ Randevu fiyatlandırması var (€50)

---

## 🏗️ Modüler Yapı Tasarımı

### Backend Modül Yapısı

```
wixi.backend/
├── wixi.WebAPI/                    # API Gateway Layer
│   ├── Controllers/
│   │   ├── PaymentsController.cs           # ✅ Public API
│   │   ├── PaymentWebhooksController.cs    # ✅ iyzico webhooks
│   │   └── AdminPaymentsController.cs      # ✅ Admin API
│   └── Program.cs
│
├── wixi.Payments/                  # Payment Module (Class Library)
│   ├── Entities/
│   │   ├── Payment.cs
│   │   ├── PaymentItem.cs
│   │   ├── PaymentTransaction.cs
│   │   └── PaymentRefund.cs
│   ├── DTOs/
│   │   ├── PaymentCreateDto.cs
│   │   ├── PaymentResponseDto.cs
│   │   ├── PaymentCallbackDto.cs
│   │   └── PaymentRefundDto.cs
│   ├── Interfaces/
│   │   ├── IPaymentService.cs
│   │   └── IPaymentProvider.cs
│   ├── Providers/
│   │   ├── IyzicoPaymentProvider.cs       # iyzico adapter
│   │   └── PaymentProviderFactory.cs
│   ├── Services/
│   │   ├── PaymentService.cs
│   │   ├── PaymentNotificationService.cs
│   │   └── PaymentWebhookService.cs
│   ├── Validators/
│   │   └── PaymentValidator.cs
│   └── wixi.Payments.csproj
│
├── wixi.Business/                  # Mevcut business logic
├── wixi.DataAccess/                 # Mevcut data access
└── wixi.Entities/                   # Mevcut entities
    └── Concrete/
        └── Payment/                 # ✅ Yeni payment entities
```

---

## 💳 Entity Tasarımı

### Payment.cs
```csharp
namespace wixi.Entities.Concrete.Payment
{
    public class Payment
    {
        public int Id { get; set; }
        
        // Payment Info
        public string PaymentNumber { get; set; } = string.Empty; // "PAY-2024-001234"
        public string PaymentProvider { get; set; } = "Iyzico"; // "Iyzico", "Stripe", etc.
        public string ProviderPaymentId { get; set; } = string.Empty; // iyzico payment ID
        public string ConversationId { get; set; } = string.Empty; // iyzico conversation ID
        
        // Customer Info
        public int? ClientId { get; set; }
        public virtual Client.Client? Client { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerIdentityNumber { get; set; } // TC Kimlik No
        
        // Amount
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public string Currency { get; set; } = "TRY";
        public decimal? ExchangeRate { get; set; } // EUR/TRY dönüşümü için
        
        // Payment Details
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentType Type { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.CreditCard;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        
        // Related Entities
        public int? AppointmentId { get; set; }
        public virtual Appointment.Appointment? Appointment { get; set; }
        public int? ApplicationId { get; set; }
        public virtual Application.Application? Application { get; set; }
        public string? RelatedEntityType { get; set; } // "Appointment", "Application", "Service"
        public int? RelatedEntityId { get; set; }
        
        // iyzico Specific
        public string? IyzicoPaymentId { get; set; }
        public string? IyzicoConversationId { get; set; }
        public string? IyzicoStatus { get; set; }
        public string? IyzicoErrorCode { get; set; }
        public string? IyzicoErrorMessage { get; set; }
        public string? IyzicoRawResponse { get; set; } // JSON response
        
        // Card Info (Masked - PCI-DSS uyumlu)
        public string? CardLastFourDigits { get; set; } // "1234"
        public string? CardHolderName { get; set; }
        public string? CardBrand { get; set; } // "VISA", "MASTERCARD"
        public string? CardType { get; set; } // "CREDIT", "DEBIT"
        public int? InstallmentCount { get; set; } // Taksit sayısı
        
        // Installment Info
        public bool IsInstallment { get; set; } = false;
        public int? InstallmentNumber { get; set; }
        public decimal? InstallmentAmount { get; set; }
        
        // Dates
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        
        // Relations
        public virtual ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
        public virtual ICollection<PaymentItem> Items { get; set; } = new List<PaymentItem>();
        public virtual ICollection<PaymentRefund> Refunds { get; set; } = new List<PaymentRefund>();
        
        // Computed Properties
        public bool IsPaid => Status == PaymentStatus.Completed && PaidAmount >= Amount;
        public bool IsPending => Status == PaymentStatus.Pending;
        public bool IsFailed => Status == PaymentStatus.Failed;
        public bool IsCancelled => Status == PaymentStatus.Cancelled;
        public bool IsRefunded => Status == PaymentStatus.Refunded;
        public decimal RemainingAmount => Amount - PaidAmount;
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    }
    
    public enum PaymentStatus
    {
        Pending = 1,        // Beklemede
        Processing = 2,     // İşleniyor
        Completed = 3,     // Tamamlandı
        Failed = 4,         // Başarısız
        Cancelled = 5,      // İptal edildi
        Refunded = 6,       // İade edildi
        PartiallyRefunded = 7 // Kısmi iade
    }
    
    public enum PaymentType
    {
        Appointment = 1,    // Randevu ödemesi
        Application = 2,    // Başvuru ödemesi
        Service = 3,        // Hizmet ödemesi
        Subscription = 4,   // Abonelik ödemesi
        Other = 99          // Diğer
    }
    
    public enum PaymentMethod
    {
        CreditCard = 1,     // Kredi kartı
        DebitCard = 2,      // Banka kartı
        BankTransfer = 3,   // Havale/EFT
        Cash = 4,           // Nakit
        Other = 99          // Diğer
    }
}
```

### PaymentTransaction.cs
```csharp
public class PaymentTransaction
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public virtual Payment Payment { get; set; } = null!;
    
    // Transaction Info
    public string TransactionId { get; set; } = string.Empty; // iyzico transaction ID
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    
    // iyzico Response
    public string? IyzicoResponse { get; set; } // JSON
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public enum TransactionType
{
    Payment = 1,
    Refund = 2,
    PartialRefund = 3,
    Void = 4
}

public enum TransactionStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3,
    Cancelled = 4
}
```

### PaymentItem.cs
```csharp
public class PaymentItem
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public virtual Payment Payment { get; set; } = null!;
    
    // Item Info
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Related Entity
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
}
```

### PaymentRefund.cs
```csharp
public class PaymentRefund
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public virtual Payment Payment { get; set; } = null!;
    
    // Refund Info
    public string RefundNumber { get; set; } = string.Empty; // "REF-2024-001234"
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? Reason { get; set; }
    public RefundStatus Status { get; set; } = RefundStatus.Pending;
    
    // iyzico
    public string? IyzicoRefundId { get; set; }
    public string? IyzicoResponse { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int? RefundedByUserId { get; set; }
}

public enum RefundStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}
```

---

## 🔌 iyzico Entegrasyonu

### NuGet Paketleri
```xml
<PackageReference Include="Iyzipay" Version="2.1.0" />
```

### iyzico Configuration
```csharp
// appsettings.json
{
  "PaymentSettings": {
    "Iyzico": {
      "ApiKey": "sandbox-xxx", // Environment variable'dan alınacak
      "SecretKey": "sandbox-xxx",
      "BaseUrl": "https://sandbox-api.iyzipay.com", // Production: https://api.iyzipay.com
      "IsTestMode": true
    },
    "Currency": "TRY",
    "DefaultInstallmentCount": 1,
    "MaxInstallmentCount": 12,
    "PaymentTimeoutMinutes": 30
  }
}
```

### IyzicoPaymentProvider.cs
```csharp
public class IyzicoPaymentProvider : IPaymentProvider
{
    private readonly IyzicoOptions _options;
    private readonly ILogger<IyzicoPaymentProvider> _logger;
    
    public IyzicoPaymentProvider(IOptions<PaymentSettings> settings, ILogger<IyzicoPaymentProvider> logger)
    {
        _options = new IyzicoOptions
        {
            ApiKey = settings.Value.Iyzico.ApiKey,
            SecretKey = settings.Value.Iyzico.SecretKey,
            BaseUrl = settings.Value.Iyzico.BaseUrl
        };
        _logger = logger;
    }
    
    public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
    {
        // iyzico payment creation
    }
    
    public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentId)
    {
        // iyzico payment status check
    }
    
    public async Task<RefundResponse> RefundPaymentAsync(RefundRequest request)
    {
        // iyzico refund
    }
    
    public async Task<bool> VerifyWebhookAsync(string signature, string payload)
    {
        // iyzico webhook verification
    }
}
```

---

## 📋 API Endpoints

### Public APIs

#### POST `/api/payments`
```json
{
  "amount": 50.00,
  "currency": "TRY",
  "paymentType": "Appointment",
  "relatedEntityType": "Appointment",
  "relatedEntityId": 123,
  "customerName": "Ahmet Yılmaz",
  "customerEmail": "ahmet@example.com",
  "customerPhone": "+905551234567",
  "customerIdentityNumber": "12345678901",
  "installmentCount": 1,
  "cardHolderName": "AHMET YILMAZ",
  "cardNumber": "5528790000000008",
  "expireMonth": "12",
  "expireYear": "2030",
  "cvc": "123"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentId": 1,
    "paymentNumber": "PAY-2024-001234",
    "status": "Processing",
    "paymentUrl": "https://payment.iyzico.com/checkout/xxx",
    "conversationId": "xxx"
  }
}
```

#### GET `/api/payments/{paymentNumber}`
Payment durumunu sorgula

#### POST `/api/payments/{paymentNumber}/cancel`
Ödemeyi iptal et

### Webhook Endpoints

#### POST `/api/payments/webhooks/iyzico`
iyzico webhook'larını alır

### Admin APIs

#### GET `/api/admin/payments`
Tüm ödemeleri listele (filtreleme, sayfalama)

#### GET `/api/admin/payments/{id}`
Ödeme detayı

#### POST `/api/admin/payments/{id}/refund`
İade işlemi

#### GET `/api/admin/payments/statistics`
Ödeme istatistikleri

---

## 🔐 Güvenlik ve PCI-DSS Uyumluluğu

### Kritik Güvenlik Kuralları

1. **Kart Bilgileri Asla Saklanmaz**
   - Kart numarası, CVV, expire date backend'de saklanmaz
   - Sadece iyzico'ya gönderilir
   - Son 4 hanesi masked olarak saklanabilir

2. **HTTPS Zorunlu**
   - Tüm ödeme endpoint'leri HTTPS üzerinden
   - HTTP istekleri reddedilir

3. **Tokenization**
   - iyzico tokenization kullanılır
   - Tekrar ödemeler için token kullanılır

4. **Webhook Verification**
   - Tüm webhook'lar imza ile doğrulanır
   - Replay attack koruması

5. **Rate Limiting**
   - Ödeme endpoint'leri rate limit ile korunur
   - DDoS koruması

6. **Audit Logging**
   - Tüm ödeme işlemleri loglanır
   - Sensitive data redacted

---

## 🔗 Randevu Sistemi Entegrasyonu

### Senaryo: Randevu Ödemesi

```csharp
// Appointment oluşturulduktan sonra
var appointment = await _appointmentService.CreateAppointmentAsync(dto);

// Ödeme oluştur
var paymentDto = new PaymentCreateDto
{
    Amount = appointment.Price,
    Currency = "TRY",
    PaymentType = PaymentType.Appointment,
    RelatedEntityType = "Appointment",
    RelatedEntityId = appointment.Id,
    CustomerName = appointment.CustomerName,
    CustomerEmail = appointment.CustomerEmail,
    CustomerPhone = appointment.CustomerPhone
};

var payment = await _paymentService.CreatePaymentAsync(paymentDto);

// Randevu'ya payment ID ekle
appointment.PaymentId = payment.Id;
await _appointmentService.UpdateAppointmentAsync(appointment.Id, updateDto);
```

### Ödeme Tamamlandığında

```csharp
// Webhook'tan gelen callback
public async Task<IActionResult> IyzicoWebhook([FromBody] IyzicoWebhookDto webhook)
{
    var payment = await _paymentService.ProcessWebhookAsync(webhook);
    
    if (payment.Status == PaymentStatus.Completed)
    {
        // Randevu'yu onayla
        if (payment.RelatedEntityType == "Appointment")
        {
            await _appointmentService.ConfirmAppointmentAsync(payment.RelatedEntityId.Value);
        }
    }
    
    return Ok();
}
```

---

## 📦 Frontend Modül Yapısı (Opsiyonel)

```
payment-module/
├── src/
│   ├── components/
│   │   ├── PaymentForm/
│   │   │   ├── PaymentForm.tsx
│   │   │   ├── CreditCardInput.tsx
│   │   │   └── InstallmentSelector.tsx
│   │   ├── PaymentStatus/
│   │   │   └── PaymentStatusCard.tsx
│   │   └── PaymentHistory/
│   │       └── PaymentHistoryList.tsx
│   ├── services/
│   │   ├── PaymentService.ts
│   │   └── IyzicoService.ts
│   ├── hooks/
│   │   ├── usePayment.ts
│   │   └── usePaymentForm.ts
│   └── types/
│       └── Payment.ts
```

---

## 🚀 Implementation Plan

### Phase 1: Backend Modülü (1 Hafta)

#### Day 1-2: Entity ve Database
- [ ] Payment entity'leri oluştur
- [ ] Migration oluştur
- [ ] Seed data (test için)

#### Day 3-4: iyzico Entegrasyonu
- [ ] iyzico SDK ekle
- [ ] IyzicoPaymentProvider implement et
- [ ] Payment service oluştur
- [ ] Test ödemeleri yap

#### Day 5: API Endpoints
- [ ] PaymentsController (Public)
- [ ] PaymentWebhooksController
- [ ] AdminPaymentsController
- [ ] Validation ve error handling

#### Day 6-7: Testing ve Documentation
- [ ] Unit tests
- [ ] Integration tests
- [ ] API documentation
- [ ] Security audit

### Phase 2: Frontend Entegrasyonu (3-4 Gün)

#### Day 1-2: Payment Form
- [ ] Payment form component
- [ ] Credit card input
- [ ] Installment selector
- [ ] Form validation

#### Day 3: Payment Status
- [ ] Payment status page
- [ ] Success/failure handling
- [ ] Payment history

#### Day 4: Testing
- [ ] E2E tests
- [ ] Payment flow test

### Phase 3: Randevu Entegrasyonu (2-3 Gün)

- [ ] Appointment'a payment ekle
- [ ] Payment callback'te appointment onayla
- [ ] Payment failed durumunda appointment iptal
- [ ] Test senaryoları

---

## 📊 Database Schema

```sql
-- Payments Table
CREATE TABLE Payments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentNumber NVARCHAR(50) UNIQUE NOT NULL,
    PaymentProvider NVARCHAR(50) NOT NULL DEFAULT 'Iyzico',
    ProviderPaymentId NVARCHAR(100),
    ConversationId NVARCHAR(100),
    
    ClientId INT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerEmail NVARCHAR(100) NOT NULL,
    CustomerPhone NVARCHAR(20) NOT NULL,
    CustomerIdentityNumber NVARCHAR(20),
    
    Amount DECIMAL(18,2) NOT NULL,
    PaidAmount DECIMAL(18,2) DEFAULT 0,
    Currency NVARCHAR(3) DEFAULT 'TRY',
    ExchangeRate DECIMAL(18,4),
    
    Status INT NOT NULL DEFAULT 1,
    Type INT NOT NULL,
    Method INT NOT NULL DEFAULT 1,
    Description NVARCHAR(500),
    Notes NVARCHAR(1000),
    
    AppointmentId INT NULL,
    ApplicationId INT NULL,
    RelatedEntityType NVARCHAR(50),
    RelatedEntityId INT,
    
    -- iyzico Specific
    IyzicoPaymentId NVARCHAR(100),
    IyzicoConversationId NVARCHAR(100),
    IyzicoStatus NVARCHAR(50),
    IyzicoErrorCode NVARCHAR(50),
    IyzicoErrorMessage NVARCHAR(500),
    IyzicoRawResponse NVARCHAR(MAX),
    
    -- Card Info (Masked)
    CardLastFourDigits NVARCHAR(4),
    CardHolderName NVARCHAR(100),
    CardBrand NVARCHAR(50),
    CardType NVARCHAR(50),
    InstallmentCount INT,
    
    IsInstallment BIT DEFAULT 0,
    InstallmentNumber INT,
    InstallmentAmount DECIMAL(18,2),
    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PaidAt DATETIME2,
    CancelledAt DATETIME2,
    RefundedAt DATETIME2,
    ExpiresAt DATETIME2,
    
    FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id),
    FOREIGN KEY (ApplicationId) REFERENCES Applications(Id)
);

-- PaymentTransactions Table
CREATE TABLE PaymentTransactions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentId INT NOT NULL,
    TransactionId NVARCHAR(100) NOT NULL,
    Type INT NOT NULL,
    Status INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) DEFAULT 'TRY',
    IyzicoResponse NVARCHAR(MAX),
    ErrorCode NVARCHAR(50),
    ErrorMessage NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2,
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);

-- PaymentItems Table
CREATE TABLE PaymentItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    Quantity INT DEFAULT 1,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    RelatedEntityType NVARCHAR(50),
    RelatedEntityId INT,
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);

-- PaymentRefunds Table
CREATE TABLE PaymentRefunds (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentId INT NOT NULL,
    RefundNumber NVARCHAR(50) UNIQUE NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) DEFAULT 'TRY',
    Reason NVARCHAR(500),
    Status INT NOT NULL DEFAULT 1,
    IyzicoRefundId NVARCHAR(100),
    IyzicoResponse NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2,
    RefundedByUserId INT,
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);
```

---

## 🔒 Güvenlik Checklist

### PCI-DSS Uyumluluk
- [ ] Kart bilgileri backend'de saklanmaz
- [ ] HTTPS zorunlu
- [ ] Tokenization kullanılır
- [ ] Webhook verification
- [ ] Rate limiting
- [ ] Audit logging
- [ ] Sensitive data redaction

### API Security
- [ ] Authentication (JWT)
- [ ] Authorization (Role-based)
- [ ] Input validation
- [ ] SQL injection protection
- [ ] XSS protection
- [ ] CSRF protection

---

## 📝 Özet

### ✅ Modüler Yapı
- Bağımsız `wixi.Payments` modülü
- Taşınabilir yapı
- iyzico adapter pattern
- Kolay test edilebilir

### ✅ Özellikler
- iyzico entegrasyonu
- Kredi kartı ödemeleri
- Taksit desteği
- İade işlemleri
- Webhook handling
- Randevu entegrasyonu

### ✅ Güvenlik
- PCI-DSS uyumlu
- Kart bilgileri saklanmaz
- HTTPS zorunlu
- Audit logging

### 📦 Sonraki Adımlar
1. Backend modülü oluştur
2. iyzico entegrasyonu
3. API endpoints
4. Frontend entegrasyonu
5. Randevu entegrasyonu

