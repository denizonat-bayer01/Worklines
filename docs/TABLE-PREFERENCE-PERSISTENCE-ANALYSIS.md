# Kullanıcı Bazlı Tablo Ayarları Kalıcılığı — Analiz

## 1. Amaç
Admin panelindeki modern DataTable bileşeninde kullanıcıların yaptığı kişiselleştirmelerin (kolon görünürlüğü, kolon sırası, sayfa boyutu, filtreler vb.) oturumlar arasında korunması isteniyor. Kullanıcı sisteme tekrar girdiğinde, bıraktığı ayarlarla aynı tablo düzenini görmeli.

Bu doküman, **backend odaklı** tasarım ve entegrasyon yaklaşımını ortaya koyar; frontend ile nasıl konuşacağı da özetlenir.

---

## 2. Fonksiyonel Gereksinimler
1. **Kapsamlı Kayıt**: En azından şu alanlar saklanmalı:
   - `visibleColumns`, `columnOrder`
   - `pageSize`
   - `sortConfig`
   - `columnFilters`
   - (opsiyonel) `selectedRows` gibi geçici durumlar saklanmaz.
2. **Tablo Kimliği**: Aynı kullanıcı farklı sayfalarda farklı tablolar kullanabilir. Bu yüzden **unique table key** şart (örn. `admin-contact-submissions`).
3. **Kullanıcı Bazlı**: Ayarlar sadece ilgili kullanıcı tarafından görülebilir. Admin başka kullanıcının ayarını değiştirmemeli.
4. **Otomatik Uygulama**: Frontend tablo ilk render edildiğinde backenden ayarı almalı; yoksa varsayılan ayarla başlamalı.
5. **Gerçek Zamanlı Güncelleme**: Kullanıcı tablo ayarını değiştirdiğinde (ör. kolon gizlediğinde) backend’e PATCH/PUT çağrısı gider ve yeni durum saklanır.

---

## 3. Veri Modeli
`wixi.backendV2` tarafında yeni bir tablo ve entity önerisi:

```csharp
public class UserTablePreference
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TableKey { get; set; } = string.Empty; // örn. admin-contact-submissions
    public string VisibleColumns { get; set; } = "[]";   // JSON array (string list)
    public string ColumnOrder { get; set; } = "[]";      // JSON array
    public string ColumnFilters { get; set; } = "{}";    // JSON object
    public string SortConfig { get; set; } = "{}";       // JSON object { key, direction }
    public int PageSize { get; set; } = 20;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
}
```

**Veritabanı şeması** (`wixi_UserTablePreferences`):
- `UserId + TableKey` unique index → aynı tablo için tek kayıt.
- JSON alanlar NVARCHAR(MAX) olarak saklanabilir; SQL Server JSON sorgularını destekliyor.

---

## 4. Backend Katmanı

### 4.1. DTO’lar
```csharp
public class TablePreferenceDto
{
    public string TableKey { get; set; } = string.Empty;
    public List<string> VisibleColumns { get; set; } = new();
    public List<string> ColumnOrder { get; set; } = new();
    public Dictionary<string, object>? ColumnFilters { get; set; }
    public SortConfigDto? SortConfig { get; set; }
    public int PageSize { get; set; } = 20;
}

public class UpdateTablePreferenceDto : TablePreferenceDto {}
```

### 4.2. Service
`ITablePreferenceService`
- `Task<TablePreferenceDto?> GetAsync(int userId, string tableKey)`
- `Task<TablePreferenceDto> UpsertAsync(int userId, UpdateTablePreferenceDto dto)`

İçerik JSON serileştirilerek saklanacak. MapStruct benzeri manual map:
```csharp
entity.VisibleColumns = JsonSerializer.Serialize(dto.VisibleColumns ?? new());
```

### 4.3. Controller
`[Route("api/v{version:apiVersion}/user/table-preferences")]`

| HTTP | Endpoint | Açıklama |
|------|----------|----------|
| GET  | `/tables/{tableKey}` | Mevcut kullanıcının ayarını döner (404 yerine empty default). |
| PUT  | `/tables/{tableKey}` | Body’deki dto’yu kaydeder/ günceller. |

`UserId`, token’dan alınır (`ClaimTypes.NameIdentifier`). Yalnızca Authenticated kullanıcılar erişmeli.

### 4.4. Validation & Güvenlik
- `tableKey` whitelist (örn. enum) ile sınırlandırılabilir, aksi halde rastgele key ile DB şişer.
- Payload boyutu limitlenmeli (ör. 32 KB).
- Audit log’a gerek yok; istenirse `UpdatedAt` loglanıyor zaten.

---

## 5. Frontend Entegrasyonu (Özet)

1. **TableKey Standardizasyonu**  
   DataTable bileşeni `tableKey` props’u almalı. Örn. `contact-submissions`.

2. **Yükleme**  
   - Sayfa açılışında `GET /user/table-preferences/tables/{tableKey}` çağrılır.
   - Dönüş gelirse DataTable başlangıç state’i bu JSON ile set edilir.

3. **Kaydetme Tetikleyicileri**  
   - Kolon sırası değiştiğinde
   - Kolon görünürlüğü/gizlemesi
   - Filtre veya sıralama değişince
   - Sayfa boyutu değişince

   Bu noktalarda throttle/debounce (örn. 1 saniye) ile `PUT` atılır.

4. **Hata Yönetimi**  
   - Kaydetme başarısız olursa kullanıcıya toast gösterilebilir, ama tablo state’i bozulmamalı.

---

## 6. Geliştirme Adımları
1. **Migration**
   - `AddUserTablePreferences` migration’ı oluşturulup apply edilir.

2. **Entity + DbContext**
   - `WixiDbContext` içine DbSet’i ekle.
   - unique index ve fk (`UserId`) tanımla.

3. **Service/Interface/Controller**
   - `TablePreferenceService` (Scoped).
   - Controller’da GET/PUT.
   - `BusinessServiceExtensions`’de register.

4. **Frontend**
   - DataTable’a `tableKey` ve `initialPreferences` propsları.
   - `tablePreferenceService` (GET/PUT).
   - State değişikliklerinde debounce ile save.

5. **Test**
   - API için integration test (isteğe bağlı).
   - Frontend tarafında jest/cypress ile smoke test: ayarlar kaydediliyor mu?

---

## 7. İleriki Genişletmeler
- **Tenant / Client bazlı** tablo ayarı: İleride client kullanıcıları için de açılırsa `TenantId` benzeri alan eklenebilir.
- **Şema Sürümü**: Tablo kolonları değişirse eski kayıtların resetlenmesi gerekebilir. DTO’ya `SchemaVersion` eklenip mismatch durumunda default’a dönülebilir.
- **Global Varsayılanlar**: Kullanıcı kayıtlı ayar vermemişse, rol bazlı varsayılanlar tanımlanabilir.

---

## 8. Özet
Bu tasarım ile:
- Backend’de hafif bir JSON storage yapısıyla tüm tablo kişiselleştirmeleri saklanır.
- Frontend, tablo state değişikliklerinde küçük API çağrıları yaparak kullanıcı deneyimini korur.
- Yeni tablo eklemek için sadece benzersiz `tableKey`’i kullanmak yeterli olur; yeniden kullanılabilir bir altyapı oluşturulur.

