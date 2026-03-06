# Admin Panel - Email Settings Integration

## Overview

Portal URL ve Support Email ayarları artık Admin Panel'den yönetilebilmektedir.

## Frontend Changes

### Settings Page (`V4/src/components/admin/Settings.tsx`)

**"Genel Ayarlar" (General) Tab'ına Yeni Alanlar Eklendi:**

```tsx
// E-posta Template Ayarları Section
<div className="pt-4 border-t border-gray-200 dark:border-gray-700">
  <h3 className="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-4">
    E-posta Template Ayarları
  </h3>
  
  <div className="space-y-4">
    {/* Portal URL */}
    <div>
      <label>Portal URL</label>
      <input 
        type="url"
        value={settings.portalUrl}
        placeholder="https://portal.worklines.de"
      />
      <p className="text-xs text-gray-500">
        E-posta template'lerinde {{PortalLink}} placeholder'ı için kullanılır
      </p>
    </div>

    {/* Support Email */}
    <div>
      <label>Destek E-posta</label>
      <input 
        type="email"
        value={settings.supportEmail}
        placeholder="support@worklines.de"
      />
      <p className="text-xs text-gray-500">
        E-posta template'lerinde {{SupportEmail}} placeholder'ı için kullanılır
      </p>
    </div>
  </div>
</div>
```

### Service Interface (`V4/src/ApiServices/services/SettingsService.ts`)

```typescript
export interface SystemSettings {
  id?: number;
  siteName: string;
  siteUrl: string;
  adminEmail: string;
  portalUrl?: string;        // ✅ Yeni
  supportEmail?: string;     // ✅ Yeni
  updatedAt?: string;
  updatedBy?: string;
}
```

## Backend Support

### API Endpoint

```
PUT /api/v1.0/admin/settings
Authorization: Bearer {token}
Content-Type: application/json

{
  "siteName": "Worklines",
  "siteUrl": "https://worklines.de",
  "adminEmail": "admin@worklines.de",
  "portalUrl": "https://portal.worklines.de",
  "supportEmail": "support@worklines.de"
}
```

## Database Setup

### 1. Migration Uygulandı

```bash
dotnet ef database update --project wixi.DataAccess --startup-project wixi.WebAPI
```

### 2. Initial Data SQL Script

`wixi.backendV2/wixi.DataAccess/Scripts/UpdateSystemSettingsEmailFields.sql` dosyasını çalıştırın:

```sql
-- Update or Insert SystemSettings with default email values
IF EXISTS (SELECT 1 FROM wixi_SystemSettings WHERE Id = 1)
BEGIN
    UPDATE wixi_SystemSettings
    SET 
        PortalUrl = 'https://portal.worklines.de',
        SupportEmail = 'support@worklines.de',
        UpdatedAt = GETUTCDATE()
    WHERE Id = 1;
    
    PRINT 'SystemSettings updated successfully.';
END
ELSE
BEGIN
    INSERT INTO wixi_SystemSettings (
        SiteName, 
        SiteUrl, 
        AdminEmail, 
        PortalUrl, 
        SupportEmail, 
        UpdatedAt
    )
    VALUES (
        'Worklines',
        'https://worklines.de',
        'admin@worklines.de',
        'https://portal.worklines.de',
        'support@worklines.de',
        GETUTCDATE()
    );
    
    PRINT 'SystemSettings record created successfully.';
END
```

## Usage in Admin Panel

### Navigation

1. Admin Panel'e giriş yapın: `https://test.worklines.com.tr/admin`
2. Sol menüden **"Settings"** (Ayarlar) sayfasına gidin
3. **"Genel"** (General) tab'ında "E-posta Template Ayarları" bölümünü bulun

### Editing Values

1. **Portal URL**: E-posta'larda kullanıcılara gönderilen portal linki
   - Örnek: `https://portal.worklines.de`
   - Email'de `{{PortalLink}}` olarak görünür

2. **Destek E-posta**: Müşterilerin iletişim kurabileceği destek e-postası
   - Örnek: `support@worklines.de`
   - Email'de `{{SupportEmail}}` olarak görünür

3. **Kaydet**: Değişiklikleri kaydetmek için sayfanın altındaki "Değişiklikleri Kaydet" butonuna tıklayın

### Visual Design

```
┌─────────────────────────────────────────────────────────┐
│ Genel Ayarlar                                           │
├─────────────────────────────────────────────────────────┤
│ Site Adı                                                │
│ [Worklines ProConsulting                             ]  │
│                                                         │
│ Site URL                                                │
│ [https://worklines.de                                ]  │
│                                                         │
│ Admin E-posta                                           │
│ [admin@worklines.de                                  ]  │
├─────────────────────────────────────────────────────────┤
│ E-posta Template Ayarları                               │
│                                                         │
│ Portal URL                                              │
│ [https://portal.worklines.de                         ]  │
│ E-posta template'lerinde {{PortalLink}} için kullanılır│
│                                                         │
│ Destek E-posta                                          │
│ [support@worklines.de                                ]  │
│ E-posta template'lerinde {{SupportEmail}} için kullanılır│
└─────────────────────────────────────────────────────────┘
                      [Değişiklikleri Kaydet]
```

## How It Works

### Flow Diagram

```
┌─────────────────┐
│  Admin Panel    │
│  (Settings)     │
└────────┬────────┘
         │ PUT /api/v1.0/admin/settings
         ↓
┌─────────────────────────────┐
│  AdminSettingsController    │
│  - UpdateSettings()         │
└────────┬────────────────────┘
         │
         ↓
┌─────────────────────────────┐
│  ContentService             │
│  - UpsertSystemSettings()   │
└────────┬────────────────────┘
         │
         ↓
┌─────────────────────────────┐
│  Database                   │
│  wixi_SystemSettings        │
│  - PortalUrl                │
│  - SupportEmail             │
└────────┬────────────────────┘
         │
         ↓ (Cached for 5 minutes)
         │
┌─────────────────────────────┐
│  FormService                │
│  - SendEmployerForm...()    │
│  - SendEmployeeForm...()    │
└────────┬────────────────────┘
         │
         ↓
┌─────────────────────────────┐
│  Email Templates            │
│  Replace:                   │
│  - {{PortalLink}}           │
│  - {{SupportEmail}}         │
└─────────────────────────────┘
```

## Cache Behavior

- **Cache Duration**: 5 minutes
- **Cache Key**: `system_settings_cache`
- **Auto-Invalidation**: Cache cleared on settings update
- **Location**: `ContentService.GetSystemSettingsAsync()`

## Testing

### 1. Update Settings via Admin Panel

```
1. Navigate to: https://test.worklines.com.tr/admin/settings
2. Change "Portal URL" to: https://test-portal.worklines.de
3. Change "Destek E-posta" to: test-support@worklines.de
4. Click "Değişiklikleri Kaydet"
5. Verify success toast message
```

### 2. Test Email Sending

```bash
curl -X POST https://api.worklines.de/api/v1.0/EmailTest/send \
  -H "Content-Type: application/json" \
  -d '{
    "templateKey": "form_submission_received_employer",
    "toEmail": "your-email@example.com",
    "language": "tr"
  }'
```

### 3. Verify Email Content

Check received email for:
- ✅ Portal link matches your updated value
- ✅ Support email matches your updated value

## Benefits

✅ **Centralized Management**: All email settings in one place  
✅ **No Code Changes**: Update URLs without redeployment  
✅ **User-Friendly**: Non-technical admins can update values  
✅ **Real-Time**: Changes reflected immediately (after cache expiry)  
✅ **Multi-Language**: Works for all email template languages  
✅ **Fallback Support**: Uses appsettings.json if database unavailable  

## Related Documentation

- [System Settings Email Configuration](./SYSTEM-SETTINGS-EMAIL-CONFIG.md)
- [Email Test API](./EMAIL-TEST-API.md)
- [Email Template Logo Integration](./EMAIL-TEMPLATE-LOGO-INTEGRATION.md)

## Troubleshooting

### Issue: Changes not reflecting in emails

**Solution**:
1. Wait 5 minutes for cache to expire
2. Or restart backend: `docker-compose restart wixi-api-v2-dev`

### Issue: "Settings not found" error

**Solution**:
1. Run SQL script: `UpdateSystemSettingsEmailFields.sql`
2. Verify database connection

### Issue: Frontend shows default values

**Solution**:
1. Check API endpoint: `GET /api/v1.0/admin/settings`
2. Verify authentication token
3. Check browser console for errors

## Screenshots

### Admin Panel - Settings Page

```
┌──────────────────────────────────────────────────────────────┐
│ ← Settings                                          👤 Admin  │
├──────────────────────────────────────────────────────────────┤
│ ┌────────┐                                                   │
│ │ Genel  │  İçerik  Bildirimler  Güvenlik  ...              │
│ └────────┘                                                   │
│                                                               │
│  Genel Ayarlar                                               │
│  ─────────────────────────────────────────────────────       │
│                                                               │
│  Site Adı                                                    │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Worklines ProConsulting                              │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                               │
│  E-posta Template Ayarları                                   │
│  ─────────────────────────────────────────────────────       │
│                                                               │
│  Portal URL                                                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ https://portal.worklines.de                          │   │
│  └──────────────────────────────────────────────────────┘   │
│  E-posta template'lerinde {{PortalLink}} için kullanılır    │
│                                                               │
│  Destek E-posta                                              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ support@worklines.de                                 │   │
│  └──────────────────────────────────────────────────────┘   │
│  E-posta template'lerinde {{SupportEmail}} için kullanılır  │
│                                                               │
│                            ┌──────────────────────────┐      │
│                            │ Değişiklikleri Kaydet 💾 │      │
│                            └──────────────────────────┘      │
└──────────────────────────────────────────────────────────────┘
```

## Version History

- **v1.0** (2025-11-22): Initial implementation
  - Added PortalUrl and SupportEmail fields
  - Admin Panel UI integration
  - Backend API support
  - Database migration

