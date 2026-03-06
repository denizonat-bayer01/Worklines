-- Tüm Project References'ları aktif yap
UPDATE ProjectReferences 
SET IsActive = 1,
    UpdatedAt = GETDATE(),
    UpdatedBy = 'System'
WHERE IsActive = 0;

-- Kontrol et
SELECT Id, Title, IsActive, IsFeatured, DocumentImageUrl 
FROM ProjectReferences;

