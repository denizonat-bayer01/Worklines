# Randevu Sistemi Modülerleştirme Analizi ve Planı

## 📊 Mevcut Durum Analizi

### Frontend (V4/src/pages/Appointment.tsx)
**Özellikler:**
- ✅ 4 adımlı randevu rezervasyon süreci
- ✅ Team member seçimi
- ✅ Tarih seçimi (Calendar component)
- ✅ Saat seçimi (9:00-17:00 arası)
- ✅ İletişim bilgileri formu
- ✅ Çok dilli destek (DE, TR, EN)
- ✅ Responsive tasarım

**Eksikler:**
- ❌ Backend entegrasyonu YOK
- ❌ Veritabanına kayıt YOK
- ❌ Email bildirimi YOK
- ❌ Randevu yönetimi (listeleme, iptal, güncelleme) YOK
- ❌ Admin paneli YOK

**Bağımlılıklar:**
- `TeamService` - Team members için
- `Header`, `Footer` component'leri
- `useLanguage` context
- UI component'leri (Button, Card, Calendar, Input, etc.)
- `BASE_URL` config

### Backend
**Mevcut:**
- ✅ TeamMember entity var
- ✅ Email sistemi var
- ✅ Form submission sistemi var (referans olarak)

**Eksikler:**
- ❌ Appointment entity YOK
- ❌ Appointment controller YOK
- ❌ Appointment service YOK
- ❌ Appointment DTOs YOK

---

## 🎯 Modülerleştirme Hedefleri

### 1. Taşınabilirlik
- ✅ Başka projeye kolayca kopyalanabilir
- ✅ Minimum bağımlılık
- ✅ Standalone çalışabilir

### 2. Bağımsızlık
- ✅ Kendi entity'leri
- ✅ Kendi API endpoint'leri
- ✅ Kendi frontend component'leri

### 3. Entegrasyon Kolaylığı
- ✅ Açık API interface'leri
- ✅ Configurable (yapılandırılabilir)
- ✅ Hook-based integration

---

## 📁 Önerilen Modül Yapısı

### Backend Modül Yapısı
```
wixi.backend/
├── wixi.Appointments/                    # Yeni modül projesi
│   ├── Entities/
│   │   ├── Appointment.cs
│   │   ├── AppointmentSlot.cs
│   │   └── AppointmentSettings.cs
│   ├── DTOs/
│   │   ├── AppointmentCreateDto.cs
│   │   ├── AppointmentResponseDto.cs
│   │   ├── AppointmentUpdateDto.cs
│   │   └── AppointmentSlotDto.cs
│   ├── Interfaces/
│   │   └── IAppointmentService.cs
│   ├── Services/
│   │   ├── AppointmentService.cs
│   │   └── AppointmentNotificationService.cs
│   ├── Controllers/
│   │   ├── AppointmentsController.cs       # Public API
│   │   └── AdminAppointmentsController.cs  # Admin API
│   ├── Validators/
│   │   └── AppointmentValidator.cs
│   └── wixi.Appointments.csproj
```

### Frontend Modül Yapısı
```
appointment-module/
├── src/
│   ├── components/
│   │   ├── AppointmentBooking/
│   │   │   ├── AppointmentWizard.tsx
│   │   │   ├── Step1SelectConsultant.tsx
│   │   │   ├── Step2SelectDate.tsx
│   │   │   ├── Step3SelectTime.tsx
│   │   │   └── Step4ContactInfo.tsx
│   │   ├── AppointmentCalendar/
│   │   │   ├── AvailableSlots.tsx
│   │   │   └── TimeSlotPicker.tsx
│   │   ├── AppointmentConfirmation/
│   │   │   └── ConfirmationCard.tsx
│   │   └── AppointmentList/
│   │       ├── AppointmentCard.tsx
│   │       └── AppointmentFilters.tsx
│   ├── services/
│   │   ├── AppointmentService.ts
│   │   └── AppointmentApiClient.ts
│   ├── hooks/
│   │   ├── useAppointment.ts
│   │   ├── useAvailableSlots.ts
│   │   └── useAppointmentForm.ts
│   ├── types/
│   │   ├── Appointment.ts
│   │   ├── AppointmentSlot.ts
│   │   └── AppointmentConfig.ts
│   ├── utils/
│   │   ├── dateUtils.ts
│   │   └── validationUtils.ts
│   ├── config/
│   │   └── appointmentConfig.ts
│   └── index.ts                          # Public exports
├── package.json
├── tsconfig.json
└── README.md
```

---

## 🔧 Implementation Plan

### Phase 1: Backend Modülü Oluşturma

#### 1.1 Entity'ler
```csharp
// Appointment.cs
public class Appointment
{
    public int Id { get; set; }
    public string AppointmentNumber { get; set; } // "APT-2024-001234"
    
    // Consultant/Team Member
    public int TeamMemberId { get; set; }
    public virtual TeamMember TeamMember { get; set; }
    
    // Customer Info
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public string? CustomerMessage { get; set; }
    
    // Appointment Details
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
    
    // Status
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Notifications
    public bool EmailSent { get; set; }
    public DateTime? EmailSentAt { get; set; }
    public bool ReminderSent { get; set; }
    public DateTime? ReminderSentAt { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

public enum AppointmentStatus
{
    Pending = 1,      // Beklemede
    Confirmed = 2,    // Onaylandı
    Completed = 3,    // Tamamlandı
    Cancelled = 4,    // İptal edildi
    NoShow = 5        // Gelmeyen
}

// AppointmentSlot.cs (Müsait saatler için)
public class AppointmentSlot
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    public virtual TeamMember TeamMember { get; set; }
    
    public DateTime SlotDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    public bool IsBlocked { get; set; } = false;
    public string? BlockReason { get; set; }
    
    public int? AppointmentId { get; set; }
    public virtual Appointment? Appointment { get; set; }
}

// AppointmentSettings.cs (Konfigürasyon)
public class AppointmentSettings
{
    public int Id { get; set; }
    public int TeamMemberId { get; set; }
    
    // Working Hours
    public TimeSpan? MondayStart { get; set; }
    public TimeSpan? MondayEnd { get; set; }
    public TimeSpan? TuesdayStart { get; set; }
    public TimeSpan? TuesdayEnd { get; set; }
    // ... diğer günler
    
    // Default Settings
    public int SlotDurationMinutes { get; set; } = 60;
    public int AdvanceBookingDays { get; set; } = 30;
    public int CancellationHours { get; set; } = 24;
    
    // Pricing
    public decimal DefaultPrice { get; set; } = 50.00m;
    public string Currency { get; set; } = "EUR";
}
```

#### 1.2 Service Interface
```csharp
public interface IAppointmentService
{
    // Public APIs
    Task<AppointmentResponseDto> CreateAppointmentAsync(AppointmentCreateDto createDto);
    Task<List<AppointmentSlotDto>> GetAvailableSlotsAsync(int teamMemberId, DateTime date);
    Task<AppointmentResponseDto> GetAppointmentByNumberAsync(string appointmentNumber);
    Task<bool> CancelAppointmentAsync(string appointmentNumber, string reason);
    
    // Admin APIs
    Task<List<AppointmentResponseDto>> GetAllAppointmentsAsync(AppointmentFilterDto filter);
    Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status);
    Task<bool> DeleteAppointmentAsync(int appointmentId);
    Task<List<AppointmentResponseDto>> GetAppointmentsByTeamMemberAsync(int teamMemberId, DateTime? startDate, DateTime? endDate);
    
    // Settings
    Task<AppointmentSettingsDto> GetAppointmentSettingsAsync(int teamMemberId);
    Task<AppointmentSettingsDto> UpdateAppointmentSettingsAsync(int teamMemberId, AppointmentSettingsDto settings);
}
```

#### 1.3 Controller
```csharp
[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreateDto createDto)
    
    [HttpGet("available-slots/{teamMemberId}")]
    public async Task<IActionResult> GetAvailableSlots(int teamMemberId, [FromQuery] DateTime date)
    
    [HttpGet("{appointmentNumber}")]
    public async Task<IActionResult> GetAppointment(string appointmentNumber)
    
    [HttpPost("{appointmentNumber}/cancel")]
    public async Task<IActionResult> CancelAppointment(string appointmentNumber, [FromBody] CancelAppointmentDto dto)
}

[ApiController]
[Route("api/admin/appointments")]
[Authorize(Roles = "Admin")]
public class AdminAppointmentsController : ControllerBase
{
    // Admin endpoints
}
```

### Phase 2: Frontend Modülü Oluşturma

#### 2.1 Service Layer
```typescript
// AppointmentService.ts
export interface AppointmentCreateDto {
  teamMemberId: number;
  appointmentDate: string; // ISO date
  startTime: string; // HH:mm
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  customerMessage?: string;
}

export interface AppointmentResponse {
  id: number;
  appointmentNumber: string;
  teamMember: TeamMember;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  price: number;
  currency: string;
  status: AppointmentStatus;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
}

export class AppointmentService {
  constructor(private apiBaseUrl: string, private apiClient: ApiClient) {}
  
  async createAppointment(dto: AppointmentCreateDto): Promise<AppointmentResponse>
  async getAvailableSlots(teamMemberId: number, date: Date): Promise<TimeSlot[]>
  async getAppointment(appointmentNumber: string): Promise<AppointmentResponse>
  async cancelAppointment(appointmentNumber: string, reason: string): Promise<boolean>
}
```

#### 2.2 Component Structure
```typescript
// AppointmentWizard.tsx - Ana component
export interface AppointmentWizardProps {
  teamMembers: TeamMember[];
  onComplete?: (appointment: AppointmentResponse) => void;
  onCancel?: () => void;
  config?: AppointmentConfig;
}

export const AppointmentWizard: React.FC<AppointmentWizardProps> = ({
  teamMembers,
  onComplete,
  onCancel,
  config
}) => {
  // 4 adımlı wizard logic
};

// Config interface
export interface AppointmentConfig {
  apiBaseUrl: string;
  defaultPrice?: number;
  availableTimeSlots?: string[];
  workingDays?: number[]; // 0=Sunday, 1=Monday, etc.
  minAdvanceBookingDays?: number;
  maxAdvanceBookingDays?: number;
  currency?: string;
  language?: 'de' | 'tr' | 'en';
  translations?: AppointmentTranslations;
}
```

#### 2.3 Standalone Usage
```typescript
// Kullanım örneği
import { AppointmentWizard, AppointmentService } from '@worklines/appointment-module';

const appointmentService = new AppointmentService('https://api.worklines.de', apiClient);

<AppointmentWizard
  teamMembers={teamMembers}
  config={{
    apiBaseUrl: 'https://api.worklines.de',
    defaultPrice: 50,
    currency: 'EUR',
    language: 'de',
    availableTimeSlots: ['09:00', '10:00', '11:00', ...],
    workingDays: [1, 2, 3, 4, 5], // Monday to Friday
  }}
  onComplete={(appointment) => {
    console.log('Appointment created:', appointment);
  }}
/>
```

---

## 🔗 Entegrasyon Stratejisi

### Mevcut Projeye Entegrasyon
1. **Backend**: `wixi.Appointments` projesini solution'a ekle
2. **Database**: Migration oluştur ve çalıştır
3. **Dependency Injection**: `Program.cs`'de servisleri kaydet
4. **Frontend**: Modülü npm package veya local package olarak ekle

### Başka Projeye Taşıma
1. **Backend**: 
   - `wixi.Appointments` klasörünü kopyala
   - Entity Framework DbContext'i yeni projeye uyarla
   - Connection string'i güncelle
   - Migration'ları çalıştır

2. **Frontend**:
   - `appointment-module` klasörünü kopyala
   - `package.json` dependencies'leri kontrol et
   - API base URL'i yapılandır
   - Gerekli UI component library'lerini ekle (shadcn/ui gibi)

---

## 📦 Bağımlılık Minimizasyonu

### Backend Bağımlılıkları
- ✅ Entity Framework Core (zaten var)
- ✅ TeamMember entity (optional - interface ile soyutlanabilir)
- ✅ Email service (interface ile soyutlanabilir)
- ❌ Diğer modüllerden bağımsız

### Frontend Bağımlılıkları
**Core Dependencies:**
- React
- React Router (optional)
- Date-fns (tarih işlemleri için)

**UI Dependencies (Configurable):**
- shadcn/ui components (veya başka UI library)
- lucide-react (icons)

**External Dependencies (Injectable):**
- API client (fetch/axios)
- Language/Translation service
- Notification service

---

## 🚀 Migration Checklist

### Backend
- [ ] `wixi.Appointments` projesi oluştur
- [ ] Entity'leri oluştur (Appointment, AppointmentSlot, AppointmentSettings)
- [ ] DTOs oluştur
- [ ] IAppointmentService interface'i oluştur
- [ ] AppointmentService implementation
- [ ] Controllers oluştur (Public + Admin)
- [ ] Validators oluştur
- [ ] DbContext'e entity'leri ekle
- [ ] Migration oluştur ve çalıştır
- [ ] Dependency Injection yapılandır
- [ ] Email bildirimleri entegre et
- [ ] Unit testler yaz

### Frontend
- [ ] `appointment-module` klasör yapısı oluştur
- [ ] Type definitions oluştur
- [ ] AppointmentService oluştur
- [ ] Component'leri oluştur (Wizard, Calendar, etc.)
- [ ] Hooks oluştur (useAppointment, useAvailableSlots)
- [ ] Config system oluştur
- [ ] Translation system entegre et
- [ ] Standalone example oluştur
- [ ] Documentation yaz
- [ ] Package.json hazırla

### Integration
- [ ] Mevcut projeye backend modülünü ekle
- [ ] Mevcut projeye frontend modülünü ekle
- [ ] API routes'ları test et
- [ ] Frontend component'lerini test et
- [ ] End-to-end test yap

---

## 📝 Örnek Kullanım Senaryoları

### Senaryo 1: Mevcut Projede Kullanım
```typescript
// V4/src/pages/Appointment.tsx (güncellenmiş)
import { AppointmentWizard } from '../modules/appointment';
import { useAppointmentService } from '../modules/appointment/hooks';

export default function AppointmentPage() {
  const appointmentService = useAppointmentService();
  const { teamMembers } = useTeamMembers();
  
  return (
    <AppointmentWizard
      teamMembers={teamMembers}
      config={{
        apiBaseUrl: API_ROUTES.BASE_URL,
        language: language,
      }}
      onComplete={(appointment) => {
        // Redirect veya notification
      }}
    />
  );
}
```

### Senaryo 2: Başka Projede Kullanım
```typescript
// Yeni proje
import { AppointmentWizard, AppointmentService } from '@worklines/appointment-module';
import { createApiClient } from './api-client';

const apiClient = createApiClient('https://api.example.com');
const appointmentService = new AppointmentService('https://api.example.com', apiClient);

function MyAppointmentPage() {
  return (
    <AppointmentWizard
      teamMembers={consultants}
      config={{
        apiBaseUrl: 'https://api.example.com',
        defaultPrice: 75,
        currency: 'USD',
        language: 'en',
      }}
    />
  );
}
```

---

## 🎨 UI Component Bağımsızlığı

### Strateji: Component Adapter Pattern
```typescript
// appointment-module/src/components/adapters/ButtonAdapter.tsx
export interface ButtonAdapterProps {
  children: React.ReactNode;
  onClick?: () => void;
  variant?: 'primary' | 'secondary' | 'outline';
  disabled?: boolean;
}

// Default implementation (shadcn/ui)
export const DefaultButtonAdapter: React.FC<ButtonAdapterProps> = (props) => {
  return <Button {...props} />;
};

// Custom adapter örneği
export const CustomButtonAdapter: React.FC<ButtonAdapterProps> = (props) => {
  return <MyCustomButton {...mapProps(props)} />;
};

// Wizard component'inde adapter kullan
<ButtonAdapter variant="primary" onClick={handleNext}>
  Continue
</ButtonAdapter>
```

---

## 🔒 Güvenlik ve Validasyon

### Backend
- ✅ Input validation (FluentValidation)
- ✅ SQL injection protection (EF Core)
- ✅ XSS protection
- ✅ Rate limiting (appointment creation için)
- ✅ Email verification (optional)

### Frontend
- ✅ Form validation
- ✅ XSS protection
- ✅ CSRF protection (token-based)
- ✅ Input sanitization

---

## 📊 Database Schema

```sql
-- Appointments Table
CREATE TABLE Appointments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AppointmentNumber NVARCHAR(50) UNIQUE NOT NULL,
    TeamMemberId INT NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerEmail NVARCHAR(100) NOT NULL,
    CustomerPhone NVARCHAR(20) NOT NULL,
    CustomerMessage NVARCHAR(1000),
    AppointmentDate DATETIME2 NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Currency NVARCHAR(3) DEFAULT 'EUR',
    Status INT NOT NULL DEFAULT 1,
    CancellationReason NVARCHAR(500),
    CancelledAt DATETIME2,
    EmailSent BIT DEFAULT 0,
    EmailSentAt DATETIME2,
    ReminderSent BIT DEFAULT 0,
    ReminderSentAt DATETIME2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    ConfirmedAt DATETIME2,
    FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id)
);

-- AppointmentSlots Table
CREATE TABLE AppointmentSlots (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TeamMemberId INT NOT NULL,
    SlotDate DATETIME2 NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsAvailable BIT DEFAULT 1,
    IsBlocked BIT DEFAULT 0,
    BlockReason NVARCHAR(500),
    AppointmentId INT,
    FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id),
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id),
    UNIQUE (TeamMemberId, SlotDate, StartTime)
);

-- AppointmentSettings Table
CREATE TABLE AppointmentSettings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TeamMemberId INT UNIQUE NOT NULL,
    SlotDurationMinutes INT DEFAULT 60,
    AdvanceBookingDays INT DEFAULT 30,
    CancellationHours INT DEFAULT 24,
    DefaultPrice DECIMAL(10,2) DEFAULT 50.00,
    Currency NVARCHAR(3) DEFAULT 'EUR',
    MondayStart TIME,
    MondayEnd TIME,
    -- ... diğer günler
    FOREIGN KEY (TeamMemberId) REFERENCES TeamMembers(Id)
);
```

---

## 🧪 Testing Strategy

### Backend Tests
- Unit tests (Service layer)
- Integration tests (Controller + Database)
- API tests (End-to-end)

### Frontend Tests
- Component tests (React Testing Library)
- Hook tests
- Integration tests
- E2E tests (Playwright/Cypress)

---

## 📚 Documentation Requirements

1. **README.md**: Modül açıklaması, kurulum, kullanım
2. **API Documentation**: Swagger/OpenAPI
3. **Component Documentation**: Storybook (optional)
4. **Migration Guide**: Mevcut projeden taşıma rehberi
5. **Configuration Guide**: Yapılandırma seçenekleri
6. **Examples**: Kullanım örnekleri

---

## ⏱️ Tahmini Süre

- **Backend Modülü**: 3-5 gün
- **Frontend Modülü**: 4-6 gün
- **Entegrasyon**: 2-3 gün
- **Testing**: 2-3 gün
- **Documentation**: 1-2 gün

**Toplam**: 12-19 gün

---

## 🎯 Sonuç

Bu modülerleştirme planı ile:
1. ✅ Randevu sistemi bağımsız bir modül haline gelir
2. ✅ Başka projelere kolayca taşınabilir
3. ✅ Bakımı ve geliştirmesi kolaylaşır
4. ✅ Test edilebilirliği artar
5. ✅ Yeniden kullanılabilirliği maksimize eder

