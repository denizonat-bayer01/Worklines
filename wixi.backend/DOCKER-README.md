# 🐳 Wixi WorkLines API - Docker Deployment

## 📋 Port Yapılandırması

| Environment | Port | URL |
|------------|------|-----|
| **Development** | 5048 | http://localhost:5048 |
| **Production** | 5045 | http://localhost:5045 |

---

## 🚀 Hızlı Başlangıç

### 1️⃣ Development Ortamı (Port 5048)

```bash
# Container'ı başlat
docker-compose up -d wixi-api-dev

# Logları izle
docker-compose logs -f wixi-api-dev

# Swagger'a eriş
# http://localhost:5048/swagger/index.html
```

### 2️⃣ Production Ortamı (Port 5045)

```bash
# Container'ı başlat
docker-compose up -d wixi-api-prod

# Logları izle
docker-compose logs -f wixi-api-prod

# Swagger'a eriş
# http://localhost:5045/swagger/index.html
```

---

## 🛠️ Docker Komutları

### Build & Run

```bash
# Development ortamını build et ve çalıştır
docker-compose up --build wixi-api-dev

# Production ortamını build et ve çalıştır
docker-compose up --build wixi-api-prod

# Her iki ortamı da çalıştır (önerilmez - port çakışması olabilir)
docker-compose up -d
```

### Container Yönetimi

```bash
# Container durumunu kontrol et
docker-compose ps

# Container'ı durdur
docker-compose stop wixi-api-dev
docker-compose stop wixi-api-prod

# Container'ı kaldır
docker-compose down

# Container'ı yeniden başlat
docker-compose restart wixi-api-dev
```

### Logs & Debugging

```bash
# Logları görüntüle
docker-compose logs wixi-api-dev
docker-compose logs wixi-api-prod

# Canlı log takibi
docker-compose logs -f wixi-api-dev

# Container içine gir
docker exec -it wixi-worklines-dev /bin/bash
docker exec -it wixi-worklines-prod /bin/bash
```

### Temizlik

```bash
# Container'ları durdur ve sil
docker-compose down

# Volume'ları da sil
docker-compose down -v

# Image'ları da sil
docker-compose down --rmi all
```

---

## 🔧 Yapılandırma

### Environment Variables

Docker Compose'da ortam değişkenlerini değiştirmek için `docker-compose.yml` dosyasını düzenleyin:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://+:5048
  - ConnectionStrings__DefaultConnection=Server=...;Database=...
```

### Port Değiştirme

Port değiştirmek için `docker-compose.yml` dosyasında:

```yaml
ports:
  - "5048:5048"  # HOST:CONTAINER
```

---

## 📊 Health Check

Container sağlık durumunu kontrol etmek için:

```bash
docker inspect --format='{{.State.Health.Status}}' wixi-worklines-dev
```

Swagger endpoint üzerinden otomatik health check yapılır:
- Development: `http://localhost:5048/swagger/index.html`
- Production: `http://localhost:5045/swagger/index.html`

---

## 🌐 Production Deployment

### Domain ile Kullanım (api.worklines.de)

Production deployment için Nginx reverse proxy kullanmanız önerilir:

```nginx
server {
    listen 80;
    server_name api.worklines.de;

    location / {
        proxy_pass http://localhost:5045;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### SSL/TLS (HTTPS)

Let's Encrypt ile SSL sertifikası:

```bash
# Certbot kur
apt-get install certbot python3-certbot-nginx

# Sertifika al
certbot --nginx -d api.worklines.de

# Otomatik yenileme
certbot renew --dry-run
```

---

## 🐛 Troubleshooting

### Port Already in Use

```bash
# Portu kullanan process'i bul (Windows)
netstat -ano | findstr :5048

# Process'i durdur
taskkill /PID <PID> /F

# Linux/Mac
lsof -ti:5048 | xargs kill -9
```

### Connection Refused

1. Container'ın çalıştığını kontrol edin:
   ```bash
   docker-compose ps
   ```

2. Logları kontrol edin:
   ```bash
   docker-compose logs wixi-api-dev
   ```

3. Health check durumunu kontrol edin:
   ```bash
   docker inspect wixi-worklines-dev
   ```

### Database Connection Issues

1. Connection string'i kontrol edin
2. Database server'ın erişilebilir olduğunu doğrulayın
3. Firewall kurallarını kontrol edin

---

## 📝 Notlar

- **Development**: Port 5048, Swagger otomatik açılır, detaylı loglar
- **Production**: Port 5045, Swagger manuel erişim, optimize edilmiş loglar
- Log dosyaları `./wixi.WebAPI/Logs` klasöründe saklanır
- Database bağlantısı her iki ortamda da aynı (değiştirilebilir)

---

**Son Güncelleme:** 31 Ekim 2025

