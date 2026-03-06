# 📚 Backend Template - Dokümantasyon İndeksi

## 🎯 Genel Bakış

Bu klasör, enterprise seviyesinde bir .NET backend template'i oluşturmak için gerekli tüm dökümanları içerir.

---

## 📖 Döküman Listesi

### 🔍 Analiz & Planlama

1. **[README.md](README.md)** ✅
   - Mevcut mimari analizi
   - Eksik sistemler listesi
   - Öncelikli geliştirmeler
   - Roadmap
   - Teknoloji stack

2. **[ARCHITECTURE.md](ARCHITECTURE.md)** ✅
   - Detaylı mimari dökümanı
   - Katmanlı yapı açıklaması
   - Design patterns
   - Data flow
   - Database schema
   - Performance considerations

3. **[IMPLEMENTATION-CHECKLIST.md](IMPLEMENTATION-CHECKLIST.md)** ✅
   - Tüm özelliklerin checklist'i
   - Faz bazlı planlama
   - Progress tracker
   - Priority matrix

4. **[DOCUMENT-TRACKING-SYSTEM-ANALYSIS.md](DOCUMENT-TRACKING-SYSTEM-ANALYSIS.md)** ✅
   - Belge Takip ve Üyelik Sistemi analizi
   - Frontend-Backend entegrasyon analizi
   - Eksik tablolar ve entity'ler
   - Önerilen veritabanı şeması
   - API endpoint'ler
   - Implementation planı

5. **[APPOINTMENT-MODULE-ANALYSIS.md](../../APPOINTMENT-MODULE-ANALYSIS.md)** ✅
   - Randevu Sistemi modülerleştirme analizi
   - Backend ve Frontend modül yapısı
   - Taşınabilirlik stratejisi
   - Implementation planı

6. **[APPOINTMENT-BACKEND-ARCHITECTURE.md](Appointment/APPOINTMENT-BACKEND-ARCHITECTURE.md)** ✅
   - Monolith vs Mikroservis karşılaştırması
   - Port yönetimi stratejisi
   - Docker yapılandırması
   - Güvenlik ve CORS yönetimi

7. **[PAYMENT-MODULE-ANALYSIS.md](Payment/PAYMENT-MODULE-ANALYSIS.md)** ✅
   - Ödeme Sistemi modülerleştirme analizi
   - iyzico entegrasyonu planı
   - PCI-DSS uyumluluk stratejisi
   - Randevu sistemi entegrasyonu
   - Entity tasarımı ve API endpoint'leri

8. **[MODULARIZATION-STRATEGY.md](MODULARIZATION-STRATEGY.md)** ✅
   - Mevcut sistemleri modüllere geçirme stratejisi
   - Incremental migration planı
   - Modül yapısı tasarımı
   - Entity, Service, Controller migration adımları
   - Risk analizi ve çözümleri
   - 10 modül migration planı (Identity, Clients, Documents, Applications, Support, Content, Email, Forms, FileStorage, Core)

9. **[MODULARIZATION-SECURITY-ANALYSIS.md](MODULARIZATION-SECURITY-ANALYSIS.md)** ✅
   - Modülerleştirme güvenlik risk analizi
   - Mikroservislere geçiş kolaylığı analizi
   - Güvenlik eksiklikleri ve çözümleri
   - Shared DbContext güvenlik riskleri
   - Inter-module communication güvenliği
   - Authorization granularity
   - Secret management
   - Audit logging
   - Mikroservislere geçiş stratejisi (5 phase)
   - Kritik eksiklikler ve öneriler

10. **[MONOLITH-SECURITY-ANALYSIS.md](MONOLITH-SECURITY-ANALYSIS.md)** ✅
    - Monolith için güvenlik sorunlarının geçerlilik analizi
    - Hangi sorunlar monolith'te geçerli?
    - Hangi sorunlar modülerleştirme sonrası ortaya çıkacak?
    - Monolith için kritik eksiklikler (Rate Limiting, Security Headers, Audit Logging)
    - Monolith için çözüm önerileri
    - Öncelik sıralaması ve implementasyon planı

---

### 🛠️ Implementation Guides

#### Faz 1: Production Hazırlığı

4. **[01-LOGGING.md](01-LOGGING.md)** ✅ **IMPLEMENTED**
   - Serilog entegrasyonu ✅
   - Structured logging ✅
   - Request/Response logging ✅
   - Database logging (ApplicationLogs table) ✅
   - File logging ✅
   - Console logging ✅
   - **Status:** ✅ **COMPLETED**

5. **[02-ERROR-HANDLING.md](02-ERROR-HANDLING.md)** ✅ **IMPLEMENTED**
   - Global exception handler ✅
   - Custom exception types ✅
   - Error response model ✅
   - **Status:** ✅ **COMPLETED**

6. **[03-VALIDATION.md](03-VALIDATION.md)** ✅ **IMPLEMENTED**
   - FluentValidation entegrasyonu ✅
   - Custom validators ✅
   - Business rule validation ✅
   - ModelValidationFilter ✅
   - **Status:** ✅ **COMPLETED**

7. **[04-SECURITY.md](04-SECURITY.md)** ⏳
   - Rate limiting
   - Security headers
   - API versioning
   - HTTPS enforcement
   - CORS yönetimi (Monolith için)
   - Authentication & Authorization
   - JWT token management
   - **Status:** ⏳ To be created
   - **Priority:** 🔴 **HIGH** (Monolith güvenliği için kritik)

8. **[05-HEALTH-CHECKS.md](05-HEALTH-CHECKS.md)** ⏳
   - Health check endpoints
   - Database health
   - Monitoring integration
   - **Status:** ⏳ To be created

#### Faz 2: Performance & Ölçeklenebilirlik

9. **[06-CACHING.md](06-CACHING.md)** ⏳
   - Redis entegrasyonu
   - Distributed cache
   - Cache strategies
   - **Status:** ⏳ To be created

10. **[07-BACKGROUND-JOBS.md](07-BACKGROUND-JOBS.md)** ⏳
    - Hangfire entegrasyonu
    - Job scheduling
    - Email queue
    - **Status:** ⏳ To be created

11. **[08-EMAIL-SERVICE.md](08-EMAIL-SERVICE.md)** ⏳
    - Email service implementation
    - Email templates
    - SMTP configuration
    - **Status:** ⏳ To be created

12. **[09-FILE-UPLOAD.md](09-FILE-UPLOAD.md)** ⏳
    - File upload service
    - Cloud storage (Azure/AWS)
    - Image processing
    - **Status:** ⏳ To be created

13. **[10-API-FEATURES.md](10-API-FEATURES.md)** ⏳
    - Pagination
    - Filtering
    - Sorting
    - Search
    - **Status:** ⏳ To be created

#### Faz 3: Testing & Quality

14. **[11-TESTING.md](11-TESTING.md)** ⏳
    - Unit tests
    - Integration tests
    - Test coverage
    - **Status:** ⏳ To be created

15. **[12-CODE-QUALITY.md](12-CODE-QUALITY.md)** ⏳
    - Static analysis
    - Code standards
    - Best practices
    - **Status:** ⏳ To be created

#### Faz 4: Microservices

16. **[13-MICROSERVICES.md](13-MICROSERVICES.md)** ⏳
    - API Gateway (YARP)
    - Message broker (RabbitMQ)
    - Service discovery
    - Circuit breaker (Polly)
    - **Status:** ⏳ To be created

17. **[14-MESSAGE-BROKER.md](14-MESSAGE-BROKER.md)** ⏳
    - RabbitMQ/MassTransit
    - Event-driven architecture
    - Message patterns
    - **Status:** ⏳ To be created

#### Faz 5: DevOps

18. **[15-DEPLOYMENT.md](15-DEPLOYMENT.md)** ⏳
    - Docker deployment
    - Kubernetes manifests
    - CI/CD pipeline
    - Environment management
    - **Status:** ⏳ To be created

19. **[16-MONITORING.md](16-MONITORING.md)** ⏳
    - Monitoring stratejisi (Monolith için)
    - Basit monitoring (Serilog + Health Checks) ✅ Mevcut
    - Prometheus/Grafana (Opsiyonel - büyük projeler için)
    - Application Insights
    - Alerting
    - **Status:** ⏳ To be created
    - **Note:** Küçük-orta ölçekli monolith için Serilog yeterli, Grafana/Prometheus opsiyonel

---

## 🗺️ Öğrenme Yolu

### Başlangıç Seviyesi
1. [README.md](README.md) - Genel bakış
2. [ARCHITECTURE.md](ARCHITECTURE.md) - Mimari anlayışı
3. [DOCUMENT-TRACKING-SYSTEM-ANALYSIS.md](DOCUMENT-TRACKING-SYSTEM-ANALYSIS.md) - Proje özel analiz
4. [01-LOGGING.md](01-LOGGING.md) - İlk implementation

### Orta Seviye
4. Error Handling
5. Validation
6. Caching
7. Background Jobs

### İleri Seviye
8. Testing
9. Microservices
10. Message Broker
11. Deployment

---

## 📊 İlerleme Durumu

| Faz | Döküman | Status | Zorluk | Süre |
|-----|---------|--------|--------|------|
| **Faz 1** | Logging | ✅ **COMPLETED** | 🟢 Kolay | ✅ DONE |
| **Faz 1** | Error Handling | ✅ **COMPLETED** | 🟢 Kolay | ✅ DONE |
| **Faz 1** | Validation | ✅ **COMPLETED** | 🟡 Orta | ✅ DONE |
| **Faz 1** | Security | ⏳ **HIGH PRIORITY** | 🟡 Orta | 5-6h |
| **Faz 1** | Health Checks | ⏳ Pending | 🟢 Kolay | 2-3h |
| **Faz 2** | Caching | ⏳ Pending | 🟡 Orta | 4-5h |
| **Faz 2** | Background Jobs | ⏳ Pending | 🟡 Orta | 5-6h |
| **Faz 2** | Email Service | ⏳ Pending | 🟢 Kolay | 3-4h |
| **Faz 2** | File Upload | ⏳ Pending | 🟡 Orta | 4-5h |
| **Faz 2** | API Features | ⏳ Pending | 🟡 Orta | 5-6h |
| **Faz 3** | Testing | ⏳ Pending | 🔴 Zor | 8-10h |
| **Faz 3** | Code Quality | ⏳ Pending | 🟡 Orta | 4-5h |
| **Faz 4** | Microservices | ⏳ Pending | 🔴 Zor | 10-12h |
| **Faz 4** | Message Broker | ⏳ Pending | 🔴 Zor | 8-10h |
| **Faz 5** | Deployment | ⏳ Pending | 🟡 Orta | 6-8h |
| **Faz 5** | Monitoring | ⏳ Pending | 🟡 Orta | 5-6h |

**Tamamlanan:** ✅ Logging, ✅ Error Handling, ✅ Validation  
**Devam Eden:** ⏳ Security (Yüksek Öncelik), ⏳ Health Checks  
**Kalan Süre:** ~70-80 saat (part-time: 1.5-2 ay)

---

## 🎯 Hızlı Başlangıç

### 1. Analiz Aşaması (TAMAMLANDI ✅)
```bash
cd wixi.backend/docs
cat README.md              # Genel analiz
cat ARCHITECTURE.md        # Mimari detaylar
cat IMPLEMENTATION-CHECKLIST.md  # Checklist
```

### 2. İlk Implementation (ŞİMDİ)
```bash
cat 01-LOGGING.md          # Logging guide'ı oku
# Adım adım uygula
```

### 3. Sonraki Adımlar
- Error Handling
- Validation
- Security

---

## 💡 Kullanım Önerileri

### Proje Başlangıcı
1. ✅ **README.md** oku - Genel resmi anla
2. ✅ **ARCHITECTURE.md** oku - Mimari kavrayış
3. ✅ **IMPLEMENTATION-CHECKLIST.md** oku - Ne yapılacağını gör

### Implementation Sırası
4. ✅ **01-LOGGING.md** - **COMPLETED** ✅
5. ✅ **02-ERROR-HANDLING.md** - **COMPLETED** ✅
6. ✅ **03-VALIDATION.md** - **COMPLETED** ✅
7. 🔴 **04-SECURITY.md** - **HIGH PRIORITY** ⏳ (Monolith güvenliği için kritik)
8. 🟡 **05-HEALTH-CHECKS.md** - Önemli ⏳

### İleri Seviye
- **06-CACHING.md** ve sonrası
- **Microservices** dökümanları
- **DevOps** dökümanları

---

## 🔄 Döküman Güncelleme

Her yeni özellik eklendiğinde:
1. Implementation guide oluştur
2. IMPLEMENTATION-CHECKLIST.md güncelle
3. INDEX.md güncelle (bu dosya)
4. README.md'de progress güncelle

---

## 📞 Yardım

### Sorular
- Mimari hakkında → [ARCHITECTURE.md](ARCHITECTURE.md)
- Neyi önce yapmalıyım → [README.md](README.md) - Roadmap
- Nasıl implement ederim → İlgili implementation guide

### Katkı
- Her döküman bağımsız çalışabilir
- Adım adım talimatlar içerir
- Kod örnekleri eksiksizdir
- Test senaryoları vardır

---

## 📈 Versiyon Geçmişi

| Versiyon | Tarih | Değişiklik |
|----------|-------|------------|
| 1.7.0 | 2025-01-XX | Monolith Güvenlik Analizi eklendi |
| | | - MONOLITH-SECURITY-ANALYSIS.md ✅ |
| | | - Monolith için güvenlik sorunlarının geçerlilik analizi |
| | | - 10 sorundan 9'u monolith için geçerli |
| | | - Kritik eksiklikler (Rate Limiting, Security Headers, Audit Logging) |
| | | - Monolith için çözüm önerileri ve implementasyon planı |
| 1.6.0 | 2025-01-XX | Modülerleştirme Güvenlik Analizi eklendi |
| | | - MODULARIZATION-SECURITY-ANALYSIS.md ✅ |
| | | - Güvenlik risk analizi (Shared DbContext, Inter-module Communication) |
| | | - Mikroservislere geçiş kolaylığı analizi |
| | | - Kritik eksiklikler (Rate Limiting, Security Headers, Audit Logging) |
| | | - Mikroservislere geçiş stratejisi (5 Phase, 6-9 ay) |
| | | - Güvenlik önerileri ve çözümleri |
| 1.5.0 | 2025-01-XX | Modülerleştirme Stratejisi eklendi |
| | | - MODULARIZATION-STRATEGY.md ✅ |
| | | - Mevcut sistemleri modüllere geçirme planı |
| | | - 10 modül migration stratejisi (Identity, Clients, Documents, Applications, Support, Content, Email, Forms, FileStorage, Core) |
| | | - Incremental migration yaklaşımı |
| | | - Risk analizi ve çözümleri |
| | | - Interface/Service ayrımı |
| 1.4.0 | 2025-01-XX | Ödeme Sistemi Modülü Analizi eklendi |
| | | - PAYMENT-MODULE-ANALYSIS.md ✅ |
| | | - iyzico entegrasyonu planı |
| | | - PCI-DSS uyumluluk stratejisi |
| | | - Randevu sistemi entegrasyonu |
| 1.3.0 | 2025-01-XX | Monitoring ve Güvenlik Stratejisi güncellendi |
| | | - Monolith için monitoring stratejisi eklendi |
| | | - Grafana/Prometheus gerekliliği açıklandı (Opsiyonel) |
| | | - Güvenlik yönetimi monolith için detaylandırıldı |
| | | - Randevu sistemi modülü analizi eklendi |
| | | - Logging, Error Handling, Validation durumu güncellendi (✅ COMPLETED) |
| 1.2.0 | 2025-01-XX | Randevu Sistemi Modülü Analizi eklendi |
| | | - APPOINTMENT-MODULE-ANALYSIS.md ✅ |
| | | - APPOINTMENT-BACKEND-ARCHITECTURE.md ✅ |
| | | - Monolith vs Mikroservis karşılaştırması |
| 1.1.0 | 2025-11-04 | Belge Takip Sistemi Analizi eklendi |
| | | - DOCUMENT-TRACKING-SYSTEM-ANALYSIS.md ✅ |
| | | - 16 yeni entity tanımı |
| | | - API endpoint planlaması |
| | | - Frontend-Backend entegrasyon analizi |
| 1.0.0 | 2025-10-28 | İlk döküman seti oluşturuldu |
| | | - README.md ✅ |
| | | - ARCHITECTURE.md ✅ |
| | | - IMPLEMENTATION-CHECKLIST.md ✅ |
| | | - 01-LOGGING.md ✅ |
| | | - INDEX.md ✅ |

---

## 🎓 Sonuç

Bu döküman seti, sıfırdan enterprise-grade bir backend template oluşturmak için kapsamlı bir rehberdir.

### Mevcut Durum
- ✅ 10 temel döküman hazır
- ✅ Belge Takip Sistemi analizi tamamlandı
- ✅ Randevu Sistemi modülü analizi tamamlandı
- ✅ **Ödeme Sistemi modülü analizi tamamlandı (iyzico)**
- ✅ **Modülerleştirme Stratejisi hazır (10 modül migration planı)**
- ✅ **Modülerleştirme Güvenlik Analizi hazır (Güvenlik riskleri, Mikroservis geçişi)**
- ✅ **Monolith Güvenlik Analizi hazır (10 sorundan 9'u monolith için geçerli)**
- ✅ **Logging, Error Handling, Validation IMPLEMENTED**
- ⏳ Security (Yüksek Öncelik - Rate Limiting, Security Headers, Audit Logging)
- ⏳ Health Checks
- ⏳ 8 döküman daha oluşturulacak

### Hedef
- 🎯 Production-ready template
- 🎯 Microservices hazır
- 🎯 Test edilebilir
- 🎯 Dokümante edilmiş

### Sonraki Adım
**[04-SECURITY.md](04-SECURITY.md)** ile devam edin! 🔒 (Monolith güvenliği için kritik)

---

## 🔒 Monolith Güvenlik ve Monitoring Stratejisi

### 📊 Monitoring: Grafana/Prometheus Gerekli mi?

#### ✅ Mevcut Durum (Yeterli)
- ✅ **Serilog** - Structured logging (Database + File + Console)
- ✅ **ApplicationLogs** table - Tüm loglar veritabanında
- ✅ **Request/Response logging** - Tüm API istekleri loglanıyor
- ✅ **Error logging** - Hatalar detaylı loglanıyor
- ✅ **Performance logging** - Request süreleri loglanıyor

#### 🤔 Grafana/Prometheus Ne Zaman Gerekli?

**Gerekli DEĞİL:**
- ✅ Küçük-orta ölçekli projeler (< 1000 req/s)
- ✅ Tek instance monolith
- ✅ Basit monitoring yeterli
- ✅ Serilog + Database logging yeterli

**Gerekli OLABİLİR:**
- ⚠️ Büyük ölçekli projeler (> 1000 req/s)
- ⚠️ Multiple instance (Load balancing)
- ⚠️ Real-time metrics gerekiyorsa
- ⚠️ Advanced alerting gerekiyorsa
- ⚠️ Distributed tracing gerekiyorsa

#### 💡 Öneri: Aşamalı Yaklaşım

**Faz 1: Basit Monitoring (ŞU AN) ✅**
```
Serilog → Database (ApplicationLogs) → SQL Queries
```
- Logları SQL ile sorgula
- Basit dashboard (Admin panel)
- Email alerts (kritik hatalar için)

**Faz 2: Health Checks (Yakın Gelecek)**
```
Health Checks → /health endpoint → Monitoring tools
```
- Database health
- External services health
- Basic metrics

**Faz 3: Grafana/Prometheus (Gerektiğinde)**
```
Prometheus → Grafana → Advanced Dashboards
```
- Real-time metrics
- Advanced alerting
- Distributed tracing

### 🔐 Monolith Güvenlik Yönetimi

#### Mevcut Güvenlik Özellikleri
- ✅ **JWT Authentication** - Token-based auth
- ✅ **Role-based Authorization** - Admin, Client roles
- ✅ **CORS** - Cross-origin resource sharing
- ✅ **HTTPS** - SSL/TLS encryption
- ✅ **Password Hashing** - BCrypt
- ✅ **SQL Injection Protection** - EF Core
- ✅ **XSS Protection** - Input validation

#### Eksik Güvenlik Özellikleri (Yüksek Öncelik)
- ⏳ **Rate Limiting** - DDoS koruması
- ⏳ **Security Headers** - HSTS, CSP, X-Frame-Options
- ⏳ **API Versioning** - Backward compatibility
- ⏳ **Request Size Limits** - Dosya upload limitleri
- ⏳ **IP Whitelisting** - Admin panel için
- ⏳ **Audit Logging** - Güvenlik olayları
- ⏳ **Token Refresh** - JWT refresh tokens
- ⏳ **2FA** - İki faktörlü kimlik doğrulama (kısmen var)

#### Güvenlik Best Practices (Monolith)

**1. Rate Limiting**
```csharp
// AspNetCoreRateLimit kullan
services.AddMemoryCache();
services.Configure<IpRateLimitOptions>(options => {
    options.GeneralRules = new List<RateLimitRule> {
        new RateLimitRule {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        }
    };
});
```

**2. Security Headers**
```csharp
app.Use(async (context, next) => {
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
    await next();
});
```

**3. Input Validation**
- ✅ FluentValidation (Mevcut)
- ✅ Model validation
- ✅ SQL injection protection (EF Core)

**4. Authentication & Authorization**
- ✅ JWT tokens
- ✅ Role-based access
- ⏳ Refresh tokens
- ⏳ Token blacklisting

**5. Audit Logging**
```csharp
// Tüm kritik işlemleri logla
Log.Information("User {UserId} created appointment {AppointmentId}", 
    userId, appointmentId);
```

### 🏗️ Monolith Büyüyünce Ne Yapmalı?

#### Aşama 1: Monolith (ŞU AN) ✅
- Tek proje
- Tek database
- Tek deployment
- Basit monitoring (Serilog)

#### Aşama 2: Modular Monolith (Yakın Gelecek)
- Modüler yapı (Appointments, Documents, etc.)
- Shared database
- Tek deployment
- Health checks + basit metrics

#### Aşama 3: Microservices (Gerektiğinde)
- Ayrı servisler
- API Gateway
- Service discovery
- Distributed tracing (Grafana/Prometheus)

### 📋 Güvenlik Checklist (Monolith)

#### Kritik (Yüksek Öncelik)
- [ ] Rate limiting
- [ ] Security headers
- [ ] Request size limits
- [ ] Token refresh mechanism
- [ ] Audit logging

#### Önemli (Orta Öncelik)
- [ ] API versioning
- [ ] IP whitelisting (admin)
- [ ] 2FA (tam implementasyon)
- [ ] Password policy
- [ ] Session management

#### İyi Olur (Düşük Öncelik)
- [ ] WAF (Web Application Firewall)
- [ ] DDoS protection
- [ ] Penetration testing
- [ ] Security scanning

### 🔍 Monitoring Checklist (Monolith)

#### Mevcut ✅
- [x] Serilog logging
- [x] Database logging
- [x] Error logging
- [x] Request logging

#### Yakın Gelecek ⏳
- [ ] Health checks
- [ ] Basic metrics endpoint
- [ ] Simple dashboard
- [ ] Email alerts

#### İleride (Gerektiğinde)
- [ ] Prometheus metrics
- [ ] Grafana dashboards
- [ ] Distributed tracing
- [ ] APM (Application Performance Monitoring)

---

## 🆕 Yeni Eklenen: Belge Takip Sistemi

**[DOCUMENT-TRACKING-SYSTEM-ANALYSIS.md](DOCUMENT-TRACKING-SYSTEM-ANALYSIS.md)** - Proje özel kapsamlı analiz:

- ✅ Frontend analizi (Dashboard, Documents, Profile, Support)
- ✅ Backend mevcut durum analizi
- ✅ 18 yeni entity oluşturuldu ve migration uygulandı
- ✅ Veritabanı şeması implementasyonu tamamlandı
- ✅ Seed data hazırlandı ve uygulandı (63 kayıt)
- ⏳ API endpoint implementation devam ediyor

**Tamamlanan Entity'ler:**
- ✅ Client (Müşteri Profili) + EducationType + EducationInfo
- ✅ Document (Belge Yönetimi) + DocumentType + DocumentReview + FileStorage
- ✅ Application (Başvuru Takip) + Templates + Steps + SubSteps + History
- ✅ SupportTicket (Destek Sistemi) + SupportMessage + FAQ
- ✅ Notification (Bildirimler)

**Seed Data (63 kayıt):**
- ✅ 3 Education Types (Üniversite, Meslek Lisesi, Kalfalık)
- ✅ 14 Document Types (Eğitim tiplerine göre)
- ✅ 3 Application Templates + Step Templates + Sub-Step Templates
- ✅ 10 FAQ (Sık Sorulan Sorular)

---

**Son Güncelleme:** 4 Kasım 2025  
**Maintainer:** Wixi Backend Team

