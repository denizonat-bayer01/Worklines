# Nginx Reverse Proxy Setup Guide for worklines.de

## Sorun
`https://worklines.de/` 400 Bad Request hatası veriyor, ancak `http://localhost:5500/` çalışıyor.

## Çözüm
Nginx reverse proxy yapılandırmasını düzeltmeniz gerekiyor.

## Adımlar

### 1. Nginx Yapılandırma Dosyası Oluşturma

Sunucuda `/etc/nginx/sites-available/worklines.de` dosyasını oluşturun:

```bash
sudo nano /etc/nginx/sites-available/worklines.de
```

`docs/nginx-worklines-de.conf` dosyasındaki içeriği kopyalayın.

### 2. Docker Network Kontrolü

Frontend container'ının nginx ile aynı network'te olduğundan emin olun:

```bash
# Container'ları kontrol edin
docker ps | grep wixi-frontend

# Network'leri kontrol edin
docker network ls
docker network inspect 02_nginx_default
```

Eğer frontend container'ı nginx network'ünde değilse:

```bash
docker network connect 02_nginx_default wixi-frontend
```

### 3. Nginx Yapılandırmasını Aktif Etme

```bash
# Symlink oluştur
sudo ln -s /etc/nginx/sites-available/worklines.de /etc/nginx/sites-enabled/

# Yapılandırmayı test et
sudo nginx -t

# Nginx'i yeniden yükle
sudo systemctl reload nginx
# veya
sudo service nginx reload
```

### 4. SSL Sertifikası Kontrolü

Let's Encrypt sertifikasının doğru olduğundan emin olun:

```bash
# Sertifika kontrolü
sudo certbot certificates

# Eğer sertifika yoksa veya yenilenmesi gerekiyorsa
sudo certbot --nginx -d worklines.de -d www.worklines.de
```

### 5. Önemli Noktalar

#### Host Header Sorunu
400 Bad Request hatası genellikle `Host` header'ının yanlış iletilmesinden kaynaklanır. Nginx yapılandırmasında şu satırların olduğundan emin olun:

```nginx
proxy_set_header Host $host;
proxy_set_header X-Forwarded-Host $host;
```

#### Container Name vs IP
Eğer nginx Docker container içinde çalışıyorsa, `proxy_pass` için container name kullanın:

```nginx
proxy_pass http://wixi-frontend:5500;
```

Eğer nginx host'ta çalışıyorsa, localhost kullanın:

```nginx
proxy_pass http://localhost:5500;
```

#### Network Bağlantısı
Frontend container'ının nginx ile iletişim kurabilmesi için aynı Docker network'te olması gerekir:

```bash
# Network'ü kontrol et
docker network inspect 02_nginx_default | grep wixi-frontend

# Eğer yoksa bağla
docker network connect 02_nginx_default wixi-frontend
```

### 6. Test

```bash
# Nginx loglarını kontrol et
sudo tail -f /var/log/nginx/worklines.de.error.log
sudo tail -f /var/log/nginx/worklines.de.access.log

# Test isteği gönder
curl -I https://worklines.de/
```

### 7. Troubleshooting

#### 502 Bad Gateway
- Frontend container'ının çalıştığından emin olun: `docker ps | grep wixi-frontend`
- Network bağlantısını kontrol edin
- Port 5500'in açık olduğundan emin olun

#### 400 Bad Request
- Host header'ının doğru iletildiğinden emin olun
- Nginx yapılandırmasını kontrol edin: `sudo nginx -t`
- Nginx loglarını kontrol edin

#### SSL Sertifika Hatası
- Sertifika yolunu kontrol edin
- Sertifikayı yenileyin: `sudo certbot renew`

## Alternatif: Nginx Proxy Manager Kullanıyorsanız

Eğer Nginx Proxy Manager kullanıyorsanız:

1. Proxy Hosts sekmesine gidin
2. `worklines.de` için yeni bir proxy host oluşturun
3. Forward Hostname/IP: `wixi-frontend` (veya container IP)
4. Forward Port: `5500`
5. SSL Certificate: Let's Encrypt seçin
6. Advanced sekmesinde şu ayarları ekleyin:

```nginx
proxy_set_header Host $host;
proxy_set_header X-Real-IP $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
proxy_set_header X-Forwarded-Proto $scheme;
proxy_set_header X-Forwarded-Host $host;
```

## Referans

- [Nginx Reverse Proxy Documentation](https://nginx.org/en/docs/http/ngx_http_proxy_module.html)
- [Docker Networking](https://docs.docker.com/network/)
- [Let's Encrypt Documentation](https://letsencrypt.org/docs/)


