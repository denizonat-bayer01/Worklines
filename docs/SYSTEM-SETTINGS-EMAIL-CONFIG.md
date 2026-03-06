# System Settings - Email Configuration

## Overview

Portal URL ve Support Email bilgileri artık `SystemSettings` tablosunda merkezi olarak tutulmaktadır. Bu ayarlar email template'lerinde `{{PortalLink}}` ve `{{SupportEmail}}` placeholder'ları için kullanılır.

## Database Changes

### Migration

```bash
dotnet ef migrations add AddEmailSettingsToSystemSettings --project wixi.DataAccess --startup-project wixi.WebAPI
dotnet ef database update --project wixi.DataAccess --startup-project wixi.WebAPI
```

### SQL Script

`wixi.DataAccess/Scripts/UpdateSystemSettingsEmailFields.sql` dosyasını çalıştırın:

```sql
-- Update or insert SystemSettings with email fields
UPDATE wixi_SystemSettings
SET 
    PortalUrl = 'https://portal.worklines.de',
    SupportEmail = 'support@worklines.de',
    UpdatedAt = GETUTCDATE()
WHERE Id = 1;
```

## SystemSettings Entity

```csharp
public class SystemSettings
{
    public int Id { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SiteUrl { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    
    // Email Configuration
    public string PortalUrl { get; set; } = "https://portal.worklines.de";
    public string SupportEmail { get; set; } = "support@worklines.de";
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public byte[]? RowVersion { get; set; }
}
```

## API Endpoints

### Get System Settings

```http
GET /api/v1.0/admin/settings
Authorization: Bearer {token}
```

**Response:**
```json
{
  "id": 1,
  "siteName": "Worklines",
  "siteUrl": "https://worklines.de",
  "adminEmail": "admin@worklines.de",
  "portalUrl": "https://portal.worklines.de",
  "supportEmail": "support@worklines.de"
}
```

### Update System Settings

```http
PUT /api/v1.0/admin/settings
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "siteName": "Worklines",
  "siteUrl": "https://worklines.de",
  "adminEmail": "admin@worklines.de",
  "portalUrl": "https://portal.worklines.de",
  "supportEmail": "support@worklines.de"
}
```

## Usage in Email Templates

### FormService Integration

`FormService` artık email gönderirken `SystemSettings`'ten Portal URL ve Support Email bilgilerini alır:

```csharp
// Get PortalLink and SupportEmail from SystemSettings
var systemSettings = await _contentService?.GetSystemSettingsAsync()!;
var portalLink = systemSettings?.PortalUrl ?? _configuration["Portal:BaseUrl"] ?? "https://portal.worklines.de";
var supportEmail = systemSettings?.SupportEmail ?? _configuration["Support:Email"] ?? "support@worklines.de";

emailBody = emailBody
    .Replace("{{PortalLink}}", portalLink)
    .Replace("{{SupportEmail}}", supportEmail);
```

### Fallback Order

1. **SystemSettings** (Database) - Öncelik
2. **appsettings.json** - Yedek
3. **Hard-coded** - Son çare

## Email Template Placeholders

Email template'lerinde kullanılabilir:

- `{{PortalLink}}` → `https://portal.worklines.de`
- `{{SupportEmail}}` → `support@worklines.de`
- `{{ClientName}}` → Müşteri/Başvuran adı
- `{{SubmissionDate}}` → Başvuru tarihi
- `{{NextSteps}}` → Sonraki adımlar

## Admin Panel Configuration

Admin panel'den (`/admin/settings`) bu ayarlar güncellenebilir:

1. **System Settings** sayfasına gidin
2. **Portal URL** ve **Support Email** alanlarını düzenleyin
3. **Save** butonuna tıklayın
4. Değişiklikler 5 dakika cache'lenir

## Benefits

✅ **Merkezi Yönetim**: Tüm email ayarları tek bir yerden yönetilir  
✅ **Kolay Güncelleme**: Admin panel üzerinden anında değiştirilebilir  
✅ **Cache Support**: Performans için 5 dakika cache'lenir  
✅ **Fallback Mechanism**: Database erişilemezse appsettings kullanılır  
✅ **Version Control**: UpdatedAt ve UpdatedBy ile değişiklik takibi  

## Migration History

- **20251122132807_AddEmailSettingsToSystemSettings**: PortalUrl ve SupportEmail alanları eklendi

## Related Files

- `wixi.Content/Entities/SystemSettings.cs` - Entity definition
- `wixi.Content/DTOs/SystemSettingsDto.cs` - DTO
- `wixi.WebAPI/Services/ContentService.cs` - Business logic
- `wixi.WebAPI/Services/FormService.cs` - Email sending
- `wixi.WebAPI/Controllers/AdminSettingsController.cs` - API endpoint
- `wixi.DataAccess/Scripts/UpdateSystemSettingsEmailFields.sql` - Initial data

## Testing

Test email gönderimi için:

```http
POST /api/v1.0/EmailTest/send
Content-Type: application/json
```

```json
{
  "templateKey": "form_submission_received_employer",
  "toEmail": "test@example.com",
  "language": "tr"
}
```

Email'de `{{PortalLink}}` ve `{{SupportEmail}}` placeholder'larının doğru değiştirildiğini kontrol edin.

