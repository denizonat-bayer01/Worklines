## Email Automation Tasks - Detaylı Yol Haritası

### Genel Bakış
Bu döküman, form başvuru sürecinden başlayarak belge yükleme ve başvuru ilerlemesi aşamalarında otomatik e-posta gönderimlerinin detaylı planını içermektedir.

---

## 1. Form Başvurusu Hoş Geldin E-postaları

### 1.1 Çalışan Formu (`form_submission_received_employee`)

**Tetikleyici:**
- Endpoint: `POST /api/v1.0/forms/employee`
- Service: `FormService.SubmitEmployeeFormAsync`
- Timing: Form kaydı başarılı olduktan hemen sonra

**Placeholder'lar:**
- `{{ClientName}}` - Form sahibinin ad soyad
- `{{SubmissionDate}}` - Form gönderim tarihi (formatted)
- `{{SupportEmail}}` - Destek e-posta adresi (config'den)
- `{{PortalLink}}` - Client portal bağlantısı
- `{{NextSteps}}` - Sıradaki adımların listesi (manuel inceleme, müşteri kodu atama, belge listesi gönderimi)

**Konu (TR):** `Başvurunuz tarafımıza ulaştı – {{SubmissionDate}}`  
**Konu (EN):** `We received your application – {{SubmissionDate}}`

**Backend Değişiklikler:**
```csharp
// FormService.cs
public async Task<EmployeeFormSubmissionDto> SubmitEmployeeFormAsync(EmployeeFormDto dto)
{
    // ... mevcut form kaydetme mantığı ...
    
    // Email gönderimi
    var emailPlaceholders = new Dictionary<string, string>
    {
        {"ClientName", $"{dto.FirstName} {dto.LastName}"},
        {"SubmissionDate", DateTime.UtcNow.ToString("dd/MM/yyyy")},
        {"SupportEmail", _configuration["Support:Email"]},
        {"PortalLink", _configuration["Portal:BaseUrl"]},
        {"NextSteps", "• Başvurunuzun manuel incelenmesi\n• Müşteri kodu oluşturulması\n• Belge listesinin paylaşılması"}
    };
    
    await _emailService.SendTemplatedEmailAsync(
        "form_submission_received_employee",
        dto.Email,
        emailPlaceholders,
        preferredLanguage: "tr"
    );
    
    return result;
}
```

---

### 1.2 İşveren Formu (`form_submission_received_employer`)

**Tetikleyici:**
- Endpoint: `POST /api/v1.0/forms/employer`
- Service: `FormService.SubmitEmployerFormAsync`
- Timing: Form kaydı başarılı olduktan hemen sonra

**Placeholder'lar:**
- `{{ClientName}}` - Şirket adı veya yetkili kişi
- `{{CompanyName}}` - Şirket adı
- `{{SubmissionDate}}` - Form gönderim tarihi
- `{{SupportEmail}}` - Destek e-posta
- `{{PortalLink}}` - Portal link
- `{{NextSteps}}` - Adımlar

**Konu (TR):** `İşveren başvurunuz alındı – {{SubmissionDate}}`  
**Konu (EN):** `Your employer request has been received – {{SubmissionDate}}`

---

## 2. Denklik Belgesi Yükleme Bildirimi

### 2.1 Template: `equivalency_document_uploaded`

**Tetikleyici:**
- Admin panel: Belge yükleme/düzenleme sayfası
- Context: Step 1, Sub-step 4 (Denklik Belgesi)
- Service: `DocumentService.UploadDocumentAsync` veya admin upload endpoint
- Koşul: Yüklenen belge türü = "denklik" ve client'a ait

**Placeholder'lar:**
- `{{ClientName}}` - Müşteri adı
- `{{DocumentName}}` - Belge adı ("Denklik Belgesi")
- `{{UploadedAt}}` - Yükleme tarihi/saati
- `{{PortalLink}}` - Portal bağlantısı
- `{{NextSteps}}` - Sıradaki adımlar (örn: "Belgeyi portaldan indirebilir, başvuru sürecine devam edebilirsiniz")

**Konu (TR):** `Denklik Belgeniz Hazır – {{UploadedAt}}`  
**Konu (EN):** `Your Equivalency Document is Ready – {{UploadedAt}}`

**HTML İçerik (TR):**
```html
<div style="max-width:640px;margin:0 auto;font-family:'Inter',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;">
  <div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;">
    <div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;">
      <span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Belge Yüklendi</span>
      <h2 style="margin:8px 0 0;font-size:24px;">Denklik Belgeniz Hazır</h2>
    </div>
    <div style="padding:32px;background:#f9fafb;">
      <div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;">
        <p style="margin:0 0 16px;font-size:15px;">Merhaba <strong>{{ClientName}}</strong>,</p>
        <p style="margin:0 0 16px;font-size:15px;">
          <strong>{{DocumentName}}</strong> belgeniz tarafımızca sisteme yüklenmiştir.
        </p>
        <div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;">
          <p style="margin:0;font-weight:600;color:#047857;font-size:14px;">📄 Belge Bilgileri</p>
          <p style="margin:6px 0 0;font-size:14px;color:#065f46;">
            <strong>Yükleme Tarihi:</strong> {{UploadedAt}}<br/>
            <strong>Belge Türü:</strong> Denklik Belgesi
          </p>
        </div>
        <div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#fef3c7;border:1px solid #fde047;">
          <p style="margin:0;font-weight:600;color:#92400e;font-size:14px;">Sıradaki Adımlar</p>
          <p style="margin:6px 0 0;font-size:14px;color:#78350f;">{{NextSteps}}</p>
        </div>
        <p style="margin:0 0 16px;font-size:15px;">
          Belgeyi portaldan görüntüleyebilir ve indirebilirsiniz: 
          <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a>
        </p>
        <p style="margin:0 0 16px;font-size:15px;">
          Sorularınız için <strong>{{SupportEmail}}</strong> adresinden bize ulaşabilirsiniz.
        </p>
        <p style="margin:0;font-weight:600;color:#0f172a;">Worklines Belge Destek Ekibi</p>
      </div>
    </div>
  </div>
</div>
```

**Backend Değişiklikler:**
```csharp
// DocumentService.cs veya Admin Upload Controller
public async Task<DocumentDto> UploadDocumentAsync(int clientId, int documentTypeId, IFormFile file)
{
    // ... mevcut yükleme mantığı ...
    
    // Eğer documentTypeId denklik belgesi ise (Step 1, SubStep 4 context)
    if (await IsDenklikDocument(documentTypeId))
    {
        var client = await _context.Clients.FindAsync(clientId);
        
        var emailPlaceholders = new Dictionary<string, string>
        {
            {"ClientName", client.FullName},
            {"DocumentName", "Denklik Belgesi"},
            {"UploadedAt", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")},
            {"PortalLink", $"{_configuration["Portal:BaseUrl"]}/documents"},
            {"NextSteps", "• Belgenizi portaldan indirin\n• 2. adım otomatik başlatıldı\n• Çalışma müsaadesi süreci başlayacak"},
            {"SupportEmail", _configuration["Support:Email"]}
        };
        
        await _emailService.SendTemplatedEmailAsync(
            "equivalency_document_uploaded",
            client.Email,
            emailPlaceholders,
            preferredLanguage: client.PreferredLanguage ?? "tr"
        );
    }
    
    return result;
}
```

---

## 3. Step/Sub-step İlerleme Bildirimleri

### 3.1 Genel Strateji

**Seçenek A: Tek Generic Template**  
- Template Key: `application_progress_updated`
- Placeholder'larla step/substep isimlerini dinamik gösterme
-장점: Tek şablon yönetimi kolay
- Eksi: Özelleştirilmiş mesajlar için esnek değil

**Seçenek B: Step-Specific Templates** (ÖNERİLEN)
- Her step/substep için ayrı template
- Keys: `progress_step1_substep1`, `progress_step1_substep2`, vb.
- 장점: Her adım için özel mesaj/yönlendirme
- Eksi: Daha fazla şablon yönetimi

### 3.2 Template Yapısı (Generic Örnek)

**Template Key:** `application_progress_updated`

**Placeholder'lar:**
- `{{ClientName}}` - Müşteri adı
- `{{StepTitle}}` - Ana adım başlığı ("Denklik İşlemleri")
- `{{SubStepName}}` - Alt adım adı ("Belgeler Yüklendi")
- `{{CompletionDate}}` - Tamamlanma tarihi
- `{{ProgressPercentage}}` - İlerleme yüzdesi
- `{{NextStepTitle}}` - Sıradaki adım başlığı
- `{{NextStepDescription}}` - Sıradaki adım açıklaması
- `{{PortalLink}}` - Portal bağlantısı
- `{{SupportEmail}}` - Destek e-posta

**Konu (TR):** `İlerleme Kaydedildi: {{StepTitle}} – {{SubStepName}}`  
**Konu (EN):** `Progress Updated: {{StepTitle}} – {{SubStepName}}`

**HTML İçerik (TR):**
```html
<div style="max-width:640px;margin:0 auto;font-family:'Inter',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;">
  <div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;">
    <div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;">
      <span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Başvuru İlerlemesi</span>
      <h2 style="margin:8px 0 0;font-size:24px;">Yeni Adım Tamamlandı</h2>
    </div>
    <div style="padding:32px;background:#f9fafb;">
      <div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;">
        <p style="margin:0 0 16px;font-size:15px;">Merhaba <strong>{{ClientName}}</strong>,</p>
        <p style="margin:0 0 16px;font-size:15px;">
          Başvuru sürecinizde yeni bir adım tamamlandı! 🎉
        </p>
        
        <div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;">
          <p style="margin:0;font-weight:600;color:#047857;font-size:14px;">✅ Tamamlanan Adım</p>
          <p style="margin:6px 0 0;font-size:14px;color:#065f46;">
            <strong>Ana Adım:</strong> {{StepTitle}}<br/>
            <strong>Alt Adım:</strong> {{SubStepName}}<br/>
            <strong>Tamamlanma:</strong> {{CompletionDate}}
          </p>
        </div>
        
        <div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#eff6ff;border:1px solid #bfdbfe;">
          <p style="margin:0;font-weight:600;color:#1e40af;font-size:14px;">📊 İlerleme Durumu</p>
          <div style="margin:8px 0 0;background:#dbeafe;border-radius:999px;height:12px;overflow:hidden;">
            <div style="background:#2563eb;height:100%;width:{{ProgressPercentage}}%;"></div>
          </div>
          <p style="margin:6px 0 0;font-size:13px;color:#1e40af;">%{{ProgressPercentage}} tamamlandı</p>
        </div>
        
        <div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#fef3c7;border:1px solid #fde047;">
          <p style="margin:0;font-weight:600;color:#92400e;font-size:14px;">🚀 Sıradaki Adım</p>
          <p style="margin:6px 0 0;font-size:14px;color:#78350f;">
            <strong>{{NextStepTitle}}</strong><br/>
            {{NextStepDescription}}
          </p>
        </div>
        
        <p style="margin:0 0 16px;font-size:15px;">
          Detaylı ilerlemenizi portaldan takip edebilirsiniz: 
          <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">Portal'a Git</a>
        </p>
        <p style="margin:0 0 16px;font-size:15px;">
          Sorularınız için <strong>{{SupportEmail}}</strong> adresinden bize ulaşabilirsiniz.
        </p>
        <p style="margin:0;font-weight:600;color:#0f172a;">Worklines Destek Ekibi</p>
      </div>
    </div>
  </div>
</div>
```

### 3.3 Backend Entegrasyon

**Tetikleyici:**
- Service: `ApplicationService.UpdateStepStatusAsync` ve `ApplicationService.UpdateSubStepStatusAsync`
- Endpoint: `PUT /api/v1.0/applications/steps/{id}` ve `PUT /api/v1.0/applications/sub-steps/{id}`
- Koşul: Status → `Completed` değiştiğinde

```csharp
// ApplicationService.cs
public async Task<ApplicationStepDto> UpdateStepStatusAsync(long stepId, StepStatus newStatus)
{
    var step = await _context.ApplicationSteps
        .Include(s => s.Application)
        .ThenInclude(a => a.Client)
        .Include(s => s.Template)
        .FirstOrDefaultAsync(s => s.Id == stepId);
    
    if (step == null) throw new NotFoundException("Step not found");
    
    var oldStatus = step.Status;
    step.Status = newStatus;
    step.UpdatedAt = DateTime.UtcNow;
    
    if (newStatus == StepStatus.Completed && oldStatus != StepStatus.Completed)
    {
        step.CompletionDate = DateTime.UtcNow;
        
        // Email gönderimi
        var client = step.Application.Client;
        var nextStep = await GetNextStepAsync(step.ApplicationId, step.StepOrder);
        
        var emailPlaceholders = new Dictionary<string, string>
        {
            {"ClientName", client.FullName},
            {"StepTitle", step.Title},
            {"SubStepName", ""}, // Main step ise boş
            {"CompletionDate", step.CompletionDate.Value.ToString("dd/MM/yyyy")},
            {"ProgressPercentage", CalculateProgressPercentage(step.ApplicationId).ToString()},
            {"NextStepTitle", nextStep?.Title ?? "Tüm adımlar tamamlandı"},
            {"NextStepDescription", nextStep?.Template.Description_TR ?? "Başvuru süreciniz tamamlanmak üzere."},
            {"PortalLink", $"{_configuration["Portal:BaseUrl"]}/dashboard"},
            {"SupportEmail", _configuration["Support:Email"]}
        };
        
        await _emailService.SendTemplatedEmailAsync(
            "application_progress_updated",
            client.Email,
            emailPlaceholders,
            preferredLanguage: client.PreferredLanguage ?? "tr"
        );
        
        // History kaydet
        await _historyService.LogStepCompletionAsync(stepId, step.Title);
    }
    
    await _context.SaveChangesAsync();
    return MapToDto(step);
}

public async Task<ApplicationSubStepDto> UpdateSubStepStatusAsync(long subStepId, StepStatus newStatus)
{
    var subStep = await _context.ApplicationSubSteps
        .Include(ss => ss.Step)
        .ThenInclude(s => s.Application)
        .ThenInclude(a => a.Client)
        .Include(ss => ss.Template)
        .FirstOrDefaultAsync(ss => ss.Id == subStepId);
    
    if (subStep == null) throw new NotFoundException("SubStep not found");
    
    var oldStatus = subStep.Status;
    subStep.Status = newStatus;
    subStep.UpdatedAt = DateTime.UtcNow;
    
    if (newStatus == StepStatus.Completed && oldStatus != StepStatus.Completed)
    {
        subStep.CompletionDate = DateTime.UtcNow;
        
        // Email gönderimi
        var client = subStep.Step.Application.Client;
        var nextSubStep = await GetNextSubStepAsync(subStep.ApplicationStepId, subStep.SubStepOrder);
        
        var emailPlaceholders = new Dictionary<string, string>
        {
            {"ClientName", client.FullName},
            {"StepTitle", subStep.Step.Title},
            {"SubStepName", subStep.Name},
            {"CompletionDate", subStep.CompletionDate.Value.ToString("dd/MM/yyyy")},
            {"ProgressPercentage", CalculateProgressPercentage(subStep.Step.ApplicationId).ToString()},
            {"NextStepTitle", nextSubStep?.Name ?? subStep.Step.Title},
            {"NextStepDescription", nextSubStep?.Template.Description_TR ?? "Bir sonraki ana adıma geçiliyor."},
            {"PortalLink", $"{_configuration["Portal:BaseUrl"]}/dashboard"},
            {"SupportEmail", _configuration["Support:Email"]}
        };
        
        await _emailService.SendTemplatedEmailAsync(
            "application_progress_updated",
            client.Email,
            emailPlaceholders,
            preferredLanguage: client.PreferredLanguage ?? "tr"
        );
        
        // History kaydet
        await _historyService.LogSubStepCompletionAsync(subStepId, subStep.Name);
    }
    
    await _context.SaveChangesAsync();
    return MapToDto(subStep);
}
```

---

## 4. SQL Seed Script'leri

### 4.1 Hoş Geldin E-postaları

```sql
DECLARE @Now DATETIME2 = SYSUTCDATETIME();

-- Çalışan Formu Hoş Geldin
INSERT INTO wixi_EmailTemplates (
    [Key], DisplayName_TR, DisplayName_EN, DisplayName_DE, DisplayName_AR,
    Subject_TR, Subject_EN, Subject_DE, Subject_AR,
    BodyHtml_TR, BodyHtml_EN, BodyHtml_DE, BodyHtml_AR,
    Description, IsActive, CreatedAt, UpdatedAt, UpdatedBy
)
VALUES
(
    'form_submission_received_employee',
    N'👤 Çalışan Başvurusu Alındı', N'👤 Employee Application Received', N'', N'',
    N'Başvurunuz tarafımıza ulaştı – {{SubmissionDate}}',
    N'We received your application – {{SubmissionDate}}',
    N'', N'',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Hoş geldiniz</span><h2 style="margin:8px 0 0;font-size:24px;">Çalışan Başvurunuz Alındı</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Merhaba <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;">Çalışan formunuz başarıyla alındı. Ekibimiz başvurunuzu inceleyip müşteri kodunuzu ve yüklemeniz gereken belgeleri kısa süre içinde paylaşacak.</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">Sıradaki adımlar</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;">{{NextSteps}}</p></div><p style="margin:0 0 16px;font-size:15px;">Portal bağlantınız: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a></p><p style="margin:0 0 16px;font-size:15px;">Herhangi bir sorunuz için <strong>{{SupportEmail}}</strong> adresinden bize ulaşabilirsiniz.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines Ekibi</p></div></div></div></div>',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Welcome</span><h2 style="margin:8px 0 0;font-size:24px;">Employee Application Received</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Hello <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;">Your employee application has been received. Our team is reviewing it and will share your client code plus required documents shortly.</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">Next steps</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;">{{NextSteps}}</p></div><p style="margin:0 0 16px;font-size:15px;">Portal link: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a></p><p style="margin:0 0 16px;font-size:15px;">If you have any questions, contact us at <strong>{{SupportEmail}}</strong>.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines Team</p></div></div></div></div>',
    N'', N'',
    N'Çalışan formu sonrası otomatik hoş geldin e-postası', 1, @Now, @Now, N'seed'
),
(
    'form_submission_received_employer',
    N'🏢 İşveren Başvurusu Alındı', N'🏢 Employer Application Received', N'', N'',
    N'İşveren başvurunuz alındı – {{SubmissionDate}}',
    N'Your employer request has been received – {{SubmissionDate}}',
    N'', N'',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Yeni işveren talebi</span><h2 style="margin:8px 0 0;font-size:24px;">Başvurunuz Alındı</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Merhaba <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;">İşveren/şirket formunuz bize ulaştı. Başvurunuz incelenip doğrulandığında müşteri kodunuz ve paylaşacağımız belgeler tarafınıza iletilecek.</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">İzlenecek adımlar</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;">{{NextSteps}}</p></div><p style="margin:0 0 16px;font-size:15px;">Portal bağlantınız: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a></p><p style="margin:0 0 16px;font-size:15px;">Sorularınız için <strong>{{SupportEmail}}</strong> üzerinden bize ulaşabilirsiniz.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines İşveren Destek Ekibi</p></div></div></div></div>',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">New employer request</span><h2 style="margin:8px 0 0;font-size:24px;">Application Received</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Hello <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;">Your employer inquiry has been submitted successfully. Once reviewed and verified you will receive your client code and document list.</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">Next steps</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;">{{NextSteps}}</p></div><p style="margin:0 0 16px;font-size:15px;">Portal link: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a></p><p style="margin:0 0 16px;font-size:15px;">Contact <strong>{{SupportEmail}}</strong> for any questions.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines Employer Support</p></div></div></div></div>',
    N'', N'',
    N'İşveren formu sonrası otomatik hoş geldin e-postası', 1, @Now, @Now, N'seed'
);
```

### 4.2 Denklik Belgesi Bildirimi

```sql
INSERT INTO wixi_EmailTemplates (
    [Key], DisplayName_TR, DisplayName_EN, DisplayName_DE, DisplayName_AR,
    Subject_TR, Subject_EN, Subject_DE, Subject_AR,
    BodyHtml_TR, BodyHtml_EN, BodyHtml_DE, BodyHtml_AR,
    Description, IsActive, CreatedAt, UpdatedAt, UpdatedBy
)
VALUES
(
    'equivalency_document_uploaded',
    N'📄 Denklik Belgesi Yüklendi', N'📄 Equivalency Document Uploaded', N'', N'',
    N'Denklik Belgeniz Hazır – {{UploadedAt}}',
    N'Your Equivalency Document is Ready – {{UploadedAt}}',
    N'', N'',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Belge Yüklendi</span><h2 style="margin:8px 0 0;font-size:24px;">Denklik Belgeniz Hazır</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Merhaba <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;"><strong>{{DocumentName}}</strong> belgeniz tarafımızca sisteme yüklenmiştir.</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">📄 Belge Bilgileri</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;"><strong>Yükleme Tarihi:</strong> {{UploadedAt}}<br/><strong>Belge Türü:</strong> Denklik Belgesi</p></div><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#fef3c7;border:1px solid #fde047;"><p style="margin:0;font-weight:600;color:#92400e;font-size:14px;">Sıradaki Adımlar</p><p style="margin:6px 0 0;font-size:14px;color:#78350f;">{{NextSteps}}</p></div><p style="margin:0 0 16px;font-size:15px;">Belgeyi portaldan görüntüleyebilir ve indirebilirsiniz: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a></p><p style="margin:0 0 16px;font-size:15px;">Sorularınız için <strong>{{SupportEmail}}</strong> adresinden bize ulaşabilirsiniz.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines Belge Destek Ekibi</p></div></div></div></div>',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Document uploaded</span><h2 style="margin:8px 0 0;font-size:24px;">Equivalency Document Ready</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Hello <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;"><strong>{{DocumentName}}</strong> has been uploaded to your account.</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">📄 Document Info</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;"><strong>Upload Date:</strong> {{UploadedAt}}<br/><strong>Document Type:</strong> Equivalency Certificate</p></div><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#fef3c7;border:1px solid #fde047;"><p style="margin:0;font-weight:600;color:#92400e;font-size:14px;">Next Steps</p><p style="margin:6px 0 0;font-size:14px;color:#78350f;">{{NextSteps}}</p></div><p style="margin:0 0 16px;font-size:15px;">View and download from portal: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">{{PortalLink}}</a></p><p style="margin:0 0 16px;font-size:15px;">Contact <strong>{{SupportEmail}}</strong> for questions.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines Document Team</p></div></div></div></div>',
    N'', N'',
    N'Admin denklik belgesi yüklediğinde müşteriye bildirim', 1, @Now, @Now, N'seed'
);
```

### 4.3 İlerleme Bildirimi (Generic)

```sql
INSERT INTO wixi_EmailTemplates (
    [Key], DisplayName_TR, DisplayName_EN, DisplayName_DE, DisplayName_AR,
    Subject_TR, Subject_EN, Subject_DE, Subject_AR,
    BodyHtml_TR, BodyHtml_EN, BodyHtml_DE, BodyHtml_AR,
    Description, IsActive, CreatedAt, UpdatedAt, UpdatedBy
)
VALUES
(
    'application_progress_updated',
    N'📊 Başvuru İlerlemesi', N'📊 Application Progress', N'', N'',
    N'İlerleme Kaydedildi: {{StepTitle}} – {{SubStepName}}',
    N'Progress Updated: {{StepTitle}} – {{SubStepName}}',
    N'', N'',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Başvuru İlerlemesi</span><h2 style="margin:8px 0 0;font-size:24px;">Yeni Adım Tamamlandı</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Merhaba <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;">Başvuru sürecinizde yeni bir adım tamamlandı! 🎉</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">✅ Tamamlanan Adım</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;"><strong>Ana Adım:</strong> {{StepTitle}}<br/><strong>Alt Adım:</strong> {{SubStepName}}<br/><strong>Tamamlanma:</strong> {{CompletionDate}}</p></div><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#eff6ff;border:1px solid #bfdbfe;"><p style="margin:0;font-weight:600;color:#1e40af;font-size:14px;">📊 İlerleme Durumu</p><div style="margin:8px 0 0;background:#dbeafe;border-radius:999px;height:12px;overflow:hidden;"><div style="background:#2563eb;height:100%;width:{{ProgressPercentage}}%;"></div></div><p style="margin:6px 0 0;font-size:13px;color:#1e40af;">%{{ProgressPercentage}} tamamlandı</p></div><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#fef3c7;border:1px solid #fde047;"><p style="margin:0;font-weight:600;color:#92400e;font-size:14px;">🚀 Sıradaki Adım</p><p style="margin:6px 0 0;font-size:14px;color:#78350f;"><strong>{{NextStepTitle}}</strong><br/>{{NextStepDescription}}</p></div><p style="margin:0 0 16px;font-size:15px;">Detaylı ilerlemenizi portaldan takip edebilirsiniz: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">Portal''a Git</a></p><p style="margin:0 0 16px;font-size:15px;">Sorularınız için <strong>{{SupportEmail}}</strong> adresinden bize ulaşabilirsiniz.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines Destek Ekibi</p></div></div></div></div>',
    N'<div style="max-width:640px;margin:0 auto;font-family:''Inter'',Arial,sans-serif;line-height:1.7;color:#0f172a;background:#f3f4f6;padding:24px;"><div style="background:#fff;border-radius:16px;box-shadow:0 20px 40px rgba(15,23,42,0.12);overflow:hidden;"><div style="background:linear-gradient(135deg,#0f766e,#10b981);color:#fff;padding:28px;"><span style="font-size:13px;letter-spacing:2px;text-transform:uppercase;opacity:.8;">Application Progress</span><h2 style="margin:8px 0 0;font-size:24px;">Step Completed</h2></div><div style="padding:32px;background:#f9fafb;"><div style="background:#fff;border-radius:14px;padding:24px;border:1px solid #e2e8f0;"><p style="margin:0 0 16px;font-size:15px;">Hello <strong>{{ClientName}}</strong>,</p><p style="margin:0 0 16px;font-size:15px;">A new step in your application process has been completed! 🎉</p><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#ecfdf5;border:1px solid #d1fae5;"><p style="margin:0;font-weight:600;color:#047857;font-size:14px;">✅ Completed Step</p><p style="margin:6px 0 0;font-size:14px;color:#065f46;"><strong>Main Step:</strong> {{StepTitle}}<br/><strong>Sub-step:</strong> {{SubStepName}}<br/><strong>Completion:</strong> {{CompletionDate}}</p></div><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#eff6ff;border:1px solid #bfdbfe;"><p style="margin:0;font-weight:600;color:#1e40af;font-size:14px;">📊 Progress Status</p><div style="margin:8px 0 0;background:#dbeafe;border-radius:999px;height:12px;overflow:hidden;"><div style="background:#2563eb;height:100%;width:{{ProgressPercentage}}%;"></div></div><p style="margin:6px 0 0;font-size:13px;color:#1e40af;">{{ProgressPercentage}}% completed</p></div><div style="margin:0 0 18px;padding:16px;border-radius:12px;background:#fef3c7;border:1px solid #fde047;"><p style="margin:0;font-weight:600;color:#92400e;font-size:14px;">🚀 Next Step</p><p style="margin:6px 0 0;font-size:14px;color:#78350f;"><strong>{{NextStepTitle}}</strong><br/>{{NextStepDescription}}</p></div><p style="margin:0 0 16px;font-size:15px;">Track your progress in the portal: <a href="{{PortalLink}}" style="color:#0f766e;font-weight:600;text-decoration:none;">Go to Portal</a></p><p style="margin:0 0 16px;font-size:15px;">Contact <strong>{{SupportEmail}}</strong> for questions.</p><p style="margin:0;font-weight:600;color:#0f172a;">Worklines Support Team</p></div></div></div></div>',
    N'', N'',
    N'Step veya substep tamamlandığında müşteriye ilerleme bildirimi', 1, @Now, @Now, N'seed'
);
```

---

## 5. Uygulama Planı

### 5.1 Öncelik Sırası

1. **Görev 1: Hoş Geldin E-postaları** ✅  
   - SQL seed script'lerini çalıştır
   - `FormService.cs` içinde email trigger'ları ekle
   - Test et (employee ve employer form submit)

2. **Görev 2: Denklik Belgesi Bildirimi** 🔄  
   - SQL seed script'ini çalıştır
   - `DocumentService.cs` veya admin upload controller'a email trigger ekle
   - Step/SubStep context kontrolü ekle (1.Step, 4.SubStep = denklik)
   - Test et

3. **Görev 3: Step/SubStep İlerleme Bildirimleri** 🔄  
   - SQL seed script'ini çalıştır
   - `ApplicationService.UpdateStepStatusAsync` ve `UpdateSubStepStatusAsync` metotlarına email tetikleyicileri ekle
   - Progress percentage hesaplama fonksiyonu ekle
   - Next step/substep bulma fonksiyonlarını ekle
   - Test et (manuel step update ile)

4. **Görev 4: Test & QA** 📋  
   - Unit test'ler yaz (email service mock ile)
   - Integration test'ler ekle
   - Staging/QA ortamında end-to-end test
   - Email template'lerini gözden geçir

### 5.2 Eksik Gereksinimler

**Configuration (`appsettings.json`):**
```json
{
  "Support": {
    "Email": "support@worklines.de"
  },
  "Portal": {
    "BaseUrl": "https://portal.worklines.de"
  }
}
```

**Email Service Interface Eklentisi:**
```csharp
public interface IEmailService
{
    Task SendTemplatedEmailAsync(
        string templateKey,
        string toEmail,
        Dictionary<string, string> placeholders,
        string preferredLanguage = "tr"
    );
}
```

**History Service (Opsiyonel ama önerilen):**
```csharp
public interface IApplicationHistoryService
{
    Task LogStepCompletionAsync(long stepId, string stepTitle);
    Task LogSubStepCompletionAsync(long subStepId, string subStepName);
}
```

---

## 6. Notlar ve Ek Hususlar

### 6.1 Placeholder Değiştirme Stratejisi

Email service içinde placeholder replacement için helper metot:

```csharp
private string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
{
    foreach (var kvp in placeholders)
    {
        template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
    }
    return template;
}
```

### 6.2 Dil Desteği

- Client'ın tercih ettiği dile göre dinamik template seçimi
- Fallback: TR → EN → DE → AR sırası
- `preferredLanguage` parametresi her email trigger'da kullanılmalı

### 6.3 Email Log

- Her email gönderiminde `wixi_EmailLogs` tablosuna kayıt
- Başarı/başarısızlık durumu takibi
- Template key, correlation ID ile audit trail

### 6.4 Rate Limiting

- Aynı kullanıcıya çok sık email gitmemesi için throttle
- Örn: Aynı template için 5 dakikada 1 email limiti

### 6.5 Async/Background Processing

- Email gönderimlerini background job'a (Hangfire/Quartz) taşımayı düşün
- API response süresini düşürmek için
- Retry mekanizması için

---

## 7. Checklist

### SQL Template Seeding
- [ ] `form_submission_received_employee` template eklendi
- [ ] `form_submission_received_employer` template eklendi
- [ ] `equivalency_document_uploaded` template eklendi
- [ ] `application_progress_updated` template eklendi
- [ ] Database'de template'ler doğrulandı

### Backend Implementation
- [ ] `FormService.SubmitEmployeeFormAsync` - email trigger eklendi
- [ ] `FormService.SubmitEmployerFormAsync` - email trigger eklendi
- [ ] `DocumentService.UploadDocumentAsync` - denklik check + email trigger
- [ ] `ApplicationService.UpdateStepStatusAsync` - completion email trigger
- [ ] `ApplicationService.UpdateSubStepStatusAsync` - completion email trigger
- [ ] Configuration (Support Email, Portal URL) eklendi
- [ ] Email service interface genişletildi
- [ ] History logging eklendi (opsiyonel)

### Testing
- [ ] Unit test: Form submission email triggers
- [ ] Unit test: Denklik upload email trigger
- [ ] Unit test: Step/substep completion email triggers
- [ ] Integration test: End-to-end email flow
- [ ] QA: Email template görünüm testleri
- [ ] QA: Placeholder doğru doldurma testleri
- [ ] Production smoke test

---

**Son Güncelleme:** 2025-11-21  
**Durum:** 🚧 Planlama Tamamlandı - Implementation Bekliyor
