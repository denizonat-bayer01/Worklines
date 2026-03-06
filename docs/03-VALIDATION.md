# Input Validation

Bu bölümde model doğrulama yaklaşımımızı özetliyoruz.

## Geçici Kurulum (Hızlı Başlangıç)
- ASP.NET `[ApiController]` + `ModelState`
- Global `ModelValidationFilter` hatalı durumlarda `ValidationException` fırlatır
- Hatalar `ErrorResponse.Errors` içine sözlük olarak yazılır

Örnek hata:
```json
{
  "statusCode": 400,
  "message": "Doğrulama hatası",
  "errors": {
    "Email": ["Email zorunludur"],
    "Password": ["Şifre en az 8 karakter olmalı"]
  }
}
```

## Planlanan Adım: FluentValidation
- Paketler: `FluentValidation.AspNetCore`
- `AddFluentValidationAutoValidation()`
- Validator örnekleri:
  - `UserForLoginDtoValidator`
  - `UserForRegisterDtoValidator`


