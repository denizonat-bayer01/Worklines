# Global Error Handling

Bu doküman, API'de tek tip ve izlenebilir hata yönetimini açıklar.

## Amaç
- Tüm hataları tek noktadan yakalamak
- Tutarlı JSON gövdesi döndürmek
- Log/izleme kolaylığı (TraceId)

## Mimarî
- Middleware: `wixi.WebAPI.Middleware.GlobalExceptionMiddleware`
- Model: `wixi.WebAPI.Models.ErrorResponse`
- Özel Exceptionlar: `wixi.Core.Exceptions.*`

```json
{
  "traceId": "0HMI...:00000001",
  "statusCode": 404,
  "message": "Kayıt bulunamadı",
  "detail": null,
  "errors": null,
  "timestampUtc": "2025-11-03T16:00:00Z",
  "path": "/api/resource/1"
}
```

## Status Mapping
- ValidationException → 400
- BusinessException → 422
- NotFoundException → 404
- UnauthorizedException → 401
- Diğer/Unhandled → 500 (detail sadece dev/log için)

## Kullanım
```csharp
using wixi.Core.Exceptions;

if (entity == null)
    throw new NotFoundException("Kayıt bulunamadı");

if (!isAllowed)
    throw new BusinessException("İş kuralı ihlali");
```

## Kurulum
Program.cs:
```csharp
using wixi.WebAPI.Middleware;
...
app.UseGlobalException();
```

## Logging
- 5xx → LogError
- 4xx (handled) → LogWarning

## En İyi Pratikler
- Controller içinde try/catch yazma (global katman yeterli)
- İş kurallarını exception ile bildir
- İstemcide `message` göster, `detail` debug içindir


