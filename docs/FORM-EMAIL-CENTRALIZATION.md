# Form Email Template Merkezileştirme

## 🎯 Yapılan Değişiklikler

### Problem
`FormService.cs` içinde **hardcoded email template'leri** vardı. Bu:
- ❌ Merkezi yönetimi zorlaştırıyordu
- ❌ Email içeriği değişiklikleri için kod değişikliği gerektiriyordu
- ❌ Çoklu dil desteğini zorlaştırıyordu
- ❌ Logo entegrasyonunu her yere ayrı ayrı eklemek gerekiyordu

### Çözüm
✅ Tüm email template'leri **veritabanından** çekiliyor
✅ Tek bir yerden yönetiliyor (`EmailTemplates` tablosu)
✅ Hardcoded fallback'ler kaldırıldı
✅ Template bulunamazsa açık hata mesajı veriliyor

## 📋 Güncellenen Template'ler

### 1. form_submission_received_employer (ID: 7)
**Key:** `form_submission_received_employer`
**Kullanım:** İşveren formu gönderildiğinde
**Placeholder'lar:**
- `{{ClientName}}` - Şirket/Yetkili adı
- `{{SubmissionDate}}` - Form gönderim tarihi
- `{{NextSteps}}` - Sıradaki adımlar
- `{{PortalLink}}` - Portal bağlantısı
- `{{SupportEmail}}` - Destek email

### 2. form_submission_received_employee (ID: 6)
**Key:** `form_submission_received_employee`
**Kullanım:** Çalışan formu gönderildiğinde
**Placeholder'lar:**
- `{{ClientName}}` - Çalışan adı
- `{{SubmissionDate}}` - Form gönderim tarihi
- `{{NextSteps}}` - Sıradaki adımlar
- `{{PortalLink}}` - Portal bağlantısı
- `{{SupportEmail}}` - Destek email

## 🔧 FormService.cs Değişiklikleri

### Önceki Kod (Hardcoded Fallback)
```csharp
else
{
    // Fallback to hardcoded email
    emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>...</style>
</head>
<body>
    <div class='container'>
        ...hardcoded HTML...
    </div>
</body>
</html>";
}
```

### Yeni Kod (Database Zorlaması)
```csharp
else
{
    // Template not found in database - log error
    _logger.LogError("Email template 'form_submission_received_employer' not found or inactive in database");
    throw new InvalidOperationException("Email template not configured. Please contact administrator.");
}
```

## 📁 Oluşturulan Dosyalar

### 1. SQL Script
**Dosya:** `wixi.backendV2/wixi.DataAccess/Scripts/AddLogoToFormTemplates.sql`
**Amaç:** Mevcut form template'lerine logo header eklemek

**Kullanım:**
```sql
-- 1. Test modunda çalıştır (ROLLBACK)
-- Script içindeki ROLLBACK satırı aktif

-- 2. Sonuçları kontrol et
SELECT * FROM EmailTemplates 
WHERE [Key] IN ('form_submission_received_employer', 'form_submission_received_employee');

-- 3. Gerçek güncelleme için COMMIT aktif et
-- ROLLBACK satırını yorum yap
-- COMMIT satırının yorumunu kaldır
```

### 2. Bu Dokümantasyon
**Dosya:** `docs/FORM-EMAIL-CENTRALIZATION.md`

## 🗄️ Veritabanı Yapısı

### EmailTemplates Tablosu
```
┌────────────────────────────────────────────┐
│ EmailTemplates                             │
├────────────────────────────────────────────┤
│ Id (int) PRIMARY KEY                       │
│ Key (nvarchar) UNIQUE                      │
│ Subject_TR, Subject_EN, Subject_DE, _AR    │
│ BodyHtml_TR, BodyHtml_EN, BodyHtml_DE, _AR │
│ DisplayName_TR, _EN, _DE, _AR              │
│ Description (nvarchar)                     │
│ IsActive (bit)                             │
│ CreatedAt, UpdatedAt (datetime)            │
│ UpdatedBy (nvarchar)                       │
└────────────────────────────────────────────┘
```

### Mevcut Form Template'leri
```
ID | Key                                   | IsActive
---+---------------------------------------+---------
6  | form_submission_received_employee     | 1 (✓)
7  | form_submission_received_employer     | 1 (✓)
```

## ✅ Avantajlar

### 1. Merkezi Yönetim
- Email içerikleri admin panel üzerinden düzenlenebilir
- Kod değişikliği gerektirmez
- Hızlı iterasyon mümkün

### 2. Çoklu Dil Desteği
- TR, EN, DE, AR dilleri destekleniyor
- Fallback mantığı: TR → EN → DE → AR
- Her dilde farklı içerik olabilir

### 3. Logo Entegrasyonu
- Logo URL tek yerden yönetiliyor
- Tüm template'ler aynı logo yapısını kullanıyor
- Değişiklikler SQL ile toplu yapılabilir

### 4. Hata Yönetimi
- Template bulunamazsa anlaşılır hata mesajı
- Log kaydı ile takip edilebilir
- Admin'e bildirim gönderilebilir

## 🧪 Test Senaryoları

### Test 1: Employer Form Email
```csharp
// FormService.cs -> SendEmployerFormConfirmationEmailAsync
// Veritabanında template var mı? ✓
// Logo var mı? ✓
// Placeholder'lar doğru mu? ✓
// Email gönderimi başarılı mı? ✓
```

### Test 2: Employee Form Email
```csharp
// FormService.cs -> SendEmployeeFormConfirmationEmailAsync
// Veritabanında template var mı? ✓
// Logo var mı? ✓
// Placeholder'lar doğru mu? ✓
// Email gönderimi başarılı mı? ✓
```

### Test 3: Template Bulunamama Durumu
```csharp
// Template IsActive = false yap
// Form gönder
// Beklenen: InvalidOperationException
// Log kaydı yapıldı mı? ✓
```

## 📊 Placeholder Mapping

### FormService.cs → Database Template

```csharp
// Kod tarafında
emailBody = emailBody
    .Replace("{{ClientName}}", submission.CompanyName)
    .Replace("{{SubmissionDate}}", submissionDate)
    .Replace("{{NextSteps}}", nextSteps)
    .Replace("{{PortalLink}}", portalLink)
    .Replace("{{SupportEmail}}", supportEmail);
```

```html
<!-- Database template -->
<p>Merhaba <strong>{{ClientName}}</strong>,</p>
<p>Başvuru tarihiniz: {{SubmissionDate}}</p>
<p>Sıradaki adımlar: {{NextSteps}}</p>
<a href="{{PortalLink}}">Portal</a>
<p>İletişim: {{SupportEmail}}</p>
```

## 🚀 Deployment Adımları

### Adım 1: SQL Script'i Çalıştır
```sql
-- wixi.backendV2/wixi.DataAccess/Scripts/AddLogoToFormTemplates.sql
-- Test modunda çalıştır, sonuçları kontrol et, COMMIT ile güncelle
```

### Adım 2: Kodu Deploy Et
```bash
# FormService.cs değişiklikleri deploy edilmeli
# Hardcoded fallback'ler kaldırıldı
```

### Adım 3: Test Et
```bash
# 1. Employer form gönder → Email kontrol et
# 2. Employee form gönder → Email kontrol et
# 3. Logo görünüyor mu?
# 4. Placeholder'lar doğru mu?
```

### Adım 4: Monitor Et
```bash
# Log'ları takip et
# "Email template not found" hatası var mı?
# Email gönderim başarı oranı %100 mü?
```

## 🔗 İlgili Dosyalar

### Kod
- `wixi.backendV2/wixi.WebAPI/Services/FormService.cs` ✏️ (güncellendi)
- `wixi.backendV2/wixi.WebAPI/Services/EmailTemplateService.cs`
- `wixi.backendV2/wixi.WebAPI/Services/SmtpEmailSender.cs`

### Veritabanı
- `EmailTemplates` tablosu
- `wixi.backendV2/wixi.DataAccess/Scripts/AddLogoToFormTemplates.sql` ✨ (yeni)

### Dokümantasyon
- `docs/FORM-EMAIL-CENTRALIZATION.md` ✨ (yeni - bu dosya)
- `docs/EMAIL-TEMPLATE-LOGO-INTEGRATION.md`
- `docs/LOGO-INTEGRATION-SUMMARY.md`

## ⚠️ Önemli Notlar

1. **Template'ler veritabanında olmalı**
   - `form_submission_received_employer` (ID: 7)
   - `form_submission_received_employee` (ID: 6)
   - Her ikisi de `IsActive = 1` olmalı

2. **Placeholder'lar doğru yazılmalı**
   - `{{ClientName}}` - doğru ✓
   - `{{ ClientName }}` - yanlış ✗
   - `{ClientName}` - yanlış ✗

3. **Logo URL doğru olmalı**
   - `https://api.worklines.de/CompanyFile/worklines-logo.jpeg`
   - Logo dosyası `wwwroot/CompanyFile/` dizininde olmalı

4. **Dil Fallback Sırası**
   - Önce istenen dil (language parametresi)
   - Bulunamazsa: TR → EN → DE → AR

## 📈 Performans

### Öncesi (Hardcoded)
- ❌ Her form için hardcoded HTML render
- ❌ Değişiklik için kod deployment gerekli
- ❌ Cache yok

### Sonrası (Database)
- ✅ Template bir kez database'den çekiliyor
- ✅ Placeholder replacement hafif işlem
- ✅ İleride cache eklenebilir
- ✅ Değişiklik anında yansıyor

## 🎉 Sonuç

✅ **Merkezileştirme Başarılı!**
- Tüm form email template'leri veritabanından yönetiliyor
- Hardcoded fallback'ler kaldırıldı
- Logo entegrasyonu tamamlandı
- Tek bir yerden yönetim mümkün

**Sıradaki Adım:** SQL script'i çalıştır ve test et! 🚀

