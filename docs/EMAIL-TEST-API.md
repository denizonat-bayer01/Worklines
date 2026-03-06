# Email Test API Dokümantasyonu

## 🎯 Amaç

Form göndermeden direkt email template'leri test etmek için admin API endpoint'leri.

## 📋 Endpoint'ler

### 1. Tüm Template'leri Listele

**GET** `/api/v1.0/emailtest/templates`

Aktif tüm email template'lerini listeler.

#### Örnek İstek:
```bash
curl http://localhost:5045/api/v1.0/emailtest/templates
```

#### Örnek Yanıt:
```json
{
  "success": true,
  "count": 5,
  "templates": [
    {
      "key": "form_submission_received_employer",
      "displayName_TR": "🏢 İşveren Başvurusu Alındı",
      "displayName_EN": "🏢 Employer Application Received",
      "description": "İşveren formu sonrası otomatik hoş geldin e-postası",
      "placeholders": [
        "ClientName",
        "NextSteps",
        "PortalLink",
        "SubmissionDate",
        "SupportEmail"
      ]
    },
    {
      "key": "form_submission_received_employee",
      "displayName_TR": "👤 Çalışan Başvurusu Alındı",
      "displayName_EN": "👤 Employee Application Received",
      "description": "Çalışan formu sonrası otomatik hoş geldin e-postası",
      "placeholders": [
        "ClientName",
        "NextSteps",
        "PortalLink",
        "SubmissionDate",
        "SupportEmail"
      ]
    }
  ]
}
```

---

### 2. Belirli Bir Template Detayı

**GET** `/api/v1.0/emailtest/templates/{key}`

Belirli bir template'in detaylarını ve varsayılan placeholder değerlerini getirir.

#### Örnek İstek:
```bash
curl http://localhost:5045/api/v1.0/emailtest/templates/form_submission_received_employer
```

#### Örnek Yanıt:
```json
{
  "success": true,
  "template": {
    "key": "form_submission_received_employer",
    "displayName_TR": "🏢 İşveren Başvurusu Alındı",
    "displayName_EN": "🏢 Employer Application Received",
    "description": "İşveren formu sonrası otomatik hoş geldin e-postası",
    "subject_TR": "İşveren başvurunuz alındı – {{SubmissionDate}}",
    "subject_EN": "Your employer request has been received – {{SubmissionDate}}",
    "isActive": true,
    "updatedAt": "2025-11-22T01:09:34"
  },
  "placeholders": [
    "ClientName",
    "NextSteps",
    "PortalLink",
    "SubmissionDate",
    "SupportEmail"
  ],
  "defaultPlaceholderValues": {
    "ClientName": "Test Kullanıcı",
    "SubmissionDate": "22/11/2025",
    "NextSteps": "• Manuel inceleme yapılacak\n• Müşteri kodu atanacak\n• Belge listesi paylaşılacak",
    "PortalLink": "https://portal.worklines.de",
    "SupportEmail": "support@worklines.de"
  }
}
```

---

### 3. Test Email Gönder ⭐

**POST** `/api/v1.0/emailtest/send`

Belirli bir template ile test email gönderir.

#### Request Body:
```json
{
  "templateKey": "form_submission_received_employer",
  "toEmail": "test@example.com",
  "language": "tr",
  "placeholders": {
    "ClientName": "Test Şirketi A.Ş.",
    "SubmissionDate": "22/11/2025",
    "NextSteps": "• Başvurunuz inceleniyor\n• 2 iş günü içinde dönüş yapılacak",
    "PortalLink": "https://portal.worklines.de",
    "SupportEmail": "support@worklines.de"
  }
}
```

#### Parametre Açıklamaları:
- **templateKey** (zorunlu): Email template key'i
- **toEmail** (zorunlu): Alıcı email adresi
- **language** (opsiyonel): Dil kodu (tr, en, de, ar) - varsayılan: "tr"
- **placeholders** (opsiyonel): Özel placeholder değerleri. Verilmezse varsayılan değerler kullanılır.

#### Örnek İstek (cURL):
```bash
curl -X POST http://localhost:5045/api/v1.0/emailtest/send \
  -H "Content-Type: application/json" \
  -d '{
    "templateKey": "form_submission_received_employer",
    "toEmail": "ahmetcanbozkurt1903@gmail.com",
    "language": "tr"
  }'
```

#### Örnek İstek (PowerShell):
```powershell
$body = @{
    templateKey = "form_submission_received_employer"
    toEmail = "ahmetcanbozkurt1903@gmail.com"
    language = "tr"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5045/api/v1.0/emailtest/send" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

#### Örnek Yanıt:
```json
{
  "success": true,
  "message": "Test email başarıyla gönderildi",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "templateKey": "form_submission_received_employer",
  "toEmail": "ahmetcanbozkurt1903@gmail.com",
  "language": "tr",
  "subject": "[TEST] İşveren başvurunuz alındı – 22/11/2025",
  "placeholdersUsed": {
    "ClientName": "Test Kullanıcı",
    "SubmissionDate": "22/11/2025",
    "NextSteps": "• Manuel inceleme yapılacak\n• Müşteri kodu atanacak\n• Belge listesi paylaşılacak",
    "PortalLink": "https://portal.worklines.de",
    "SupportEmail": "support@worklines.de"
  }
}
```

---

## 🧪 Test Senaryoları

### Senaryo 1: İşveren Formu Email'i Test Et

```bash
curl -X POST http://localhost:5045/api/v1.0/emailtest/send \
  -H "Content-Type: application/json" \
  -d '{
    "templateKey": "form_submission_received_employer",
    "toEmail": "test@example.com",
    "language": "tr"
  }'
```

**Kontrol Et:**
- ✅ Email geldi mi?
- ✅ Logo görünüyor mu?
- ✅ Placeholder'lar doğru mu?
- ✅ Footer doğru mu?

---

### Senaryo 2: Çalışan Formu Email'i Test Et

```bash
curl -X POST http://localhost:5045/api/v1.0/emailtest/send \
  -H "Content-Type: application/json" \
  -d '{
    "templateKey": "form_submission_received_employee",
    "toEmail": "test@example.com",
    "language": "en"
  }'
```

---

### Senaryo 3: Özel Placeholder Değerleri ile Test

```bash
curl -X POST http://localhost:5045/api/v1.0/emailtest/send \
  -H "Content-Type: application/json" \
  -d '{
    "templateKey": "form_submission_received_employer",
    "toEmail": "test@example.com",
    "language": "tr",
    "placeholders": {
      "ClientName": "Özel Şirket Adı",
      "SubmissionDate": "25/12/2025",
      "NextSteps": "• Özel adım 1\n• Özel adım 2\n• Özel adım 3",
      "PortalLink": "https://custom-portal.example.com",
      "SupportEmail": "custom-support@example.com"
    }
  }'
```

---

### Senaryo 4: Tüm Template'leri Toplu Test Et

```bash
# Önce template listesini al
curl http://localhost:5045/api/v1.0/emailtest/templates > templates.json

# Her template için test email gönder
for key in form_submission_received_employer form_submission_received_employee; do
  curl -X POST http://localhost:5045/api/v1.0/emailtest/send \
    -H "Content-Type: application/json" \
    -d "{
      \"templateKey\": \"$key\",
      \"toEmail\": \"test@example.com\",
      \"language\": \"tr\"
    }"
  echo "Test email sent for $key"
  sleep 2
done
```

---

## 🔍 Email Log Kontrolü

Test email'leri gönderdikten sonra veritabanında kontrol edin:

```sql
-- Son 10 test email
SELECT TOP 10
    Id,
    ToEmails,
    Subject,
    Status,
    TemplateKey,
    CreatedAt,
    LastError
FROM EmailLogs
WHERE MetadataJson LIKE '%TestEmail%'
ORDER BY CreatedAt DESC;

-- Test email'lerde logo var mı?
SELECT 
    TemplateKey,
    ToEmails,
    CASE 
        WHEN BodyHtml LIKE '%api.worklines.de/CompanyFile/worklines-logo.jpeg%' 
        THEN 'Logo Var ✓' 
        ELSE 'Logo Yok ✗' 
    END AS LogoDurumu,
    Status,
    CreatedAt
FROM EmailLogs
WHERE MetadataJson LIKE '%TestEmail%'
ORDER BY CreatedAt DESC;

-- Başarısız email'ler
SELECT *
FROM EmailLogs
WHERE Status = 'Failed'
  AND MetadataJson LIKE '%TestEmail%'
ORDER BY CreatedAt DESC;
```

---

## 📊 Varsayılan Placeholder Değerleri

API otomatik olarak şu varsayılan değerleri kullanır:

| Placeholder | Varsayılan Değer |
|------------|------------------|
| `{{ClientName}}` | "Test Kullanıcı" |
| `{{FullName}}` | "Test Kullanıcı" |
| `{{CompanyName}}` | "Test Şirketi A.Ş." |
| `{{ContactPerson}}` | "Ahmet Yılmaz" |
| `{{SubmissionDate}}` | Bugünün tarihi (dd/MM/yyyy) |
| `{{ClientCode}}` | "WL-TEST-12345" |
| `{{NextSteps}}` | "• Manuel inceleme yapılacak\n• Müşteri kodu atanacak\n• Belge listesi paylaşılacak" |
| `{{PortalLink}}` | "https://portal.worklines.de" |
| `{{SupportEmail}}` | "support@worklines.de" |
| `{{Profession}}` | "Yazılım Geliştirici" |
| `{{Experience}}` | "5" |
| `{{GermanLevel}}` | "B2" |

*Tam liste için: GET `/api/v1.0/emailtest/templates/{key}` endpoint'ini kullanın.*

---

## ⚙️ Konfigürasyon

API, `appsettings.json`'dan şu değerleri kullanır:

```json
{
  "Portal": {
    "BaseUrl": "https://portal.worklines.de"
  },
  "Support": {
    "Email": "support@worklines.de"
  }
}
```

---

## 🚨 Hata Durumları

### Template Bulunamadı
```json
{
  "error": "Template 'invalid_key' bulunamadı veya aktif değil"
}
```

**Çözüm:** Geçerli bir template key kullanın. `GET /api/v1.0/emailtest/templates` ile listeyi kontrol edin.

### Email Gönderimi Başarısız
```json
{
  "error": "Email gönderimi başarısız",
  "details": "SMTP server connection failed"
}
```

**Çözüm:** 
- SMTP ayarlarını kontrol edin
- EmailLogs tablosunda `LastError` kolonu detayları gösterir

---

## 🔐 Güvenlik Notları

⚠️ **ÖNEMLİ:** Bu API endpoint'leri **sadece development/test ortamında** kullanılmalı!

Production ortamında:
1. Authorization ekleyin (Admin rolü gerekli)
2. Rate limiting uygulayın
3. IP whitelist kullanın
4. Test email'lere `[TEST]` prefix'i eklensin (zaten yapılıyor ✓)

---

## 📱 Postman Collection

### Collection Setup

1. **Base URL Variable:**
   ```
   {{base_url}} = http://localhost:5045
   ```

2. **Requests:**

#### List All Templates
```
GET {{base_url}}/api/v1.0/emailtest/templates
```

#### Get Template Detail
```
GET {{base_url}}/api/v1.0/emailtest/templates/form_submission_received_employer
```

#### Send Test Email
```
POST {{base_url}}/api/v1.0/emailtest/send
Content-Type: application/json

{
  "templateKey": "form_submission_received_employer",
  "toEmail": "test@example.com",
  "language": "tr"
}
```

---

## ✅ Test Checklist

- [ ] API endpoint'leri çalışıyor mu?
- [ ] Template listesi geliyor mu?
- [ ] Template detayı geliyor mu?
- [ ] Test email gönderilebiliyor mu?
- [ ] Email'de logo var mı?
- [ ] Placeholder'lar doğru çalışıyor mu?
- [ ] Farklı diller test edildi mi? (tr, en, de)
- [ ] EmailLogs tablosunda kayıt oluşuyor mu?
- [ ] Subject'e [TEST] prefix'i eklenmiş mi?
- [ ] Hata durumları düzgün handle ediliyor mu?

---

## 🎉 Kullanım Örneği

Admin panel'den test email göndermek için:

```javascript
// Frontend (Admin Panel)
async function sendTestEmail(templateKey, toEmail) {
  const response = await fetch('/api/v1.0/emailtest/send', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      templateKey,
      toEmail,
      language: 'tr'
    })
  });
  
  const result = await response.json();
  
  if (result.success) {
    alert(`Test email gönderildi! CorrelationId: ${result.correlationId}`);
  } else {
    alert(`Hata: ${result.error}`);
  }
}

// Kullanım
sendTestEmail('form_submission_received_employer', 'test@example.com');
```

---

**Artık form göndermeden direkt email test edebilirsiniz! 🚀**

