-- Admin rolünü oluştur
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Name, NormalizedName)
    VALUES ('Admin', 'ADMIN')
END

-- Admin kullanıcısını bul ve Admin rolünü ata
DECLARE @UserId INT
DECLARE @RoleId INT

-- Email'e göre kullanıcıyı bul (değiştir)
SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'admin@worklines.de'

-- Admin rolünün ID'sini al
SELECT @RoleId = Id FROM AspNetRoles WHERE Name = 'Admin'

-- Kullanıcıya Admin rolünü ata (eğer yoksa)
IF NOT EXISTS (SELECT * FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@UserId, @RoleId)
END

-- Kontrol için
SELECT 
    u.Email,
    r.Name as RoleName
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.RoleId
WHERE u.Email = 'admin@worklines.de'

