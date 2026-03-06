-- ============================================
-- TEST DATA CLEANUP SCRIPT
-- ============================================
-- Bu script, foreign key kısıtlamaları nedeniyle silinemeyen test verilerini temizler
-- DİKKAT: Bu script tüm test verilerini siler! Production veritabanında kullanmayın!
-- ============================================

PRINT 'Starting test data cleanup...'
GO

-- Foreign key constraint'leri geçici olarak devre dışı bırak
PRINT 'Disabling foreign key constraints...'
GO

-- Tüm foreign key constraint'leri bul ve devre dışı bırak
DECLARE @sql NVARCHAR(MAX) = ''
SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
       ' NOCHECK CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE is_disabled = 0

EXEC sp_executesql @sql
PRINT 'Foreign key constraints disabled.'
GO

-- ============================================
-- 1. SUPPORT SYSTEM (En alt seviye - bağımlılık yok)
-- ============================================
PRINT 'Cleaning Support Messages...'
DELETE FROM [dbo].[wixi_SupportMessage]
PRINT 'Cleaned Support Messages'

PRINT 'Cleaning Support Tickets...'
DELETE FROM [dbo].[wixi_SupportTicket]
PRINT 'Cleaned Support Tickets'

-- ============================================
-- 2. NOTIFICATIONS
-- ============================================
PRINT 'Cleaning Notifications...'
DELETE FROM [dbo].[wixi_Notification]
PRINT 'Cleaned Notifications'

-- ============================================
-- 3. APPLICATION SYSTEM (Application'a bağlı veriler)
-- ============================================
PRINT 'Cleaning Application Histories...'
DELETE FROM [dbo].[wixi_ApplicationHistory]
PRINT 'Cleaned Application Histories'

PRINT 'Cleaning Application Sub-Steps...'
DELETE FROM [dbo].[wixi_ApplicationSubStep]
PRINT 'Cleaned Application Sub-Steps'

PRINT 'Cleaning Application Steps...'
DELETE FROM [dbo].[wixi_ApplicationStep]
PRINT 'Cleaned Application Steps'

PRINT 'Cleaning Applications...'
DELETE FROM [dbo].[wixi_Application]
PRINT 'Cleaned Applications'

-- ============================================
-- 4. DOCUMENT SYSTEM (Document'a bağlı veriler)
-- ============================================
PRINT 'Cleaning Document Reviews...'
DELETE FROM [dbo].[wixi_DocumentReview]
PRINT 'Cleaned Document Reviews'

PRINT 'Cleaning Documents...'
DELETE FROM [dbo].[wixi_Document]
PRINT 'Cleaned Documents'

PRINT 'Cleaning File Storage...'
DELETE FROM [dbo].[wixi_FileStorage]
PRINT 'Cleaned File Storage'

-- ============================================
-- 5. CLIENT SYSTEM (Client'a bağlı veriler)
-- ============================================
PRINT 'Cleaning Education Info...'
DELETE FROM [dbo].[wixi_EducationInfo]
PRINT 'Cleaned Education Info'

PRINT 'Cleaning Pending Client Codes...'
DELETE FROM [dbo].[wixi_PendingClientCode]
PRINT 'Cleaned Pending Client Codes'

PRINT 'Cleaning Clients...'
DELETE FROM [dbo].[wixi_Client]
PRINT 'Cleaned Clients'

-- ============================================
-- 6. FORM SUBMISSIONS (Opsiyonel - test verileri için)
-- ============================================
PRINT 'Cleaning Employee Form Submissions...'
DELETE FROM [dbo].[EmployeeFormSubmissions]
PRINT 'Cleaned Employee Form Submissions'

PRINT 'Cleaning Employer Form Submissions...'
DELETE FROM [dbo].[EmployerFormSubmissions]
PRINT 'Cleaned Employer Form Submissions'

PRINT 'Cleaning Contact Form Submissions...'
DELETE FROM [dbo].[ContactFormSubmissions]
PRINT 'Cleaned Contact Form Submissions'

-- ============================================
-- 7. EMAIL LOGS (Opsiyonel - test verileri için)
-- ============================================
PRINT 'Cleaning Email Logs...'
DELETE FROM [dbo].[EmailLogs]
PRINT 'Cleaned Email Logs'

-- ============================================
-- 8. USER PREFERENCES (Opsiyonel)
-- ============================================
PRINT 'Cleaning User Preferences...'
DELETE FROM [dbo].[UserPreferences]
PRINT 'Cleaned User Preferences'

-- ============================================
-- 9. APPLICATION LOGS (Opsiyonel)
-- ============================================
PRINT 'Cleaning Application Logs...'
DELETE FROM [dbo].[ApplicationLogs]
PRINT 'Cleaned Application Logs'

-- ============================================
-- 10. USERS (Identity - Dikkatli olun!)
-- ============================================
-- NOT: User'ları silmek istiyorsanız, önce role'lerden çıkarın
PRINT 'Cleaning User Roles...'
DELETE FROM [dbo].[AspNetUserRoles]
PRINT 'Cleaned User Roles'

PRINT 'Cleaning User Claims...'
DELETE FROM [dbo].[AspNetUserClaims]
PRINT 'Cleaned User Claims'

PRINT 'Cleaning User Logins...'
DELETE FROM [dbo].[AspNetUserLogins]
PRINT 'Cleaned User Logins'

PRINT 'Cleaning User Tokens...'
DELETE FROM [dbo].[AspNetUserTokens]
PRINT 'Cleaned User Tokens'

-- User'ları sil (sadece test user'ları - admin user'ları koruyun)
-- DİKKAT: Bu tüm user'ları siler! İsterseniz WHERE koşulu ekleyebilirsiniz
PRINT 'Cleaning Users (excluding admin users)...'
DELETE FROM [dbo].[AspNetUsers]
WHERE Id NOT IN (
    SELECT UserId FROM [dbo].[AspNetUserRoles] 
    WHERE RoleId IN (SELECT Id FROM [dbo].[AspNetRoles] WHERE Name IN ('Admin', 'SuperAdmin'))
)
-- Yukarıdaki sorgu çalışmazsa, direkt silmek için:
-- DELETE FROM [dbo].[AspNetUsers]
PRINT 'Cleaned Users'

-- ============================================
-- Foreign key constraint'leri tekrar aktif et
-- ============================================
PRINT 'Re-enabling foreign key constraints...'
GO

DECLARE @sql2 NVARCHAR(MAX) = ''
SELECT @sql2 = @sql2 + 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
       ' CHECK CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE is_disabled = 1

EXEC sp_executesql @sql2
PRINT 'Foreign key constraints re-enabled.'
GO

-- ============================================
-- VERİTABANI İSTATİSTİKLERİ
-- ============================================
PRINT ''
PRINT '============================================'
PRINT 'CLEANUP COMPLETED!'
PRINT '============================================'
PRINT 'Remaining data counts:'
PRINT ''

SELECT 'Clients' AS TableName, COUNT(*) AS Count FROM [dbo].[wixi_Client]
UNION ALL
SELECT 'Documents', COUNT(*) FROM [dbo].[wixi_Document]
UNION ALL
SELECT 'Applications', COUNT(*) FROM [dbo].[wixi_Application]
UNION ALL
SELECT 'Support Tickets', COUNT(*) FROM [dbo].[wixi_SupportTicket]
UNION ALL
SELECT 'Users', COUNT(*) FROM [dbo].[AspNetUsers]

PRINT ''
PRINT 'Cleanup script completed successfully!'
GO

