# 📋 Deployment Senaryoları Özeti

**Hızlı Referans Rehberi**

---

## 🎯 Senaryo Karşılaştırması

| Özellik | SaaS ☁️ | On-Premise 🏢 | Hybrid 🔄 |
|---------|---------|--------------|----------|
| **Kurulum Süresi** | 1 gün | 1-2 hafta | 1 hafta |
| **Maliyet** | Düşük (€X/ay) | Yüksek (Tek seferlik) | Orta |
| **Güvenlik** | Orta | Yüksek | Yüksek |
| **Bakım** | Otomatik | Manuel | Karma |
| **Ölçeklenebilirlik** | Yüksek | Orta | Yüksek |
| **Veri Kontrolü** | Cloud | Müşteri | Karma |
| **İnternet Gereksinimi** | Var | Yok | Kısmi |
| **Özelleştirme** | Sınırlı | Yüksek | Yüksek |

---

## 📦 Modül Listesi ve Lisans Gereksinimleri

### Core Modüller (Zorunlu)
- ✅ **Identity** - Her zaman aktif
- ✅ **Core** - Her zaman aktif

### Business Modülleri
| Modül | Basic | Premium | Enterprise |
|-------|-------|---------|------------|
| **Clients** | ✅ | ✅ | ✅ |
| **Documents** | ✅ | ✅ | ✅ |
| **Support** | ✅ | ✅ | ✅ |
| **Content** | ✅ | ✅ | ✅ |
| **Forms** | ✅ | ✅ | ✅ |
| **Email** | ✅ | ✅ | ✅ |
| **Applications** | ❌ | ✅ | ✅ |
| **Appointments** | ❌ | ✅ | ✅ |
| **Payments** | ❌ | ❌ | ✅ |

---

## 🚀 Hızlı Senaryo Seçimi

### Senaryo 1: SaaS ☁️
**Ne zaman seçilir?**
- ✅ 10-50 kullanıcı
- ✅ Hızlı kurulum isteniyor
- ✅ Standart modüller yeterli
- ✅ İnternet bağlantısı mevcut

**Kurulum:**
1. Tenant oluştur → 5 dk
2. Database oluştur → 10 dk
3. Frontend kur → 15 dk
4. **Toplam: ~30 dakika** ⚡

**Detaylar:** [QUICK-START-DEPLOYMENT-GUIDE.md](./QUICK-START-DEPLOYMENT-GUIDE.md#senaryo-1-saas-küçük-firma)

---

### Senaryo 2: On-Premise 🏢
**Ne zaman seçilir?**
- ✅ 100+ kullanıcı
- ✅ Yüksek güvenlik gereksinimleri
- ✅ Veri lokal olmalı
- ✅ Özel modül kombinasyonu

**Kurulum:**
1. Sunucu hazırlığı → 1 gün
2. API kurulumu → 2 gün
3. Database kurulumu → 1 gün
4. Frontend kurulumu → 1 gün
5. Test ve yapılandırma → 2 gün
6. **Toplam: ~1 hafta** 📅

**Detaylar:** [QUICK-START-DEPLOYMENT-GUIDE.md](./QUICK-START-DEPLOYMENT-GUIDE.md#senaryo-2-on-premise-büyük-firma)

---

### Senaryo 3: Hybrid 🔄
**Ne zaman seçilir?**
- ✅ 500+ kullanıcı
- ✅ Karma güvenlik gereksinimleri
- ✅ Public + Private modüller
- ✅ API Gateway kullanımı

**Kurulum:**
1. Cloud API yapılandırma → 1 gün
2. On-premise API yapılandırma → 2 gün
3. API Gateway kurulumu → 1 gün
4. Frontend yapılandırma → 1 gün
5. Test → 1 gün
6. **Toplam: ~1 hafta** 📅

**Detaylar:** [QUICK-START-DEPLOYMENT-GUIDE.md](./QUICK-START-DEPLOYMENT-GUIDE.md#senaryo-3-hybrid-kurumsal)

---

## 🌐 Uzaktan Erişim Modelleri

### 1. Direct API (HTTPS) 🔒
- **Kullanım:** SaaS modeli
- **Güvenlik:** TLS 1.3 + JWT
- **Kurulum:** 5 dakika
- **Detaylar:** [DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md](./DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md#model-1-direct-api-access-doğrudan-api-erisimi)

### 2. VPN Tunneling 🔐
- **Kullanım:** On-premise modeli
- **Güvenlik:** IPSec/OpenVPN + JWT
- **Kurulum:** 1-2 saat
- **Detaylar:** [DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md](./DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md#model-2-vpn-tunneling-vpn-tüneli)

### 3. API Gateway (mTLS) 🛡️
- **Kullanım:** Enterprise, Hybrid
- **Güvenlik:** Mutual TLS + Certificate Pinning
- **Kurulum:** 2-3 saat
- **Detaylar:** [DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md](./DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md#model-3-api-gateway-with-mtls-mutual-tls)

---

## 💼 Gerçek Dünya Örnekleri

### Örnek 1: Küçük Danışmanlık Firması
- **Firma:** ABC Consulting GmbH
- **Kullanıcı:** 15 kişi
- **Senaryo:** SaaS (Basic)
- **Modüller:** Identity, Clients, Documents, Support, Content, Forms, Email
- **Kurulum:** 30 dakika ⚡
- **Maliyet:** €X/ay

### Örnek 2: Büyük Kurumsal Firma
- **Firma:** XYZ Corporation
- **Kullanıcı:** 200 kişi
- **Senaryo:** On-Premise (Enterprise)
- **Modüller:** Tüm modüller
- **Kurulum:** 1 hafta 📅
- **Maliyet:** Tek seferlik + yıllık bakım

### Örnek 3: Kurumsal Holding
- **Firma:** DEF Industries
- **Kullanıcı:** 500+ kişi
- **Senaryo:** Hybrid (Premium)
- **Cloud Modüller:** Identity, Content, Forms
- **On-Premise Modüller:** Clients, Documents, Applications, Payments
- **Kurulum:** 1 hafta 📅
- **Maliyet:** Karma model

---

## 🔧 Modül Yönetimi

### Modül Aktif Etme
```bash
POST /api/admin/tenants/{tenantCode}/modules/activate
{
  "moduleName": "Payments",
  "licenseKey": "PAYMENTS-XXXX-XXXX"
}
```

### Modül Pasif Etme
```bash
POST /api/admin/tenants/{tenantCode}/modules/deactivate
{
  "moduleName": "Payments"
}
```

### Aktif Modülleri Listeleme
```bash
GET /api/admin/tenants/{tenantCode}/modules
```

---

## 📊 Karar Matrisi

**Hangi senaryoyu seçmeliyim?**

```
Kullanıcı Sayısı < 50?
  ├─ Evet → SaaS ☁️
  └─ Hayır → Devam

Güvenlik Kritik mi?
  ├─ Evet → On-Premise 🏢
  └─ Hayır → Devam

Özel Modül Kombinasyonu?
  ├─ Evet → On-Premise 🏢 veya Hybrid 🔄
  └─ Hayır → SaaS ☁️

Karma Gereksinimler?
  ├─ Evet → Hybrid 🔄
  └─ Hayır → On-Premise 🏢
```

---

## 📚 Detaylı Dokümanlar

1. **[DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md](./DEPLOYMENT-AND-REMOTE-ACCESS-SCENARIOS.md)**
   - Tüm deployment modelleri
   - Uzaktan erişim stratejileri
   - Güvenlik detayları
   - Teknik konfigürasyonlar

2. **[QUICK-START-DEPLOYMENT-GUIDE.md](./QUICK-START-DEPLOYMENT-GUIDE.md)**
   - Adım adım kurulum
   - Pratik örnekler
   - Troubleshooting
   - Checklist'ler

3. **[MULTI-TENANT-ANALYSIS-AND-RECOMMENDATIONS.md](./MULTI-TENANT-ANALYSIS-AND-RECOMMENDATIONS.md)**
   - Mimari detaylar
   - Kod örnekleri
   - Best practices

---

## ✅ Hızlı Checklist

### SaaS Deployment
- [ ] Tenant oluşturuldu
- [ ] Database hazır
- [ ] Frontend kuruldu
- [ ] Config düzenlendi
- [ ] Test edildi

### On-Premise Deployment
- [ ] Sunucu hazır
- [ ] API kuruldu
- [ ] Database kuruldu
- [ ] License key'ler eklendi
- [ ] Frontend kuruldu
- [ ] VPN yapılandırıldı (gerekirse)
- [ ] Test edildi

### Hybrid Deployment
- [ ] Cloud API hazır
- [ ] On-premise API hazır
- [ ] API Gateway yapılandırıldı
- [ ] Frontend yapılandırıldı
- [ ] Test edildi

---

## 📞 Destek

- **Email:** support@wixisoftware.com
- **Portal:** https://support.wixisoftware.com
- **Dokümantasyon:** https://docs.wixisoftware.com

---

**Son Güncelleme:** 2024

