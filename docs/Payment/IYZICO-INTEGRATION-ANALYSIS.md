# Iyzico Entegrasyon Analiz Dökümanı

## 📋 İçindekiler
1. [Genel Bakış](#genel-bakış)
2. [Iyzico SDK Yapısı](#iyzico-sdk-yapısı)
3. [Payment Flow](#payment-flow)
4. [Database Şeması](#database-şeması)
5. [Entegrasyon Adımları](#entegrasyon-adımları)
6. [Güvenlik Gereksinimleri](#güvenlik-gereksinimleri)
7. [Randevu Sistemi Entegrasyonu](#randevu-sistemi-entegrasyonu)
8. [Webhook Yapısı](#webhook-yapısı)
9. [Implementation Plan](#implementation-plan)

---

## 🎯 Genel Bakış

### Iyzico Nedir?
Iyzico, Türkiye'de faaliyet gösteren bir ödeme altyapı sağlayıcısıdır. Kredi kartı, banka kartı ve diğer ödeme yöntemlerini destekler.

### Proje İhtiyacı
Randevu sistemi için danışmanlık ücreti ödemelerini Iyzico üzerinden almak.

### Desteklenen Özellikler
- ✅ Kredi kartı ödemeleri
- ✅ Taksit seçenekleri
- ✅ İade (Refund) işlemleri
- ✅ İptal (Cancel) işlemleri
- ✅ Webhook desteği
- ✅ Kart kaydetme (Tokenization)
- ✅ 3D Secure desteği

---

## 📦 Iyzico SDK Yapısı

### NuGet Paketi
```xml
<PackageReference Include="Iyzipay" Version="latest" />
```

### Temel Sınıflar

#### 1. Options (Yapılandırma)
```csharp
public class Options
{
    public string ApiKey { get; set; }        // Iyzico API Key
    public string SecretKey { get; set; }      // Iyzico Secret Key
    public string BaseUrl { get; set; }        // Sandbox: "https://sandbox-api.iyzipay.com"
                                                // Production: "https://api.iyzipay.com"
}
```

#### 2. CreatePaymentRequest (Ödeme İsteği)
```csharp
public class CreatePaymentRequest
{
    // Ödeme Bilgileri
    public string Price { get; set; }              // Toplam tutar (string formatında)
    public string PaidPrice { get; set; }          // Ödenecek tutar (komisyon dahil)
    public string Currency { get; set; }           // "TRY", "EUR", "USD"
    public int? Installment { get; set; }          // Taksit sayısı (1 = peşin)
    public string BasketId { get; set; }           // Sepet ID (unique)
    public string PaymentChannel { get; set; }      // "WEB", "MOBILE", "MOBILE_WEB"
    public string PaymentGroup { get; set; }       // "PRODUCT", "SUBSCRIPTION", "LISTING"
    
    // Kart Bilgileri
    public PaymentCard PaymentCard { get; set; }
    
    // Müşteri Bilgileri
    public Buyer Buyer { get; set; }
    
    // Adres Bilgileri
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
    
    // Sepet Ürünleri
    public List<BasketItem> BasketItems { get; set; }
    
    // Callback
    public string CallbackUrl { get; set; }        // Webhook URL
}
```

#### 3. PaymentCard (Kart Bilgileri)
```csharp
public class PaymentCard
{
    public string CardHolderName { get; set; }     // "AHMET YILMAZ"
    public string CardNumber { get; set; }         // "5528790000000008"
    public string ExpireMonth { get; set; }        // "12"
    public string ExpireYear { get; set; }         // "2030"
    public string Cvc { get; set; }                // "123"
    public int? RegisterCard { get; set; }         // 0 = kaydetme, 1 = kaydet
    
    // Token ile ödeme için
    public string CardToken { get; set; }
    public string CardUserKey { get; set; }
}
```

#### 4. Buyer (Müşteri)
```csharp
public class Buyer
{
    public string Id { get; set; }                 // Müşteri ID (unique)
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string GsmNumber { get; set; }          // "+905350000000"
    public string IdentityNumber { get; set; }     // TC Kimlik No (opsiyonel)
    public string Ip { get; set; }                 // Müşteri IP adresi
    public string City { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    public string RegistrationAddress { get; set; }
    public string RegistrationDate { get; set; }   // "2013-04-21 15:12:09"
    public string LastLoginDate { get; set; }
}
```

#### 5. BasketItem (Sepet Ürünü)
```csharp
public class BasketItem
{
    public string Id { get; set; }                 // Ürün ID (unique)
    public string Name { get; set; }               // "Danışmanlık - Okan Bettas"
    public string Price { get; set; }              // Ürün fiyatı (string)
    public string Category1 { get; set; }           // "Consultation"
    public string Category2 { get; set; }           // "Professional Service"
    public string ItemType { get; set; }           // "PHYSICAL" veya "VIRTUAL"
}
```

#### 6. PaymentResource (Ödeme Yanıtı)
```csharp
public class PaymentResource
{
    public string PaymentId { get; set; }           // Iyzico Payment ID
    public string PaymentStatus { get; set; }       // "SUCCESS", "FAILURE"
    public string Status { get; set; }             // "success", "failure"
    public string ErrorMessage { get; set; }
    public string ErrorCode { get; set; }
    
    // Kart Bilgileri (Masked)
    public string CardType { get; set; }           // "CREDIT", "DEBIT"
    public string CardAssociation { get; set; }    // "VISA", "MASTERCARD"
    public string CardFamily { get; set; }
    public string BinNumber { get; set; }          // İlk 6 hane
    public string LastFourDigits { get; set; }     // Son 4 hane
    
    // Token (Kart kaydetme için)
    public string CardToken { get; set; }
    public string CardUserKey { get; set; }
    
    // Komisyon Bilgileri
    public string MerchantCommissionRate { get; set; }
    public string MerchantCommissionRateAmount { get; set; }
    public string IyziCommissionRateAmount { get; set; }
    
    // Transaction Bilgileri
    public List<PaymentItem> PaymentItems { get; set; }
    public string AuthCode { get; set; }
    public string HostReference { get; set; }
}
```

---

## 🔄 Payment Flow

### 1. Ödeme Oluşturma Akışı

```
[Frontend] → [Backend API] → [Iyzico API] → [Backend] → [Database] → [Frontend]
```

#### Adım 1: Frontend'den Ödeme İsteği
```typescript
// Frontend: PaymentForm.tsx
const paymentData = {
  cardNumber: "5528790000000008",
  cardHolder: "AHMET YILMAZ",
  expiryMonth: "12",
  expiryYear: "2030",
  cvv: "123",
  appointmentId: 123,
  amount: 50.00,
  currency: "EUR"
};

await fetch('/api/payments/create', {
  method: 'POST',
  body: JSON.stringify(paymentData)
});
```

#### Adım 2: Backend'de Iyzico İsteği Oluşturma
```csharp
// Backend: PaymentService.cs
var request = new CreatePaymentRequest
{
    Locale = "tr",
    ConversationId = Guid.NewGuid().ToString(),
    Price = "50.00",
    PaidPrice = "51.50",  // Komisyon dahil
    Currency = "EUR",
    Installment = 1,
    BasketId = $"BASKET-{appointmentId}",
    PaymentChannel = "WEB",
    PaymentGroup = "PRODUCT",
    CallbackUrl = "https://api.worklines.de/api/payments/webhooks/iyzico"
};

// Kart Bilgileri
request.PaymentCard = new PaymentCard
{
    CardHolderName = paymentDto.CardHolder,
    CardNumber = paymentDto.CardNumber,
    ExpireMonth = paymentDto.ExpireMonth,
    ExpireYear = paymentDto.ExpireYear,
    Cvc = paymentDto.Cvc,
    RegisterCard = 0  // Kart kaydetme (opsiyonel)
};

// Müşteri Bilgileri
request.Buyer = new Buyer
{
    Id = $"BUYER-{appointment.CustomerEmail}",
    Name = appointment.CustomerName.Split(' ')[0],
    Surname = appointment.CustomerName.Split(' ').Length > 1 
        ? string.Join(" ", appointment.CustomerName.Split(' ').Skip(1)) 
        : "",
    Email = appointment.CustomerEmail,
    GsmNumber = appointment.CustomerPhone,
    Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
    City = "Istanbul",  // Varsayılan veya formdan al
    Country = "Turkey",
    ZipCode = "34000"
};

// Sepet Ürünü
request.BasketItems = new List<BasketItem>
{
    new BasketItem
    {
        Id = $"ITEM-{appointmentId}",
        Name = $"Danışmanlık - {teamMember.Name}",
        Category1 = "Consultation",
        Category2 = "Professional Service",
        ItemType = BasketItemType.VIRTUAL.ToString(),
        Price = "50.00"
    }
};

// Iyzico'ya gönder
var payment = await Payment.Create(request, iyzicoOptions);
```

#### Adım 3: Iyzico Yanıtını İşleme
```csharp
if (payment.Status == "success" && payment.PaymentStatus == "SUCCESS")
{
    // Ödeme başarılı
    var paymentEntity = new Payment
    {
        PaymentNumber = GeneratePaymentNumber(),
        IyzicoPaymentId = payment.PaymentId,
        IyzicoConversationId = payment.ConversationId,
        IyzicoStatus = payment.PaymentStatus,
        Amount = decimal.Parse(payment.Price),
        PaidAmount = decimal.Parse(payment.PaidPrice),
        Currency = payment.Currency,
        Status = PaymentStatus.Completed,
        CardLastFourDigits = payment.LastFourDigits,
        CardBrand = payment.CardAssociation,
        CardType = payment.CardType,
        PaidAt = DateTime.UtcNow,
        IyzicoRawResponse = JsonConvert.SerializeObject(payment)
    };
    
    await _paymentRepository.AddAsync(paymentEntity);
    
    // Randevuyu onayla
    appointment.Status = AppointmentStatus.Confirmed;
    appointment.PaymentId = paymentEntity.Id;
    await _appointmentRepository.UpdateAsync(appointment);
}
else
{
    // Ödeme başarısız
    var paymentEntity = new Payment
    {
        Status = PaymentStatus.Failed,
        IyzicoErrorCode = payment.ErrorCode,
        IyzicoErrorMessage = payment.ErrorMessage
    };
    
    // Randevuyu iptal et
    appointment.Status = AppointmentStatus.Cancelled;
}
```

---

## 📊 Database Şeması

### 1. Payments Tablosu

```sql
CREATE TABLE Payments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    
    -- Ödeme Numarası (Unique)
    PaymentNumber NVARCHAR(50) UNIQUE NOT NULL,
    
    -- Provider Bilgileri
    PaymentProvider NVARCHAR(50) NOT NULL DEFAULT 'Iyzico',
    ProviderPaymentId NVARCHAR(100),              -- Iyzico PaymentId
    ConversationId NVARCHAR(100),                  -- Iyzico ConversationId
    
    -- Müşteri Bilgileri
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerEmail NVARCHAR(100) NOT NULL,
    CustomerPhone NVARCHAR(20) NOT NULL,
    CustomerIdentityNumber NVARCHAR(20),
    
    -- Tutar Bilgileri
    Amount DECIMAL(18,2) NOT NULL,               -- Ödeme tutarı
    PaidAmount DECIMAL(18,2) DEFAULT 0,          -- Ödenen tutar
    Currency NVARCHAR(3) DEFAULT 'EUR',           -- EUR, TRY, USD
    ExchangeRate DECIMAL(18,4),                  -- Döviz kuru (varsa)
    
    -- Durum Bilgileri
    Status INT NOT NULL DEFAULT 1,               -- 1=Pending, 2=Completed, 3=Failed, 4=Cancelled, 5=Refunded
    PaymentType INT NOT NULL,                     -- 1=Appointment, 2=Application, 3=Service
    PaymentMethod INT NOT NULL DEFAULT 1,         -- 1=CreditCard, 2=BankTransfer, 3=Other
    
    -- Açıklama
    Description NVARCHAR(500),
    Notes NVARCHAR(1000),
    
    -- İlişkiler
    AppointmentId INT NULL,
    ApplicationId INT NULL,
    RelatedEntityType NVARCHAR(50),              -- "Appointment", "Application"
    RelatedEntityId INT,
    
    -- Iyzico Özel Alanlar
    IyzicoPaymentId NVARCHAR(100),
    IyzicoConversationId NVARCHAR(100),
    IyzicoStatus NVARCHAR(50),                    -- "SUCCESS", "FAILURE"
    IyzicoErrorCode NVARCHAR(50),
    IyzicoErrorMessage NVARCHAR(500),
    IyzicoRawResponse NVARCHAR(MAX),              -- JSON response
    
    -- Kart Bilgileri (Masked - PCI-DSS uyumlu)
    CardLastFourDigits NVARCHAR(4),               -- "1234"
    CardHolderName NVARCHAR(100),
    CardBrand NVARCHAR(50),                      -- "VISA", "MASTERCARD"
    CardType NVARCHAR(50),                       -- "CREDIT", "DEBIT"
    InstallmentCount INT,                        -- Taksit sayısı
    
    -- Taksit Bilgileri
    IsInstallment BIT DEFAULT 0,
    InstallmentNumber INT,
    InstallmentAmount DECIMAL(18,2),
    
    -- Tarihler
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PaidAt DATETIME2,
    CancelledAt DATETIME2,
    RefundedAt DATETIME2,
    ExpiresAt DATETIME2,
    
    -- Foreign Keys
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id),
    FOREIGN KEY (ApplicationId) REFERENCES Applications(Id)
);

-- Indexes
CREATE INDEX IX_Payments_PaymentNumber ON Payments(PaymentNumber);
CREATE INDEX IX_Payments_IyzicoPaymentId ON Payments(IyzicoPaymentId);
CREATE INDEX IX_Payments_AppointmentId ON Payments(AppointmentId);
CREATE INDEX IX_Payments_Status ON Payments(Status);
CREATE INDEX IX_Payments_CreatedAt ON Payments(CreatedAt);
```

### 2. PaymentTransactions Tablosu

```sql
CREATE TABLE PaymentTransactions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentId INT NOT NULL,
    
    -- Transaction Bilgileri
    TransactionId NVARCHAR(100) NOT NULL,         -- Iyzico Transaction ID
    TransactionType INT NOT NULL,                 -- 1=Payment, 2=Refund, 3=Cancel
    Status INT NOT NULL,                          -- 1=Pending, 2=Success, 3=Failed
    
    -- Tutar
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) DEFAULT 'EUR',
    
    -- Iyzico Response
    IyzicoResponse NVARCHAR(MAX),                 -- JSON response
    ErrorCode NVARCHAR(50),
    ErrorMessage NVARCHAR(500),
    
    -- Tarihler
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2,
    
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);

CREATE INDEX IX_PaymentTransactions_PaymentId ON PaymentTransactions(PaymentId);
CREATE INDEX IX_PaymentTransactions_TransactionId ON PaymentTransactions(TransactionId);
```

### 3. PaymentItems Tablosu (Sepet Detayları)

```sql
CREATE TABLE PaymentItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentId INT NOT NULL,
    
    -- Ürün Bilgileri
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    Quantity INT DEFAULT 1,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    
    -- İlişki
    RelatedEntityType NVARCHAR(50),               -- "Appointment", "Service"
    RelatedEntityId INT,
    
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);
```

### 4. PaymentRefunds Tablosu

```sql
CREATE TABLE PaymentRefunds (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PaymentId INT NOT NULL,
    
    -- İade Bilgileri
    RefundNumber NVARCHAR(50) UNIQUE NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency NVARCHAR(3) DEFAULT 'EUR',
    Reason NVARCHAR(500),
    Status INT NOT NULL DEFAULT 1,                -- 1=Pending, 2=Completed, 3=Failed
    
    -- Iyzico Bilgileri
    IyzicoRefundId NVARCHAR(100),
    IyzicoResponse NVARCHAR(MAX),
    
    -- Tarihler
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2,
    RefundedByUserId INT,                         -- Admin user ID
    
    FOREIGN KEY (PaymentId) REFERENCES Payments(Id)
);
```

### 5. Appointments Tablosuna Eklenmesi Gerekenler

```sql
-- Appointments tablosuna PaymentId ekle
ALTER TABLE Appointments
ADD PaymentId INT NULL;

ALTER TABLE Appointments
ADD CONSTRAINT FK_Appointments_Payments 
FOREIGN KEY (PaymentId) REFERENCES Payments(Id);

CREATE INDEX IX_Appointments_PaymentId ON Appointments(PaymentId);
```

---

## 🔧 Entegrasyon Adımları

### Adım 1: NuGet Paketi Ekleme

```xml
<!-- wixi.WebAPI/wixi.WebAPI.csproj -->
<ItemGroup>
  <PackageReference Include="Iyzipay" Version="2.1.0" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.2" />
</ItemGroup>
```

### Adım 2: appsettings.json Yapılandırması

```json
{
  "Iyzico": {
    "ApiKey": "sandbox-api-key",
    "SecretKey": "sandbox-secret-key",
    "BaseUrl": "https://sandbox-api.iyzipay.com",
    "CallbackUrl": "https://api.worklines.de/api/payments/webhooks/iyzico"
  }
}
```

### Adım 3: IyzicoOptions Sınıfı

```csharp
// wixi.Core/Configuration/IyzicoOptions.cs
public class IyzicoOptions
{
    public string ApiKey { get; set; }
    public string SecretKey { get; set; }
    public string BaseUrl { get; set; }
    public string CallbackUrl { get; set; }
    
    public Options ToIyzipayOptions()
    {
        return new Options
        {
            ApiKey = ApiKey,
            SecretKey = SecretKey,
            BaseUrl = BaseUrl
        };
    }
}
```

### Adım 4: Service Katmanı

```csharp
// wixi.Business/Interfaces/IPaymentService.cs
public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);
    Task<PaymentDto> GetPaymentAsync(int paymentId);
    Task<PaymentDto> GetPaymentByNumberAsync(string paymentNumber);
    Task<bool> CancelPaymentAsync(int paymentId, string reason);
    Task<bool> RefundPaymentAsync(int paymentId, decimal amount, string reason);
    Task ProcessWebhookAsync(WebhookPayload payload);
}
```

### Adım 5: Repository Katmanı

```csharp
// wixi.DataAccess/Repositories/IPaymentRepository.cs
public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment> GetByPaymentNumberAsync(string paymentNumber);
    Task<Payment> GetByIyzicoPaymentIdAsync(string iyzicoPaymentId);
    Task<List<Payment>> GetByAppointmentIdAsync(int appointmentId);
}
```

---

## 🔒 Güvenlik Gereksinimleri

### PCI-DSS Uyumluluk

1. **Kart Bilgileri Asla Saklanmaz**
   - Kart numarası, CVV, expire date backend'de saklanmaz
   - Sadece Iyzico'ya gönderilir
   - Son 4 hane masked olarak saklanabilir

2. **HTTPS Zorunlu**
   - Tüm ödeme endpoint'leri HTTPS üzerinden
   - HTTP istekleri reddedilir

3. **Tokenization**
   - Iyzico tokenization kullanılır
   - Tekrar ödemeler için token kullanılır
   - Kart bilgileri Iyzico'da saklanır

4. **Webhook Verification**
   - Tüm webhook'lar imza ile doğrulanır
   - HMAC-SHA256 signature kontrolü
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
// 1. Randevu oluşturulduktan sonra
var appointment = await _appointmentService.CreateAppointmentAsync(dto);

// 2. Ödeme oluştur
var paymentDto = new CreatePaymentDto
{
    Amount = appointment.Price,
    Currency = appointment.Currency,
    PaymentType = PaymentType.Appointment,
    RelatedEntityType = "Appointment",
    RelatedEntityId = appointment.Id,
    CustomerName = appointment.CustomerName,
    CustomerEmail = appointment.CustomerEmail,
    CustomerPhone = appointment.CustomerPhone,
    CardNumber = paymentFormDto.CardNumber,
    CardHolder = paymentFormDto.CardHolder,
    ExpireMonth = paymentFormDto.ExpireMonth,
    ExpireYear = paymentFormDto.ExpireYear,
    Cvc = paymentFormDto.Cvc
};

var payment = await _paymentService.CreatePaymentAsync(paymentDto);

// 3. Ödeme başarılı ise randevuyu onayla
if (payment.Status == PaymentStatus.Completed)
{
    appointment.Status = AppointmentStatus.Confirmed;
    appointment.PaymentId = payment.Id;
    await _appointmentService.UpdateAppointmentAsync(appointment);
}
else
{
    appointment.Status = AppointmentStatus.Cancelled;
    await _appointmentService.UpdateAppointmentAsync(appointment);
}
```

### Ödeme Durumuna Göre Randevu İşlemleri

| Ödeme Durumu | Randevu Durumu | Aksiyon |
|-------------|---------------|---------|
| Completed | Confirmed | Randevu onaylandı, email gönder |
| Failed | Cancelled | Randevu iptal edildi, kullanıcıya bilgi ver |
| Cancelled | Cancelled | Randevu iptal edildi |
| Refunded | Cancelled | İade yapıldı, randevu iptal |

---

## 📡 Webhook Yapısı

### Webhook Endpoint

```csharp
[HttpPost("webhooks/iyzico")]
public async Task<IActionResult> IyzicoWebhook([FromBody] WebhookPayload payload)
{
    // 1. Signature doğrulama
    var isValid = VerifyWebhookSignature(payload);
    if (!isValid)
    {
        return Unauthorized();
    }
    
    // 2. Webhook'u işle
    await _paymentService.ProcessWebhookAsync(payload);
    
    return Ok();
}
```

### Webhook Signature Doğrulama

```csharp
private bool VerifyWebhookSignature(WebhookPayload payload)
{
    var secretKey = _configuration["Iyzico:SecretKey"];
    var key = secretKey + payload.IyziEventType + payload.IyziPaymentId 
              + payload.Token + payload.PaymentConversationId + payload.Status;
    
    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
    var signature = BitConverter.ToString(hash).Replace("-", "").ToLower();
    
    return signature == payload.Signature;
}
```

### Webhook Event Tipleri

- `PAYMENT_SUCCESS` - Ödeme başarılı
- `PAYMENT_FAILURE` - Ödeme başarısız
- `REFUND_SUCCESS` - İade başarılı
- `CANCEL_SUCCESS` - İptal başarılı

---

## 📅 Implementation Plan

### Phase 1: Backend Hazırlıkları (3-4 Gün)

#### Day 1: Database & Entities
- [ ] Payments tablosu oluştur
- [ ] PaymentTransactions tablosu oluştur
- [ ] PaymentItems tablosu oluştur
- [ ] PaymentRefunds tablosu oluştur
- [ ] Appointments tablosuna PaymentId ekle
- [ ] Entity sınıfları oluştur
- [ ] Migration oluştur ve çalıştır

#### Day 2: Repository & Service Layer
- [ ] IPaymentRepository interface
- [ ] PaymentRepository implementation
- [ ] IPaymentService interface
- [ ] PaymentService implementation
- [ ] IyzicoOptions configuration
- [ ] IyzicoService wrapper

#### Day 3: Controller & API
- [ ] PaymentController oluştur
- [ ] CreatePayment endpoint
- [ ] GetPayment endpoint
- [ ] CancelPayment endpoint
- [ ] RefundPayment endpoint
- [ ] Webhook endpoint

#### Day 4: Testing & Integration
- [ ] Unit tests
- [ ] Integration tests
- [ ] Iyzico sandbox test
- [ ] Appointment entegrasyonu

### Phase 2: Frontend Entegrasyonu (2-3 Gün)

#### Day 1-2: Payment Form
- [ ] PaymentForm component'i güncelle
- [ ] Iyzico API entegrasyonu
- [ ] Error handling
- [ ] Loading states

#### Day 3: Payment Status
- [ ] Payment success page
- [ ] Payment failure page
- [ ] Payment history

### Phase 3: Randevu Entegrasyonu (1-2 Gün)

- [ ] Appointment oluşturma sonrası payment
- [ ] Payment callback'te appointment onaylama
- [ ] Payment failed durumunda appointment iptal
- [ ] Test senaryoları

---

## 🧪 Test Kartları

### Başarılı Ödeme Test Kartları

| Kart Numarası | Banka | Tip |
|--------------|-------|-----|
| 5528790000000008 | Halkbank | MasterCard (Credit) |
| 4603450000000000 | Denizbank | Visa (Credit) |
| 5451030000000000 | Yapı Kredi | MasterCard (Credit) |

### Hata Senaryoları Test Kartları

| Kart Numarası | Hata |
|--------------|------|
| 4111111111111129 | Yetersiz bakiye |
| 4125111111111115 | Süresi dolmuş kart |
| 4124111111111116 | Geçersiz CVV |

---

## 📝 Notlar

1. **Currency Conversion**: EUR → TRY dönüşümü gerekebilir
2. **Komisyon Hesaplama**: Iyzico komisyon oranı %2.875 (yaklaşık)
3. **Taksit Seçenekleri**: İleride taksit desteği eklenebilir
4. **Kart Kaydetme**: Tekrar ödemeler için kart kaydetme özelliği eklenebilir
5. **3D Secure**: Iyzico otomatik 3D Secure desteği sağlar

---

## 🔗 Kaynaklar

- [Iyzico Dokümantasyon](https://dev.iyzipay.com/tr)
- [Iyzico .NET SDK](https://github.com/iyzico/iyzipay-dotnet)
- [Iyzico Test Kartları](https://dev.iyzipay.com/tr/api/test-kartlari)

---

**Son Güncelleme**: 2025-01-XX
**Hazırlayan**: AI Assistant
**Versiyon**: 1.0

