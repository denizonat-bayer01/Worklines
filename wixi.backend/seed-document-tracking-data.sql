-- ============================================
-- Seed Data for Document Tracking System
-- ============================================
-- Date: November 4, 2025
-- Description: Reference data for Education Types, Document Types, Application Templates, and FAQs
-- ============================================

USE [wixi-worklinesde]
GO

-- ============================================
-- 1. EDUCATION TYPES (Eğitim Tipleri)
-- ============================================
PRINT 'Inserting Education Types...'

SET IDENTITY_INSERT [dbo].[EducationTypes] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[EducationTypes] WHERE Code = 'university')
BEGIN
    INSERT INTO [dbo].[EducationTypes] (Id, Code, Name, NameEn, Description, IsActive, DisplayOrder, CreatedAt)
    VALUES 
    (1, 'university', 'Üniversite Mezunu', 'University Graduate', 
     'Lisans, Yüksek Lisans veya Doktora mezunu adaylar için', 1, 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[EducationTypes] WHERE Code = 'vocational')
BEGIN
    INSERT INTO [dbo].[EducationTypes] (Id, Code, Name, NameEn, Description, IsActive, DisplayOrder, CreatedAt)
    VALUES 
    (2, 'vocational', 'Meslek Lisesi Mezunu', 'Vocational School Graduate', 
     'Meslek lisesi mezunu adaylar için', 1, 2, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[EducationTypes] WHERE Code = 'masterCraftsman')
BEGIN
    INSERT INTO [dbo].[EducationTypes] (Id, Code, Name, NameEn, Description, IsActive, DisplayOrder, CreatedAt)
    VALUES 
    (3, 'masterCraftsman', 'Kalfalık/Ustalık Belgesi', 'Master Craftsman Certificate', 
     'Kalfalık veya ustalık belgesi olan adaylar için', 1, 3, GETDATE());
END

SET IDENTITY_INSERT [dbo].[EducationTypes] OFF;

PRINT 'Education Types inserted successfully.'
GO

-- ============================================
-- 2. DOCUMENT TYPES (Belge Türleri)
-- ============================================
PRINT 'Inserting Document Types...'

SET IDENTITY_INSERT [dbo].[DocumentTypes] ON;

-- University Document Types
INSERT INTO [dbo].[DocumentTypes] (Id, Code, Name, NameEn, EducationTypeId, IsRequired, AllowedFileTypes, MaxFileSizeBytes, RequiresApproval, DisplayOrder, IconName, Note, IsActive, CreatedAt)
VALUES 
(1, 'passport', 'Pasaport (Renkli Fotokopi - PDF)', 'Passport (Color Copy - PDF)', 1, 1, '.pdf', 10485760, 1, 1, 'file-text', NULL, 1, GETDATE()),
(2, 'cv', 'CV (Almanca veya İngilizce)', 'CV (German or English)', 1, 1, '.pdf,.doc,.docx', 5242880, 1, 2, 'file-text', NULL, 1, GETDATE()),
(3, 'residence', 'İkamet Belgesi (E-Devlet PDF)', 'Residence Certificate (E-Government PDF)', 1, 1, '.pdf', 5242880, 1, 3, 'file-text', NULL, 1, GETDATE()),
(4, 'university_diploma', 'Üniversite Diploması', 'University Diploma', 1, 1, '.pdf', 10485760, 1, 4, 'award', NULL, 1, GETDATE()),
(5, 'university_diploma_2', 'İkinci Üniversite Diploması (Varsa)', 'Second University Diploma (If Any)', 1, 0, '.pdf', 10485760, 1, 5, 'award', NULL, 1, GETDATE()),
(6, 'university_diploma_3', 'Üçüncü Üniversite Diploması (Varsa)', 'Third University Diploma (If Any)', 1, 0, '.pdf', 10485760, 1, 6, 'award', NULL, 1, GETDATE()),
(7, 'yok_certificate', 'YÖK Mezun Belgesi (E-Devlet)', 'YÖK Graduate Certificate (E-Government)', 1, 1, '.pdf', 5242880, 1, 7, 'file-text', NULL, 1, GETDATE()),
(8, 'transcript', 'Supplement - Transkript (E-Devlet)', 'Supplement - Transcript (E-Government)', 1, 1, '.pdf', 10485760, 1, 8, 'file-text', NULL, 1, GETDATE()),
(9, 'formula_b', 'Formül B - Evlilik Belgesi (Evli Kadınlar İçin)', 'Formula B - Marriage Certificate (For Married Women)', 1, 0, '.pdf', 5242880, 1, 9, 'file-text', '20€ ücret karşılığında nüfus müdürlüğünden alınır', 1, GETDATE()),
(10, 'highschool_diploma', 'Lise Diploması/Mezun Belgesi (E-Devlet)', 'High School Diploma/Graduate Certificate (E-Government)', 1, 1, '.pdf', 10485760, 1, 10, 'award', NULL, 1, GETDATE()),
(11, 'other_certificates', 'Diğer Sertifikalar (Varsa)', 'Other Certificates (If Any)', 1, 0, '.pdf,.jpg,.png', 10485760, 1, 11, 'file-text', 'Dil sertifikaları, kurslar vb.', 1, GETDATE());

-- Vocational School Document Types
INSERT INTO [dbo].[DocumentTypes] (Id, Code, Name, NameEn, EducationTypeId, IsRequired, AllowedFileTypes, MaxFileSizeBytes, RequiresApproval, DisplayOrder, IconName, Note, IsActive, CreatedAt)
VALUES 
(12, 'passport_vocational', 'Pasaport (Renkli Fotokopi - PDF)', 'Passport (Color Copy - PDF)', 2, 1, '.pdf', 10485760, 1, 1, 'file-text', NULL, 1, GETDATE()),
(13, 'cv_vocational', 'CV (Almanca veya İngilizce)', 'CV (German or English)', 2, 1, '.pdf,.doc,.docx', 5242880, 1, 2, 'file-text', NULL, 1, GETDATE()),
(14, 'vocational_diploma', 'Meslek Lisesi Diploması', 'Vocational School Diploma', 2, 1, '.pdf', 10485760, 1, 3, 'award', NULL, 1, GETDATE()),
(15, 'vocational_graduate_cert', 'Lise Mezun Belgesi (E-Devlet)', 'High School Graduate Certificate (E-Government)', 2, 1, '.pdf', 5242880, 1, 4, 'file-text', NULL, 1, GETDATE()),
(16, 'sgk_service', 'SGK Hizmet Dökümü (E-Devlet)', 'SGK Service Summary (E-Government)', 2, 1, '.pdf', 5242880, 1, 5, 'file-text', 'Çalışma geçmişinizi gösterir', 1, GETDATE()),
(17, 'residence_vocational', 'İkamet Belgesi (E-Devlet PDF)', 'Residence Certificate (E-Government PDF)', 2, 1, '.pdf', 5242880, 1, 6, 'file-text', NULL, 1, GETDATE()),
(18, 'formula_b_vocational', 'Formül B - Evlilik Belgesi', 'Formula B - Marriage Certificate', 2, 0, '.pdf', 5242880, 1, 7, 'file-text', 'Evli kadınlar için', 1, GETDATE()),
(19, 'other_certificates_vocational', 'Diğer Sertifikalar (Varsa)', 'Other Certificates (If Any)', 2, 0, '.pdf,.jpg,.png', 10485760, 1, 8, 'file-text', NULL, 1, GETDATE());

-- Master Craftsman Document Types
INSERT INTO [dbo].[DocumentTypes] (Id, Code, Name, NameEn, EducationTypeId, IsRequired, AllowedFileTypes, MaxFileSizeBytes, RequiresApproval, DisplayOrder, IconName, Note, IsActive, CreatedAt)
VALUES 
(20, 'passport_craftsman', 'Pasaport (Renkli Fotokopi - PDF)', 'Passport (Color Copy - PDF)', 3, 1, '.pdf', 10485760, 1, 1, 'file-text', NULL, 1, GETDATE()),
(21, 'cv_craftsman', 'CV (Almanca veya İngilizce)', 'CV (German or English)', 3, 1, '.pdf,.doc,.docx', 5242880, 1, 2, 'file-text', NULL, 1, GETDATE()),
(22, 'apprentice_certificate', 'Kalfalık Belgesi', 'Apprenticeship Certificate', 3, 1, '.pdf', 10485760, 1, 3, 'award', NULL, 1, GETDATE()),
(23, 'master_certificate', 'Ustalık Belgesi (Varsa)', 'Master Certificate (If Any)', 3, 0, '.pdf', 10485760, 1, 4, 'award', NULL, 1, GETDATE()),
(24, 'apprentice_transcript', 'Kalfalık Belgesi Transkripti', 'Apprenticeship Certificate Transcript', 3, 1, '.pdf', 5242880, 1, 5, 'file-text', NULL, 1, GETDATE()),
(25, 'master_transcript', 'Ustalık Belgesi Transkripti (Varsa)', 'Master Certificate Transcript (If Any)', 3, 0, '.pdf', 5242880, 1, 6, 'file-text', NULL, 1, GETDATE()),
(26, 'apprentice_edevlet', 'Kalfalık Belgesi E-Devlet Çıktısı', 'Apprenticeship Certificate E-Government Printout', 3, 1, '.pdf', 5242880, 1, 7, 'file-text', NULL, 1, GETDATE()),
(27, 'master_edevlet', 'Ustalık Belgesi E-Devlet Çıktısı (Varsa)', 'Master Certificate E-Government Printout (If Any)', 3, 0, '.pdf', 5242880, 1, 8, 'file-text', NULL, 1, GETDATE()),
(28, 'residence_craftsman', 'İkamet Belgesi (E-Devlet PDF)', 'Residence Certificate (E-Government PDF)', 3, 1, '.pdf', 5242880, 1, 9, 'file-text', NULL, 1, GETDATE()),
(29, 'formula_b_craftsman', 'Formül B - Evlilik Belgesi', 'Formula B - Marriage Certificate', 3, 0, '.pdf', 5242880, 1, 10, 'file-text', 'Evli kadınlar için', 1, GETDATE()),
(30, 'other_certificates_craftsman', 'Diğer Sertifikalar (Varsa)', 'Other Certificates (If Any)', 3, 0, '.pdf,.jpg,.png', 10485760, 1, 11, 'file-text', NULL, 1, GETDATE());

SET IDENTITY_INSERT [dbo].[DocumentTypes] OFF;

PRINT 'Document Types inserted successfully.'
GO

-- ============================================
-- 3. APPLICATION TEMPLATES (Başvuru Şablonları)
-- ============================================
PRINT 'Inserting Application Templates...'

SET IDENTITY_INSERT [dbo].[ApplicationTemplates] ON;

-- Template 1: Denklik İşlemleri
IF NOT EXISTS (SELECT 1 FROM [dbo].[ApplicationTemplates] WHERE [Type] = 0)
BEGIN
    INSERT INTO [dbo].[ApplicationTemplates] (Id, Name, NameEn, Description, [Type], IsActive, DisplayOrder, CreatedAt)
    VALUES 
    (1, 'Denklik İşlem Süreci', 'Recognition Process', 
     'Diploma denklik işlemleri için standart süreç', 0, 1, 1, GETDATE());
END

-- Template 2: İş Bulma ve Çalışma İzni
IF NOT EXISTS (SELECT 1 FROM [dbo].[ApplicationTemplates] WHERE [Type] = 1)
BEGIN
    INSERT INTO [dbo].[ApplicationTemplates] (Id, Name, NameEn, Description, [Type], IsActive, DisplayOrder, CreatedAt)
    VALUES 
    (2, 'İş Bulma ve Çalışma Müsadesi Süreci', 'Work Permit Process', 
     'İş arama ve çalışma izni başvuru süreci', 1, 1, 2, GETDATE());
END

-- Template 3: Vize İşlemleri
IF NOT EXISTS (SELECT 1 FROM [dbo].[ApplicationTemplates] WHERE [Type] = 2)
BEGIN
    INSERT INTO [dbo].[ApplicationTemplates] (Id, Name, NameEn, Description, [Type], IsActive, DisplayOrder, CreatedAt)
    VALUES 
    (3, 'Vize İşlem Süreci', 'Visa Process', 
     'Vize başvurusu ve takip süreci', 2, 1, 3, GETDATE());
END

SET IDENTITY_INSERT [dbo].[ApplicationTemplates] OFF;

PRINT 'Application Templates inserted successfully.'
GO

-- ============================================
-- 4. APPLICATION STEP TEMPLATES
-- ============================================
PRINT 'Inserting Application Step Templates...'

SET IDENTITY_INSERT [dbo].[ApplicationStepTemplates] ON;

-- Steps for Template 1: Denklik İşlemleri
INSERT INTO [dbo].[ApplicationStepTemplates] (Id, ApplicationTemplateId, Title, TitleEn, Description, StepOrder, IconName, IsRequired)
VALUES 
(1, 1, 'Denklik İşlemleri', 'Recognition Process', 'Diploma denklik işlemleri', 1, 'file-check', 1);

-- Steps for Template 2: İş Bulma Başvurusu (3 steps: Denklik, İş Bulma, Vize)
INSERT INTO [dbo].[ApplicationStepTemplates] (Id, ApplicationTemplateId, Title, TitleEn, Description, StepOrder, IconName, IsRequired)
VALUES 
(2, 2, 'Denklik İşlemleri', 'Recognition Process', 'Diploma denklik işlemleri', 1, 'file-check', 1),
(3, 2, 'İş Bulma ve Çalışma Müsadesi İşlemleri', 'Work Permit Process', 'İş arama ve çalışma izni süreçleri', 2, 'briefcase', 1),
(4, 2, 'Vize İşlemleri', 'Visa Process', 'Vize başvurusu ve onay süreci', 3, 'globe', 1);

-- Steps for Template 3: Vize İşlemleri
INSERT INTO [dbo].[ApplicationStepTemplates] (Id, ApplicationTemplateId, Title, TitleEn, Description, StepOrder, IconName, IsRequired)
VALUES 
(5, 3, 'Vize İşlemleri', 'Visa Process', 'Vize başvurusu ve onay süreci', 1, 'globe', 1);

SET IDENTITY_INSERT [dbo].[ApplicationStepTemplates] OFF;

PRINT 'Application Step Templates inserted successfully.'
GO

-- ============================================
-- 5. APPLICATION SUB-STEP TEMPLATES
-- ============================================
PRINT 'Inserting Application Sub-Step Templates...'

SET IDENTITY_INSERT [dbo].[ApplicationSubStepTemplates] ON;

-- Sub-steps for Denklik İşlemleri (StepTemplateId = 1)
INSERT INTO [dbo].[ApplicationSubStepTemplates] (Id, StepTemplateId, Name, NameEn, Description, SubStepOrder, IsRequired)
VALUES 
(1, 1, 'Belgeler Yüklendi', 'Documents Uploaded', 'Gerekli belgeler sisteme yüklendi', 1, 1),
(2, 1, 'Denklik Başvurusu Yapıldı', 'Recognition Application Submitted', 'Denklik başvurusu ilgili kuruma yapıldı', 2, 1),
(3, 1, 'Denklik Harc Ödemesi Yapıldı', 'Recognition Fee Paid', 'Denklik işlem ücreti ödendi', 3, 1),
(4, 1, 'Denklik Belgesi Hazır', 'Recognition Certificate Ready', 'Denklik belgesi onaylandı ve hazır', 4, 1);

-- Sub-steps for İş Bulma (StepTemplateId = 2)
INSERT INTO [dbo].[ApplicationSubStepTemplates] (Id, StepTemplateId, Name, NameEn, Description, SubStepOrder, IsRequired)
VALUES 
(5, 2, 'İş Sözleşmesi Sunuldu', 'Job Contract Presented', 'İş sözleşmesi adaya sunuldu', 1, 1),
(6, 2, 'İş Sözleşmesi İmzalandı', 'Job Contract Signed', 'İş sözleşmesi karşılıklı imzalandı', 2, 1),
(7, 2, 'Çalışma Müsadesi Başvurusu Yapıldı', 'Work Permit Application Submitted', 'Çalışma izni başvurusu yapıldı', 3, 1),
(8, 2, 'Çalışma Müsadesi Hazır', 'Work Permit Ready', 'Çalışma izni onaylandı', 4, 1);

-- Sub-steps for Vize İşlemleri (StepTemplateId = 3)
INSERT INTO [dbo].[ApplicationSubStepTemplates] (Id, StepTemplateId, Name, NameEn, Description, SubStepOrder, IsRequired)
VALUES 
(9, 3, 'Vize Başvurusu Yapıldı', 'Visa Application Submitted', 'Vize başvurusu yapıldı', 1, 1),
(10, 3, 'Vize Başvurusu Cevabı Geldi', 'Visa Application Response Received', 'Vize başvurusu yanıtlandı', 2, 1),
(11, 3, 'Vize Ön Onay Geldi', 'Visa Pre-Approval Received', 'Vize için ön onay alındı', 3, 1),
(12, 3, 'Konsolosluk Randevu Talebi Oluşturuldu', 'Consulate Appointment Request Created', 'Konsolosluk randevusu talep edildi', 4, 1),
(13, 3, 'Konsolosluk Randevusu Gerçekleşti', 'Consulate Appointment Completed', 'Konsolosluk randevusu yapıldı', 5, 1),
(14, 3, 'Vize Çıktı', 'Visa Issued', 'Vize onaylandı ve çıktı', 6, 1);

SET IDENTITY_INSERT [dbo].[ApplicationSubStepTemplates] OFF;

PRINT 'Application Sub-Step Templates inserted successfully.'
GO

-- ============================================
-- 6. FAQ (Sık Sorulan Sorular)
-- ============================================
PRINT 'Inserting FAQs...'

SET IDENTITY_INSERT [dbo].[FAQs] ON;

INSERT INTO [dbo].[FAQs] (Id, Question, QuestionEn, Answer, AnswerEn, Category, Tags, DisplayOrder, IsActive, IsFeatured, ViewCount, HelpfulCount, NotHelpfulCount, CreatedAt)
VALUES 
-- Genel Sorular
(1, 'Başvuru süreci ne kadar sürer?', 'How long does the application process take?',
 'Başvuru süreci ortalama 3-6 ay arasında tamamlanır. Bu süre belge onayı, denklik işlemleri ve vize süreçlerine bağlı olarak değişebilir.',
 'The application process typically takes 3-6 months to complete. This duration may vary depending on document approval, recognition procedures, and visa processes.',
 0, 'süre,başvuru,genel', 1, 1, 1, 0, 0, 0, GETDATE()),

(2, 'Hangi belgeleri yüklemem gerekiyor?', 'Which documents do I need to upload?',
 'Yüklemeniz gereken belgeler eğitim durumunuza göre değişir. Üniversite mezunları için pasaport, CV, diploma, transkript gibi belgeler gereklidir. Detaylı liste "Belgelerim" sayfasında bulunmaktadır.',
 'Required documents vary based on your education level. University graduates need passport, CV, diploma, transcript, etc. Detailed list is available on the "My Documents" page.',
 1, 'belgeler,yükleme,gereksinimler', 2, 1, 1, 0, 0, 0, GETDATE()),

(3, 'Belgelerim reddedilirse ne yapmalıyım?', 'What should I do if my documents are rejected?',
 'Reddedilen belgeler için sistem size geri bildirim mesajı gönderir. Bu mesajı inceleyip belirtilen düzeltmeleri yaptıktan sonra belgeyi yeniden yükleyebilirsiniz.',
 'If documents are rejected, the system sends you feedback message. Review the message, make required corrections, and re-upload the document.',
 1, 'red,belge,yeniden yükleme', 3, 1, 0, 0, 0, 0, GETDATE()),

-- Süreç Soruları
(4, 'Başvuru durumumu nasıl takip edebilirim?', 'How can I track my application status?',
 'Dashboard sayfasından başvurunuzun tüm aşamalarını takip edebilirsiniz. Her adımın durumu, tamamlanma tarihi ve dosya numaraları burada görüntülenir.',
 'You can track all stages of your application from the Dashboard page. Status, completion dates, and file numbers for each step are displayed here.',
 2, 'takip,dashboard,durum', 4, 1, 1, 0, 0, 0, GETDATE()),

(5, 'Denklik işlemleri nedir?', 'What are recognition procedures?',
 'Denklik işlemleri, Türkiye''de aldığınız diplomaların Almanya''da geçerli olması için yapılan onay sürecidir. Bu süreçte belgeleriniz Alman makamları tarafından değerlendirilir.',
 'Recognition procedures are the approval process for your Turkish diplomas to be valid in Germany. Your documents are evaluated by German authorities during this process.',
 2, 'denklik,diploma,onay', 5, 1, 1, 0, 0, 0, GETDATE()),

-- İletişim Soruları
(6, 'Destek ekibiyle nasıl iletişime geçebilirim?', 'How can I contact the support team?',
 'Destek Merkezi sayfasından yeni bir talep oluşturabilir veya mevcut taleplerinizi takip edebilirsiniz. Acil durumlar için e-posta ile iletişime geçebilirsiniz.',
 'You can create a new request or track existing ones from the Support Center page. For urgent matters, you can contact us via email.',
 3, 'destek,iletişim,talep', 6, 1, 0, 0, 0, 0, GETDATE()),

(7, 'Hangi dillerde destek alabiliyorum?', 'Which languages are supported?',
 'Sistemimiz Türkçe, Almanca ve İngilizce dillerinde hizmet vermektedir. Destek ekibimiz bu üç dilde size yardımcı olabilir.',
 'Our system supports Turkish, German, and English languages. Our support team can assist you in these three languages.',
 3, 'dil,destek,çeviri', 7, 1, 0, 0, 0, 0, GETDATE()),

-- Teknik Sorular
(8, 'Hangi dosya formatlarını yükleyebilirim?', 'Which file formats can I upload?',
 'PDF, JPG, PNG formatlarında belgelerinizi yükleyebilirsiniz. Dosya boyutu maksimum 10 MB olmalıdır. PDF formatı önerilir.',
 'You can upload documents in PDF, JPG, PNG formats. Maximum file size is 10 MB. PDF format is recommended.',
 4, 'dosya,format,yükleme,pdf', 8, 1, 0, 0, 0, 0, GETDATE()),

(9, 'Şifremi unuttum, ne yapmalıyım?', 'I forgot my password, what should I do?',
 'Giriş sayfasındaki "Şifremi Unuttum" linkine tıklayarak e-posta adresinize şifre sıfırlama linki gönderebilirsiniz.',
 'Click on "Forgot Password" link on the login page to receive a password reset link to your email address.',
 4, 'şifre,güvenlik,giriş', 9, 1, 0, 0, 0, 0, GETDATE()),

(10, 'Belgelerimin güvenliği nasıl sağlanıyor?', 'How is the security of my documents ensured?',
 'Tüm belgeleriniz şifreli olarak saklanır ve sadece yetkili personel tarafından erişilebilir. Veri güvenliği en üst düzeyde sağlanmaktadır.',
 'All your documents are stored encrypted and accessible only by authorized personnel. Data security is maintained at the highest level.',
 4, 'güvenlik,belge,şifreleme', 10, 1, 1, 0, 0, 0, GETDATE());

SET IDENTITY_INSERT [dbo].[FAQs] OFF;

PRINT 'FAQs inserted successfully.'
GO

-- ============================================
-- Summary
-- ============================================
PRINT ''
PRINT '============================================'
PRINT 'Seed Data Installation Complete!'
PRINT '============================================'
PRINT ''
PRINT 'Summary:'
PRINT '- Education Types: 3 records'
PRINT '- Document Types: 30 records'
PRINT '- Application Templates: 3 records'
PRINT '- Application Step Templates: 3 records'
PRINT '- Application Sub-Step Templates: 14 records'
PRINT '- FAQs: 10 records'
PRINT ''
PRINT 'Total: 63 records inserted'
PRINT '============================================'
GO

