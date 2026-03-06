# Randevu Sistemi Backend Mimari Seçenekleri

## 🏗️ Mimari Seçenekler

### Seçenek 1: Monolith (Aynı Proje, Aynı Port) ✅ ÖNERİLEN

**Yapı:**
```
wixi.backend/
├── wixi.WebAPI/              # Ana API (Port 5045/5048)
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── AppointmentsController.cs  # ✅ Buraya eklenecek
│   │   └── ...
│   └── Program.cs
└── wixi.Appointments/        # Modül projesi
    ├── Entities/
    ├── Services/
    └── DTOs/
```

**Avantajlar:**
- ✅ Tek port (5045/5048)
- ✅ CORS sorunları yok
- ✅ Tek database connection
- ✅ Kolay deployment
- ✅ Transaction yönetimi kolay
- ✅ Shared authentication/authorization
- ✅ Daha az network overhead

**Dezavantajlar:**
- ❌ Tek proje büyür
- ❌ Bağımsız scale edilemez
- ❌ Tek hata noktası

**Port Yapılandırması:**
```yaml
# docker-compose.yml (değişiklik yok)
services:
  wixi-api-prod:
    ports:
      - "5045:5045"  # Tek port, tüm endpoint'ler burada
```

**URL Yapısı:**
```
https://api.worklines.de/api/auth/login
https://api.worklines.de/api/appointments
https://api.worklines.de/api/admin/appointments
```

---

### Seçenek 2: Mikroservis (Ayrı Proje, Farklı Port) 🔄

**Yapı:**
```
wixi.backend/
├── wixi.WebAPI/              # Ana API (Port 5045)
│   └── Controllers/
└── wixi.Appointments.API/    # Appointment API (Port 5050)
    └── Controllers/
```

**Avantajlar:**
- ✅ Bağımsız deployment
- ✅ Bağımsız scale
- ✅ Teknoloji bağımsızlığı
- ✅ Takım bağımsızlığı
- ✅ Fault isolation

**Dezavantajlar:**
- ❌ Farklı portlar (CORS sorunları)
- ❌ API Gateway gerekli
- ❌ Distributed transaction yönetimi zor
- ❌ Network latency
- ❌ Daha karmaşık deployment
- ❌ Shared database veya service communication gerekli

**Port Yapılandırması:**
```yaml
# docker-compose.yml
services:
  wixi-api-prod:
    ports:
      - "5045:5045"  # Ana API
  
  wixi-appointments-api:
    ports:
      - "5050:5050"  # Appointment API
```

**URL Yapısı:**
```
https://api.worklines.de/api/auth/login          # Ana API
https://api-appointments.worklines.de/api/appointments  # Appointment API
```

---

## 🎯 Öneri: Monolith (Seçenek 1) + Modüler Yapı

### Neden Monolith?

1. **Küçük-Orta Ölçekli Proje**: Mikroservis overhead'i gereksiz
2. **Shared Resources**: Aynı database, authentication, team members
3. **Deployment Basitliği**: Tek container, tek deployment
4. **Transaction Management**: Cross-service transaction'lar zor
5. **CORS Sorunları**: Farklı portlar CORS sorunları yaratır

### Modüler Monolith Yaklaşımı

```
wixi.backend/
├── wixi.WebAPI/                    # API Gateway Layer
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── AppointmentsController.cs      # ✅ Public API
│   │   └── AdminAppointmentsController.cs # ✅ Admin API
│   └── Program.cs
│
├── wixi.Appointments/              # Appointment Module
│   ├── Entities/
│   │   ├── Appointment.cs
│   │   ├── AppointmentSlot.cs
│   │   └── AppointmentSettings.cs
│   ├── DTOs/
│   │   ├── AppointmentCreateDto.cs
│   │   └── AppointmentResponseDto.cs
│   ├── Interfaces/
│   │   └── IAppointmentService.cs
│   ├── Services/
│   │   ├── AppointmentService.cs
│   │   └── AppointmentNotificationService.cs
│   └── Validators/
│       └── AppointmentValidator.cs
│
├── wixi.Business/                  # Mevcut business logic
├── wixi.DataAccess/                # Mevcut data access
└── wixi.Entities/                  # Mevcut entities
    └── Concrete/
        └── Appointment/            # ✅ Yeni appointment entities
```

---

## 📋 Implementation Plan: Monolith Yaklaşımı

### Adım 1: Proje Yapısı Oluştur

```bash
# wixi.backend klasöründe
dotnet new classlib -n wixi.Appointments -f net8.0
```

### Adım 2: Dependencies Ekle

```xml
<!-- wixi.Appointments/wixi.Appointments.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\wixi.Entities\wixi.Entities.csproj" />
    <ProjectReference Include="..\wixi.Business\wixi.Business.csproj" />
  </ItemGroup>
</Project>
```

### Adım 3: wixi.WebAPI'ye Reference Ekle

```xml
<!-- wixi.WebAPI/wixi.WebAPI.csproj -->
<ItemGroup>
  <ProjectReference Include="..\wixi.Appointments\wixi.Appointments.csproj" />
</ItemGroup>
```

### Adım 4: DbContext'e Entity'leri Ekle

```csharp
// wixi.DataAccess/Concrete/EntityFramework/Contexts/WixiDbContext.cs
public class WixiDbContext : IdentityDbContext<AppUser, AppRole, int>
{
    // Mevcut DbSet'ler
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<Client> Clients { get; set; }
    
    // ✅ Yeni Appointment DbSet'ler
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentSlot> AppointmentSlots { get; set; }
    public DbSet<AppointmentSettings> AppointmentSettings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Mevcut configurations
        // ...
        
        // ✅ Appointment configurations
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentSlotConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentSettingsConfiguration());
    }
}
```

### Adım 5: Service Registration

```csharp
// wixi.WebAPI/Program.cs
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAppointmentNotificationService, AppointmentNotificationService>();
```

### Adım 6: Controller Ekle

```csharp
// wixi.WebAPI/Controllers/AppointmentsController.cs
[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    
    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto createDto)
    {
        // Implementation
    }
}
```

---

## 🔄 Alternatif: Mikroservis Yaklaşımı (Gerekirse)

Eğer gerçekten ayrı bir backend istiyorsanız:

### Yapı

```
wixi.backend/
├── wixi.WebAPI/              # Ana API (Port 5045)
└── wixi.Appointments.API/    # Appointment API (Port 5050)
    ├── Controllers/
    ├── Services/
    └── Program.cs
```

### Docker Compose

```yaml
services:
  # Ana API
  wixi-api-prod:
    build:
      context: .
      dockerfile: wixi.WebAPI/Dockerfile
    ports:
      - "5045:5045"
    environment:
      - ASPNETCORE_URLS=http://+:5045
      - ConnectionStrings__DefaultConnection=Server=78.188.86.124,1533;...
    networks:
      - wixi-network

  # Appointment API
  wixi-appointments-api:
    build:
      context: .
      dockerfile: wixi.Appointments.API/Dockerfile
    ports:
      - "5050:5050"
    environment:
      - ASPNETCORE_URLS=http://+:5050
      - ConnectionStrings__DefaultConnection=Server=78.188.86.124,1533;...
      - ApiSettings__MainApiUrl=http://wixi-api-prod:5045
    networks:
      - wixi-network
    depends_on:
      - wixi-api-prod

networks:
  wixi-network:
    driver: bridge
```

### CORS Ayarları

```csharp
// wixi.Appointments.API/Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMainAPI", policy =>
    {
        policy.WithOrigins("https://api.worklines.de")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Frontend Entegrasyonu

```typescript
// V4/src/ApiServices/config/api.config.ts
export const API_ROUTES = {
    BASE_URL: import.meta.env.VITE_API_BASE_URL || 'https://api.worklines.de',
    APPOINTMENTS_BASE_URL: import.meta.env.VITE_APPOINTMENTS_API_URL || 'https://api-appointments.worklines.de',
    
    APPOINTMENTS: {
        CREATE: `${API_ROUTES.APPOINTMENTS_BASE_URL}/api/appointments`,
        AVAILABLE_SLOTS: (teamMemberId: number, date: string) => 
            `${API_ROUTES.APPOINTMENTS_BASE_URL}/api/appointments/available-slots/${teamMemberId}?date=${date}`,
        // ...
    }
};
```

### API Gateway (Nginx) - Opsiyonel

```nginx
# nginx.conf
server {
    listen 80;
    server_name api.worklines.de;

    # Ana API
    location /api/auth {
        proxy_pass http://wixi-api-prod:5045;
    }
    
    location /api/team-members {
        proxy_pass http://wixi-api-prod:5045;
    }
    
    # Appointment API
    location /api/appointments {
        proxy_pass http://wixi-appointments-api:5050;
    }
    
    location /api/admin/appointments {
        proxy_pass http://wixi-appointments-api:5050;
    }
}
```

---

## 🎯 Sonuç ve Öneri

### ✅ ÖNERİLEN: Monolith + Modüler Yapı

**Neden?**
1. **Basitlik**: Tek port, tek deployment
2. **Performans**: Network overhead yok
3. **Transaction**: Shared transaction context
4. **CORS**: Sorun yok
5. **Bakım**: Daha kolay

**Ne zaman Mikroservis?**
- Team büyüklüğü 10+ kişi
- Farklı teknolojiler kullanmak istiyorsanız
- Bağımsız scale gerekiyorsa
- Farklı deployment cycle'ları gerekiyorsa

### 📊 Karşılaştırma Tablosu

| Özellik | Monolith | Mikroservis |
|---------|----------|-------------|
| Port | Tek (5045) | Çoklu (5045, 5050) |
| Deployment | Basit | Karmaşık |
| CORS | Sorun yok | Yapılandırma gerekli |
| Transaction | Kolay | Zor |
| Scale | Birlikte | Bağımsız |
| Bakım | Kolay | Zor |
| Network | Yok | Var |
| Database | Shared | Shared veya Ayrı |

---

## 🚀 Implementation: Monolith Yaklaşımı

### Adım 1: Proje Oluştur

```bash
cd wixi.backend
dotnet new classlib -n wixi.Appointments -f net8.0
```

### Adım 2: Solution'a Ekle

```bash
dotnet sln add wixi.Appointments/wixi.Appointments.csproj
```

### Adım 3: Reference'ları Ekle

```bash
cd wixi.Appointments
dotnet add reference ../wixi.Entities/wixi.Entities.csproj
dotnet add reference ../wixi.Business/wixi.Business.csproj
```

### Adım 4: wixi.WebAPI'ye Reference Ekle

```bash
cd ../wixi.WebAPI
dotnet add reference ../wixi.Appointments/wixi.Appointments.csproj
```

### Adım 5: Entity'leri Oluştur

```csharp
// wixi.Entities/Concrete/Appointment/Appointment.cs
namespace wixi.Entities.Concrete.Appointment
{
    public class Appointment
    {
        public int Id { get; set; }
        public string AppointmentNumber { get; set; } = string.Empty;
        public int TeamMemberId { get; set; }
        public virtual TeamMember TeamMember { get; set; } = null!;
        // ... diğer property'ler
    }
}
```

### Adım 6: Migration Oluştur

```bash
cd wixi.DataAccess
dotnet ef migrations add AddAppointmentModule --startup-project ../wixi.WebAPI
dotnet ef database update --startup-project ../wixi.WebAPI
```

---

## 🔧 Docker Yapılandırması (Monolith)

```yaml
# docker-compose.yml (değişiklik YOK)
services:
  wixi-api-prod:
    build:
      context: .
      dockerfile: wixi.WebAPI/Dockerfile
    ports:
      - "5045:5045"  # ✅ Tek port, tüm endpoint'ler
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5045
      - ConnectionStrings__DefaultConnection=Server=78.188.86.124,1533;...
```

**Dockerfile (değişiklik yok):**
```dockerfile
# wixi.WebAPI/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["wixi.WebAPI/wixi.WebAPI.csproj", "wixi.WebAPI/"]
COPY ["wixi.Appointments/wixi.Appointments.csproj", "wixi.Appointments/"]  # ✅ Yeni
COPY ["wixi.Entities/wixi.Entities.csproj", "wixi.Entities/"]
# ... diğer projeler
RUN dotnet restore "wixi.WebAPI/wixi.WebAPI.csproj"
COPY . .
WORKDIR "/src/wixi.WebAPI"
RUN dotnet build "wixi.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "wixi.WebAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5045
ENTRYPOINT ["dotnet", "wixi.WebAPI.dll"]
```

---

## 📝 Özet

### ✅ Önerilen Çözüm: Monolith + Modüler Yapı

1. **Aynı Port**: 5045 (production), 5048 (development)
2. **Aynı Proje**: wixi.WebAPI içinde controller'lar
3. **Ayrı Modül**: wixi.Appointments class library
4. **Aynı Database**: Shared DbContext
5. **Tek Deployment**: Tek container

### 🎯 Avantajlar

- ✅ Basit deployment
- ✅ CORS sorunları yok
- ✅ Shared transaction
- ✅ Kolay bakım
- ✅ Performans (network overhead yok)

### 📦 Yapı

```
wixi.backend/
├── wixi.WebAPI/           # API (Port 5045)
│   └── Controllers/
│       └── AppointmentsController.cs
└── wixi.Appointments/     # Modül (Class Library)
    ├── Entities/
    ├── Services/
    └── DTOs/
```

