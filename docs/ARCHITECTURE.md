# 🏗️ Backend Template - Mimari Dokümantasyon

## 📐 Katmanlı Mimari Detayları

### 1. **wixi.Entities (Domain Layer)**

**Sorumluluk:** Veritabanı varlıklarını ve veri transfer objelerini tanımlar.

```
wixi.Entities/
├── Concrete/
│   ├── AppUser.cs              # Kullanıcı entity (Identity)
│   ├── AppRole.cs              # Rol entity (Identity)
│   └── TokenBlacklist.cs       # Blacklist edilen tokenlar
│
└── DTOs/
    ├── UserForLoginDto.cs      # Login request DTO
    ├── UserForRegisterDto.cs   # Register request DTO
    └── TokenDto.cs             # Token response DTO
```

**Bağımlılıklar:**
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Http

**Prensip:** 
- ✅ Pure domain objects
- ✅ No business logic
- ✅ No dependencies on other layers

---

### 2. **wixi.Core (Core/Shared Layer)**

**Sorumluluk:** Ortak kullanılan utility'ler, helper'lar ve konfigürasyonlar.

```
wixi.Core/
├── Configuration/
│   └── JwtTokenOptions.cs      # JWT ayarları
│
└── Utilities/
    └── Security/
        └── JWT/
            ├── ITokenHelper.cs  # Token interface
            └── JwtHelper.cs     # Token generator
```

**Bağımlılıklar:**
- System.IdentityModel.Tokens.Jwt
- Microsoft.IdentityModel.Tokens
- Microsoft.AspNetCore.Identity

**Prensip:**
- ✅ Reusable components
- ✅ No business logic
- ✅ Framework independent (mümkünse)

---

### 3. **wixi.DataAccess (Data Access Layer)**

**Sorumluluk:** Veritabanı işlemleri, DbContext, migrations.

```
wixi.DataAccess/
├── Concrete/
│   └── EntityFramework/
│       └── Contexts/
│           └── WixiDbContext.cs    # EF Core DbContext
│
└── Migrations/
    ├── 20251028092939_InitialCreate.cs
    └── WixiDbContextModelSnapshot.cs
```

**Bağımlılıklar:**
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- wixi.Entities

**Prensip:**
- ✅ Repository pattern (optional, EF Core is already UoW)
- ✅ Generic repository (gelecekte eklenebilir)
- ❌ No business logic

**Eksikler:**
- ❌ Generic Repository pattern yok
- ❌ Unit of Work pattern yok
- ❌ Specification pattern yok

---

### 4. **wixi.Business (Business Logic Layer)**

**Sorumluluk:** İş kuralları, business logic, servis implementasyonları.

```
wixi.Business/
├── Abstract/
│   └── IAuthService.cs         # Authentication interface
│
└── Concrete/
    └── AuthManager.cs          # Authentication implementation
```

**Bağımlılıklar:**
- Microsoft.AspNetCore.Identity
- wixi.Core
- wixi.DataAccess
- wixi.Entities

**Prensip:**
- ✅ Interface-based design
- ✅ Dependency Inversion
- ✅ Single Responsibility

**Eksikler:**
- ❌ Validation logic yok (FluentValidation eklenecek)
- ❌ Business exceptions yok
- ❌ Domain events yok
- ❌ CQRS pattern yok (MediatR eklenecek)

---

### 5. **wixi.WebAPI (Presentation Layer)**

**Sorumluluk:** HTTP endpoints, middleware, DI configuration.

```
wixi.WebAPI/
├── Controllers/
│   └── AuthController.cs       # Authentication endpoints
│
├── Extensions/
│   ├── DbContextExt.cs         # DbContext DI
│   ├── IdentityExt.cs          # Identity DI
│   ├── JwtExt.cs               # JWT DI
│   └── ServiceCollectionExt.cs # Services DI
│
├── Program.cs                   # Application entry point
└── appsettings.json            # Configuration
```

**Bağımlılıklar:**
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore
- wixi.Business
- wixi.Core
- wixi.DataAccess
- wixi.Entities

**Prensip:**
- ✅ Thin controllers
- ✅ Dependency Injection
- ✅ Extension methods for clean startup

**Eksikler:**
- ❌ Global exception handler yok
- ❌ API versioning yok
- ❌ Rate limiting yok
- ❌ Response compression yok
- ❌ Middleware pipeline eksik

---

## 🔄 Data Flow

### Typical Request Flow

```
1. HTTP Request
   ↓
2. Middleware Pipeline
   - Authentication
   - Authorization
   - (Logging - EKLENECEK)
   - (Exception Handling - EKLENECEK)
   ↓
3. Controller
   - Route to action
   - Model binding
   - (Validation - EKLENECEK)
   ↓
4. Business Layer
   - Business logic
   - Business rules
   ↓
5. Data Access Layer
   - EF Core
   - Database query
   ↓
6. Database
   - SQL Server
   ↓
7. Response
   - DTO mapping
   - JSON serialization
   - HTTP response
```

---

## 🎯 Design Patterns

### **Mevcut Patterns**

1. **Layered Architecture**
   - Separation of Concerns
   - Dependency Inversion

2. **Dependency Injection**
   - Constructor injection
   - Service lifetime management

3. **Repository Pattern** (implicit via EF Core)
   - DbContext as repository
   - DbSet as collection

4. **DTO Pattern**
   - Data transfer objects
   - Separation of domain and API models

5. **Factory Pattern** (JWT Helper)
   - Token creation
   - Refresh token generation

### **Eklenecek Patterns**

1. **CQRS** (Command Query Responsibility Segregation)
   - Commands (Write operations)
   - Queries (Read operations)
   - MediatR library

2. **Specification Pattern**
   - Reusable query logic
   - Complex filtering

3. **Unit of Work Pattern**
   - Transaction management
   - Multiple repository coordination

4. **Result Pattern**
   - Success/Failure handling
   - Error propagation

5. **Options Pattern** (kısmen var)
   - Configuration management
   - Strongly-typed settings

---

## 🔐 Security Architecture

### **Mevcut Security**

```
┌─────────────────────────────────────────┐
│          HTTP Request                   │
└────────────────┬────────────────────────┘
                 ↓
         ┌───────────────┐
         │  CORS Check   │
         └───────┬───────┘
                 ↓
         ┌───────────────┐
         │ Authentication│ (JWT Bearer)
         └───────┬───────┘
                 ↓
         ┌───────────────┐
         │ Authorization │ (Role-based)
         └───────┬───────┘
                 ↓
         ┌───────────────┐
         │  Controller   │
         └───────────────┘
```

### **Eklenecek Security**

- **Rate Limiting** - DDoS koruması
- **Input Validation** - Injection attacks
- **Security Headers** - XSS, Clickjacking
- **API Key** - Public API'ler için
- **IP Whitelist** - Admin endpoints
- **Audit Logging** - Security events

---

## 📊 Database Schema

### **Identity Tables** (ASP.NET Core Identity)

```sql
AspNetUsers
├── Id (PK)
├── UserName
├── Email
├── PasswordHash
├── FirstName (custom)
├── LastName (custom)
├── RefreshToken (custom)
├── RefreshTokenEndDate (custom)
├── TwoFactorEnabled (custom)
├── TwoFactorCode (custom)
└── TwoFactorCodeExpiration (custom)

AspNetRoles
├── Id (PK)
└── Name

AspNetUserRoles
├── UserId (FK)
└── RoleId (FK)

TokenBlacklist
├── Id (PK)
├── Token
├── BlacklistedAt
└── ExpirationDate
```

### **Eksik Tablolar**

- **AuditLog** - Tüm işlemlerin kaydı
- **EmailQueue** - Email gönderim kuyruğu
- **FileMetadata** - Upload edilen dosyalar
- **Settings** - Uygulama ayarları
- **ApiKey** - API key management

---

## 🚀 Performance Considerations

### **Mevcut**
- ✅ EF Core query optimization (default)
- ✅ Async/await pattern
- ✅ Connection pooling (default)

### **Eksik**
- ❌ Caching strategy
- ❌ Query optimization (indexes)
- ❌ Database connection resilience
- ❌ Lazy loading disabled (best practice)
- ❌ Response compression
- ❌ CDN integration

---

## 🔧 Configuration Management

### **Mevcut**

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "JwtTokenOptions": {
    "Audience": "...",
    "Issuer": "...",
    "AccessTokenExpiration": 60,
    "SecurityKey": "...",
    "RefreshTokenExpiration": 1440
  }
}
```

### **Eklenecek**

```json
{
  "Logging": {
    "Serilog": { ... }
  },
  "Redis": {
    "ConnectionString": "...",
    "InstanceName": "..."
  },
  "Email": {
    "SmtpServer": "...",
    "Port": 587,
    "Username": "...",
    "From": "..."
  },
  "FileStorage": {
    "Provider": "Local|Azure|AWS",
    "Path": "..."
  },
  "RateLimiting": {
    "PermitLimit": 100,
    "Window": "00:01:00"
  }
}
```

---

## 🐳 Docker Architecture

### **Mevcut**

```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Build application

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Run application
```

### **Geliştirilecek**

```yaml
# docker-compose.yml (Full Stack)
services:
  api:
    build: .
    ports:
      - "5045:5045"
    depends_on:
      - db
      - redis
      - rabbitmq
    
  db:
    image: mcr.microsoft.com/mssql/server
    
  redis:
    image: redis:alpine
    
  rabbitmq:
    image: rabbitmq:management
    
  seq:
    image: datalust/seq
```

---

## 📈 Scalability Strategy

### **Horizontal Scaling**

```
                  ┌──────────────┐
                  │ Load Balancer│
                  └───────┬──────┘
         ┌────────────┬───┴───┬────────────┐
         ↓            ↓       ↓            ↓
    ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐
    │ API #1 │  │ API #2 │  │ API #3 │  │ API #N │
    └───┬────┘  └───┬────┘  └───┬────┘  └───┬────┘
        └───────────┴───────────┴────────────┘
                    ↓
            ┌───────────────┐
            │ Redis Cluster │
            └───────────────┘
                    ↓
            ┌───────────────┐
            │   SQL Server  │
            └───────────────┘
```

### **Gereksinimler**
- ✅ Stateless API (mevcut)
- ❌ Distributed cache (Redis eklenecek)
- ❌ Session management (Redis)
- ❌ Centralized logging (Seq/ELK)
- ❌ Health checks (eklenecek)

---

## 🎓 Best Practices

### **✅ Mevcut İyi Pratikler**

1. **Async/Await** - Tüm I/O operations async
2. **Dependency Injection** - Loose coupling
3. **Interface Segregation** - IAuthService
4. **Configuration** - appsettings.json
5. **Migrations** - Code-first approach

### **❌ Eksik Pratikler**

1. **Logging** - Her katmanda log
2. **Error Handling** - Try-catch her yerde
3. **Validation** - Input validation
4. **Documentation** - XML comments
5. **Testing** - Unit & integration tests
6. **Code Review** - PR templates
7. **Git Workflow** - Feature branches

---

## 📚 Sonraki Adımlar

1. **Faz 1:** Logging & Error Handling
2. **Faz 2:** Validation & Security
3. **Faz 3:** Caching & Performance
4. **Faz 4:** Testing & Documentation
5. **Faz 5:** Microservices Ready

Her faz için detaylı implementation guide hazırlanacak.

---

**Son Güncelleme:** 28 Ekim 2025

