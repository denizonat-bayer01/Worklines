# ⚡ Hızlı Başlangıç: Deployment ve Erişim Rehberi

**Pratik Senaryolar ve Adım Adım Kurulum**

---

## 🎯 Senaryo Seçimi

Hangi senaryo size uygun?

1. **Küçük Firma (SaaS)** → [Senaryo 1](#senaryo-1-saas-küçük-firma)
2. **Büyük Firma (On-Premise)** → [Senaryo 2](#senaryo-2-on-premise-büyük-firma)
3. **Kurumsal (Hybrid)** → [Senaryo 3](#senaryo-3-hybrid-kurumsal)

---

## 📋 Senaryo 1: SaaS (Küçük Firma)

### Durum
- ✅ 10-50 kullanıcı
- ✅ Standart modüller yeterli
- ✅ Hızlı kurulum isteniyor
- ✅ İnternet bağlantısı mevcut

### Adımlar

#### 1. Müşteri Kaydı
```bash
# API'ye tenant oluşturma isteği
POST https://api.worklines.de/api/admin/tenants
{
  "customerCode": "ABC_001",
  "name": "ABC Consulting GmbH",
  "licenseType": "Basic",
  "modules": ["Identity", "Clients", "Documents", "Support", "Content", "Forms", "Email"]
}
```

#### 2. Database Oluşturma
```sql
-- Master DB'de tenant kaydı
INSERT INTO Tenants (CustomerCode, Name, ConnectionString, IsActive)
VALUES ('ABC_001', 'ABC Consulting GmbH', 'Server=...;Database=DB_ABC_001;...', 1);

-- Tenant database oluştur
CREATE DATABASE DB_ABC_001;
```

#### 3. Frontend Kurulumu (Müşteri Makinesi)
```bash
# 1. Frontend build'i indir
wget https://releases.worklines.de/frontend/latest.zip

# 2. Extract et
unzip latest.zip -d C:\Worklines

# 3. Config dosyasını düzenle
# C:\Worklines\config.json
{
  "apiUrl": "https://api.worklines.de",
  "tenantCode": "ABC_001"
}

# 4. Launcher'ı çalıştır
C:\Worklines\Worklines.exe
```

#### 4. İlk Giriş
- Frontend açılır
- Login sayfası görünür
- Admin kullanıcı oluşturulur
- Sistem kullanıma hazır ✅

---

## 📋 Senaryo 2: On-Premise (Büyük Firma)

### Durum
- ✅ 100+ kullanıcı
- ✅ Yüksek güvenlik gereksinimleri
- ✅ Veri lokal olmalı
- ✅ Özel modül kombinasyonu

### Adımlar

#### 1. Sunucu Hazırlığı
```bash
# Windows Server 2019/2022
# SQL Server 2019+ kurulu olmalı
# .NET 8 Runtime kurulu olmalı
```

#### 2. API Kurulumu
```bash
# 1. Deployment package'i indir
wget https://releases.worklines.de/onpremise/latest.zip

# 2. Extract et
unzip latest.zip -d C:\Worklines\API

# 3. appsettings.json düzenle
# C:\Worklines\API\appsettings.json
{
  "Deployment": {
    "Mode": "OnPremise",
    "ApiBaseUrl": "https://internal.xyz.local:5001"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DB_XYZ_001;..."
  },
  "Modules": {
    "Identity": { "Enabled": true },
    "Clients": { "Enabled": true, "LicenseKey": "CLIENTS-XXXX-XXXX" },
    "Documents": { "Enabled": true, "LicenseKey": "DOCS-XXXX-XXXX" },
    "Applications": { "Enabled": true, "LicenseKey": "APPS-XXXX-XXXX" },
    "Support": { "Enabled": true, "LicenseKey": "SUPPORT-XXXX-XXXX" },
    "Payments": { "Enabled": true, "LicenseKey": "PAYMENTS-XXXX-XXXX" }
  },
  "License": {
    "ServerUrl": "https://license.wixisoftware.com",
    "OfflineMode": true,
    "ValidationInterval": "24:00:00"
  }
}

# 4. Windows Service olarak kur
sc create WorklinesAPI binPath="C:\Worklines\API\wixi.WebAPI.exe"
sc start WorklinesAPI
```

#### 3. Database Kurulumu
```sql
-- 1. Database oluştur
CREATE DATABASE DB_XYZ_001;

-- 2. Migration çalıştır
-- API'den migration endpoint'i çağır
POST https://internal.xyz.local:5001/api/admin/database/migrate
Authorization: Bearer {admin-token}
```

#### 4. Frontend Kurulumu
```bash
# 1. Frontend build'i indir
wget https://releases.worklines.de/frontend/latest.zip

# 2. Extract et
unzip latest.zip -d C:\Worklines\Frontend

# 3. Config düzenle
# C:\Worklines\Frontend\config.json
{
  "apiUrl": "https://internal.xyz.local:5001",
  "tenantCode": "XYZ_001",
  "vpnRequired": false
}

# 4. Launcher'ı çalıştır
C:\Worklines\Frontend\Worklines.exe
```

#### 5. VPN Yapılandırması (Opsiyonel)
```bash
# OpenVPN kurulumu (gerekirse)
# VPN client config
client
dev tun
proto udp
remote vpn.xyz.local 1194
cert client.crt
key client.key
ca ca.crt
```

---

## 📋 Senaryo 3: Hybrid (Kurumsal)

### Durum
- ✅ 500+ kullanıcı
- ✅ Karma güvenlik gereksinimleri
- ✅ Public + Private modüller
- ✅ API Gateway kullanımı

### Adımlar

#### 1. Cloud API Yapılandırması
```json
// Cloud API appsettings.json
{
  "Deployment": {
    "Mode": "SaaS",
    "ApiBaseUrl": "https://api.worklines.de"
  },
  "Modules": {
    "Identity": { "Enabled": true, "Location": "Cloud" },
    "Content": { "Enabled": true, "Location": "Cloud" },
    "Forms": { "Enabled": true, "Location": "Cloud" }
  }
}
```

#### 2. On-Premise API Yapılandırması
```json
// On-Premise API appsettings.json
{
  "Deployment": {
    "Mode": "OnPremise",
    "ApiBaseUrl": "https://internal.def.local:5001"
  },
  "Modules": {
    "Clients": { "Enabled": true, "Location": "OnPremise" },
    "Documents": { "Enabled": true, "Location": "OnPremise" },
    "Applications": { "Enabled": true, "Location": "OnPremise" },
    "Payments": { "Enabled": true, "Location": "OnPremise" }
  }
}
```

#### 3. API Gateway Yapılandırması
```yaml
# Traefik veya Nginx config
routes:
  - match: Host(`gateway.worklines.de`) && PathPrefix(`/api/identity`)
    service: cloud-api
  - match: Host(`gateway.worklines.de`) && PathPrefix(`/api/content`)
    service: cloud-api
  - match: Host(`gateway.worklines.de`) && PathPrefix(`/api/clients`)
    service: onpremise-api
  - match: Host(`gateway.worklines.de`) && PathPrefix(`/api/documents`)
    service: onpremise-api
```

#### 4. Frontend Yapılandırması
```json
// Frontend config.json
{
  "apiGatewayUrl": "https://gateway.worklines.de",
  "tenantCode": "DEF_001",
  "modules": {
    "cloud": ["Identity", "Content", "Forms"],
    "onpremise": ["Clients", "Documents", "Applications", "Payments"]
  }
}
```

---

## 🔧 Modül Aktivasyon/Deaktivasyon

### API Üzerinden

```bash
# Modül aktif et
POST https://api.worklines.de/api/admin/tenants/ABC_001/modules/activate
Authorization: Bearer {admin-token}
{
  "moduleName": "Payments",
  "licenseKey": "PAYMENTS-XXXX-XXXX"
}

# Modül pasif et
POST https://api.worklines.de/api/admin/tenants/ABC_001/modules/deactivate
Authorization: Bearer {admin-token}
{
  "moduleName": "Payments"
}

# Aktif modülleri listele
GET https://api.worklines.de/api/admin/tenants/ABC_001/modules
Authorization: Bearer {admin-token}
```

### Config Dosyası Üzerinden

```json
// appsettings.json
{
  "Modules": {
    "Payments": {
      "Enabled": true,
      "LicenseKey": "PAYMENTS-XXXX-XXXX",
      "LicenseRequired": "Enterprise"
    }
  }
}

// Değişiklik sonrası API'yi restart et
```

---

## 🌐 Uzaktan Erişim Yapılandırmaları

### 1. Direct API (HTTPS)

```json
// Frontend config.json
{
  "remoteAccess": {
    "mode": "DirectApi",
    "apiUrl": "https://api.worklines.de",
    "protocol": "HTTPS",
    "authentication": "JWT"
  }
}
```

**Güvenlik:**
- ✅ TLS 1.3
- ✅ JWT token
- ✅ CORS policy
- ✅ Rate limiting

---

### 2. VPN Tunneling

```json
// Frontend config.json
{
  "remoteAccess": {
    "mode": "VPN",
    "vpnType": "OpenVPN",
    "vpnServer": "vpn.worklines.local",
    "vpnPort": 1194,
    "certificatePath": "C:\\Worklines\\certs\\client.crt",
    "apiUrl": "https://internal.worklines.local:5001"
  }
}
```

**Kurulum:**
```bash
# OpenVPN client kur
# Certificates yerleştir
# VPN bağlantısını test et
```

---

### 3. API Gateway (mTLS)

```json
// Frontend config.json
{
  "remoteAccess": {
    "mode": "ApiGateway_mTLS",
    "gatewayUrl": "https://gateway.worklines.de",
    "clientCertificate": {
      "path": "C:\\Worklines\\certs\\client.pfx",
      "password": "${CERT_PASSWORD}"
    },
    "certificatePinning": {
      "enabled": true,
      "publicKeyHash": "sha256/..."
    }
  }
}
```

---

## 🔐 License Yönetimi

### License Key Alma

```bash
# 1. License portal'a giriş
https://license.wixisoftware.com

# 2. Tenant seç
# 3. Modül seç
# 4. License key oluştur
# 5. Key'i kopyala
```

### License Validation

```bash
# Online validation
POST https://license.wixisoftware.com/api/validate
{
  "tenantCode": "ABC_001",
  "moduleName": "Payments",
  "licenseKey": "PAYMENTS-XXXX-XXXX"
}

# Offline validation (on-premise)
# License key'i config'e ekle
# API her 24 saatte bir validate eder
```

---

## 📊 Monitoring ve Health Checks

### API Health Check

```bash
# Genel health check
GET https://api.worklines.de/health

# Tenant bazlı health check
GET https://api.worklines.de/health/tenant/ABC_001

# Modül bazlı health check
GET https://api.worklines.de/health/modules
```

### Frontend Health Check

```typescript
// Frontend'den API connectivity test
async function checkApiHealth(): Promise<boolean> {
  try {
    const response = await fetch(`${apiUrl}/health`);
    return response.ok;
  } catch {
    return false;
  }
}
```

---

## 🚨 Troubleshooting

### Problem: Frontend API'ye bağlanamıyor

**Çözüm:**
1. API URL'i kontrol et (`config.json`)
2. Firewall kurallarını kontrol et
3. SSL sertifikası geçerli mi?
4. CORS ayarları doğru mu?

```bash
# Test
curl -v https://api.worklines.de/health
```

---

### Problem: Modül erişilemiyor (403 Forbidden)

**Çözüm:**
1. License key geçerli mi?
2. Modül aktif mi?
3. Tenant aktif mi?

```bash
# Kontrol
GET https://api.worklines.de/api/admin/tenants/ABC_001/modules
```

---

### Problem: On-Premise License Validation Hatası

**Çözüm:**
1. License server'a erişim var mı?
2. Offline mode aktif mi?
3. License key doğru mu?

```json
// appsettings.json
{
  "License": {
    "OfflineMode": true,
    "OfflineGracePeriod": "30:00:00:00"
  }
}
```

---

## ✅ Deployment Checklist

### SaaS Deployment
- [ ] Tenant oluşturuldu
- [ ] Database oluşturuldu
- [ ] Migration çalıştırıldı
- [ ] Modüller aktif edildi
- [ ] Frontend build edildi
- [ ] Frontend müşteriye gönderildi
- [ ] Config dosyası düzenlendi
- [ ] İlk giriş test edildi

### On-Premise Deployment
- [ ] Sunucu hazır
- [ ] SQL Server kurulu
- [ ] .NET Runtime kurulu
- [ ] API package kuruldu
- [ ] Database oluşturuldu
- [ ] Migration çalıştırıldı
- [ ] License key'ler eklendi
- [ ] Windows Service kuruldu
- [ ] Frontend kuruldu
- [ ] VPN yapılandırıldı (gerekirse)
- [ ] Test edildi

### Hybrid Deployment
- [ ] Cloud API hazır
- [ ] On-premise API hazır
- [ ] API Gateway yapılandırıldı
- [ ] Route'lar test edildi
- [ ] Frontend yapılandırıldı
- [ ] Test edildi

---

## 📞 Destek

- **Email:** support@wixisoftware.com
- **Portal:** https://support.wixisoftware.com
- **Dokümantasyon:** https://docs.wixisoftware.com

---

**Son Güncelleme:** 2024

