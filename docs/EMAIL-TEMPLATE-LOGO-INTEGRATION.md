# Email Template Logo Entegrasyonu

## Logo URL
Tüm email template'lerinde kullanılacak logo URL'si:
```
https://api.worklines.de/CompanyFile/worklines-logo.jpeg
```

## Standart Email Header Yapısı

Tüm email template'lerinde aşağıdaki header yapısı kullanılmalıdır:

```html
<div style="max-width:640px;margin:0 auto;font-family:'Inter',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;">
  <div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;">
    <!-- Logo Header -->
    <div style="background:#ffffff;padding:24px;text-align:center;border-bottom:1px solid #e2e8f0;">
      <img src="https://api.worklines.de/CompanyFile/worklines-logo.jpeg" alt="Worklines Logo" style="max-width:180px;height:auto;display:inline-block;" />
    </div>
    
    <!-- Colored Title Section -->
    <div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;">
      <span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">{{SectionLabel}}</span>
      <h2 style="margin:8px 0 0;font-size:24px;">{{EmailTitle}}</h2>
    </div>
    
    <!-- Content Section -->
    <div style="padding:32px;background:#f9fafb;">
      <!-- Email içeriği buraya gelir -->
    </div>
    
    <!-- Footer -->
    <div style="background:#f3f4f6;padding:20px;text-align:center;border-top:1px solid #e2e8f0;">
      <p style="margin:0 0 8px;font-size:13px;color:#64748b;">
        © 2024 Worklines Pro. Tüm hakları saklıdır.
      </p>
      <p style="margin:0;font-size:13px;color:#64748b;">
        <a href="https://portal.worklines.de" style="color:#0f766e;text-decoration:none;">Portal</a> | 
        <a href="mailto:support@worklines.de" style="color:#0f766e;text-decoration:none;">Destek</a>
      </p>
    </div>
  </div>
</div>
```

## Veritabanında Güncelleme

Mevcut email template'lerini güncellemek için aşağıdaki SQL script'i kullanılabilir:

```sql
-- Logo header'ı ekleyen fonksiyon
DECLARE @logoHeader NVARCHAR(MAX) = 
'<div style="background:#ffffff;padding:24px;text-align:center;border-bottom:1px solid #e2e8f0;">
  <img src="https://api.worklines.de/CompanyFile/worklines-logo.jpeg" alt="Worklines Logo" style="max-width:180px;height:auto;display:inline-block;" />
</div>';

-- Tüm template'lerde logo header yoksa ekle
UPDATE EmailTemplates
SET 
    BodyHtml_TR = REPLACE(BodyHtml_TR, 
        '<div style="background:linear-gradient', 
        @logoHeader + '<div style="background:linear-gradient'),
    BodyHtml_EN = REPLACE(BodyHtml_EN, 
        '<div style="background:linear-gradient', 
        @logoHeader + '<div style="background:linear-gradient'),
    BodyHtml_DE = REPLACE(BodyHtml_DE, 
        '<div style="background:linear-gradient', 
        @logoHeader + '<div style="background:linear-gradient'),
    BodyHtml_AR = REPLACE(BodyHtml_AR, 
        '<div style="background:linear-gradient', 
        @logoHeader + '<div style="background:linear-gradient'),
    UpdatedAt = GETUTCDATE()
WHERE 
    BodyHtml_TR NOT LIKE '%api.worklines.de/CompanyFile/worklines-logo.jpeg%'
    OR BodyHtml_EN NOT LIKE '%api.worklines.de/CompanyFile/worklines-logo.jpeg%'
    OR BodyHtml_DE NOT LIKE '%api.worklines.de/CompanyFile/worklines-logo.jpeg%'
    OR BodyHtml_AR NOT LIKE '%api.worklines.de/CompanyFile/worklines-logo.jpeg%';
```

## Örnek: Form Submission Template (İşveren)

### Türkçe (TR)
```html
<div style="max-width:640px;margin:0 auto;font-family:'Inter',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;">
  <div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;">
    <!-- Logo Header -->
    <div style="background:#ffffff;padding:24px;text-align:center;border-bottom:1px solid #e2e8f0;">
      <img src="https://api.worklines.de/CompanyFile/worklines-logo.jpeg" alt="Worklines Logo" style="max-width:180px;height:auto;display:inline-block;" />
    </div>
    
    <div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;">
      <span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Başvuru Onayı</span>
      <h2 style="margin:8px 0 0;font-size:24px;">İşveren Başvurunuz Alındı</h2>
    </div>
    
    <div style="padding:32px;background:#f9fafb;">
      <div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;">
        <p style="margin:0 0 16px;font-size:15px;">Sayın <strong>{{ClientName}}</strong>,</p>
        <p style="margin:0 0 16px;font-size:15px;">
          İşveren başvurunuz başarıyla tarafımıza ulaştı. Ekibimiz en kısa sürede başvurunuzu inceleyecek ve sizinle iletişime geçecektir.
        </p>
        
        <div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;">
          <p style="margin:0;font-weight:600;color:#047857;font-size:14px;">📋 Başvuru Bilgileri</p>
          <p style="margin:6px 0 0;font-size:14px;color:#065f46;">
            <strong>Başvuru Tarihi:</strong> {{SubmissionDate}}
          </p>
        </div>
        
        <div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#fef3c7;border:1px solid #fde047;">
          <p style="margin:0;font-weight:600;color:#92400e;font-size:14px;">Sıradaki Adımlar</p>
          <p style="margin:6px 0 0;font-size:14px;color:#78350f;">{{NextSteps}}</p>
        </div>
        
        <p style="margin:0 0 16px;font-size:15px;">
          Portalımıza giriş yapmak için: 
          <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a>
        </p>
        
        <p style="margin:0 0 16px;font-size:15px;">
          Sorularınız için <strong>{{SupportEmail}}</strong> adresinden bize ulaşabilirsiniz.
        </p>
        
        <p style="margin:0;font-weight:600;color:#0f172a;">Worklines Destek Ekibi</p>
      </div>
    </div>
    
    <div style="background:#f3f4f6;padding:20px;text-align:center;border-top:1px solid #e2e8f0;">
      <p style="margin:0 0 8px;font-size:13px;color:#64748b;">
        © 2024 Worklines Pro. Tüm hakları saklıdır.
      </p>
      <p style="margin:0;font-size:13px;color:#64748b;">
        <a href="https://portal.worklines.de" style="color:#0f766e;text-decoration:none;">Portal</a> | 
        <a href="mailto:support@worklines.de" style="color:#0f766e;text-decoration:none;">Destek</a>
      </p>
    </div>
  </div>
</div>
```

## Template Key'leri

Güncellenmesi gereken template key'leri:
1. `form_submission_received_employer` - İşveren form başvurusu
2. `form_submission_received_employee` - Çalışan form başvurusu
3. `equivalency_document_uploaded` - Denklik belgesi yükleme
4. `application_step_completed` - Başvuru adımı tamamlama
5. `document_approval_notification` - Belge onay bildirimi
6. `client_code_assigned` - Müşteri kodu atama
7. Diğer tüm email template'leri

## Uygulama Adımları

1. ✅ Logo URL belirlendi: `https://api.worklines.de/CompanyFile/worklines-logo.jpeg`
2. ⏳ Veritabanındaki mevcut template'leri güncelle
3. ⏳ Yeni oluşturulacak template'ler için standart yapıyı kullan
4. ⏳ Test email'leri gönder ve logo görünümünü kontrol et

## Notlar

- Logo responsive olmalı (max-width:180px)
- Email client'larında (Gmail, Outlook, vb.) düzgün görünmeli
- Logo yüklenme hatası durumunda alt text görünmeli
- Tüm dillerdeki template'ler (TR, EN, DE, AR) aynı logo URL'sini kullanmalı

