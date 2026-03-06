/*
  Seed translations into wixi_Translation table.
  Safe to run multiple times (MERGE by Key).

  Notes:
  - Adjust schema name if different.
  - Ensure the database context has created the table: wixi_Translation
*/

SET NOCOUNT ON;

-- Helper MERGE template
-- Usage:
--   Replace @Key, @De, @Tr, @En, @Ar then execute MERGE block

/* Dashboard / Admin titles */
MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.title.dashboard' AS [Key], N'Dashboard' AS De, N'Kontrol Paneli' AS Tr, N'Dashboard' AS En, N'لوحة التحكم' AS Ar) AS src
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.title.users', N'Benutzer', N'Kullanıcılar', N'Users', N'المستخدمون') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.title.reports', N'Berichte', N'Raporlar', N'Reports', N'التقارير') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

/* Admin menu */
MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.main', N'Hauptmenü', N'Ana Menü', N'Main', N'القائمة الرئيسية') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.applications', N'Bewerbungen', N'Başvurular', N'Applications', N'الطلبات') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.email_management', N'E-Mail Verwaltung', N'E-Mail Yönetimi', N'Email Management', N'إدارة البريد') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.system', N'System', N'Sistem', N'System', N'النظام') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.dashboard', N'Dashboard', N'Kontrol Paneli', N'Dashboard', N'لوحة التحكم') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.users', N'Benutzer', N'Kullanıcılar', N'Users', N'المستخدمون') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.reports', N'Berichte', N'Raporlar', N'Reports', N'التقارير') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.smtp_settings', N'SMTP Einstellungen', N'SMTP Ayarları', N'SMTP Settings', N'إعدادات SMTP') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.email_logs', N'E-Mail Protokoll', N'E-Posta Logları', N'Email Logs', N'Sجلات البريد') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.email_templates', N'E-Mail Vorlagen', N'E-Posta Şablonları', N'Email Templates', N'قوالب البريد') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.application_logs', N'Anwendungsprotokolle', N'Uygulama Logları', N'Application Logs', N'Sجلات التطبيق') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.settings', N'Einstellungen', N'Ayarlar', N'Settings', N'الإعدادات') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.team_members', N'Teammitglieder', N'Takım Üyeleri', N'Team Members', N'أعضاء الفريق') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.news', N'Neuigkeiten', N'Haberler', N'News', N'الأخبار') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

/* Applications */
MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.employee_submissions', N'Mitarbeiterbewerbungen', N'Çalışan Başvuruları', N'Employee Submissions', N'طلبات الموظفين') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.employer_submissions', N'Arbeitgeberanfragen', N'İşveren Talepleri', N'Employer Requests', N'طلبات أصحاب العمل') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.employer_submissions_v2', N'Arbeitgeberanfragen V2', N'İşveren Talepleri V2', N'Employer Requests V2', N'طلبات أصحاب العمل 2') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'admin.menu.contact_submissions', N'Kontaktanfragen', N'İletişim Talepleri', N'Contact Submissions', N'طلبات التواصل') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

/* Footer / Contact basics */
MERGE dbo.wixi_Translation AS target
USING (SELECT 'footer.company_desc', N'Worklines ist ein internationales Personal- und Karriereunternehmen.', N'Worklines uluslararası bir insan kaynakları ve kariyer şirketidir.', N'Worklines is an international HR and career company.', N'ووركلاينز هي شركة توظيف ومسيرة مهنية دولية.') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'footer.follow_us', N'Folgen Sie uns', N'Bizi takip edin', N'Follow us', N'تابعنا') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

/* About page */
MERGE dbo.wixi_Translation AS target
USING (SELECT 'about.mission_text_1', N'Wir verbinden Talente mit Chancen.', N'Yetenekleri fırsatlarla buluşturuyoruz.', N'We connect talent with opportunities.', N'نربط المواهب بالفرص.') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'about.mission_text_2', N'Gemeinsam gestalten wir Karrieren über Grenzen hinweg.', N'Birlikte sınırların ötesinde kariyerler inşa ediyoruz.', N'Together, we build careers beyond borders.', N'معًا نبني مسيرات مهنية تتجاوز الحدود.') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

/* Contact page */
MERGE dbo.wixi_Translation AS target
USING (SELECT 'contact.info_title', N'Kontaktinformationen', N'İletişim Bilgileri', N'Contact Information', N'معلومات الاتصال') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'contact.phone', N'Telefon', N'Telefon', N'Phone', N'الهاتف') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'contact.email', N'E-Mail', N'E-Posta', N'Email', N'البريد الإلكتروني') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'contact.address', N'Adresse', N'Adres', N'Address', N'العنوان') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'contact.address_germany', N'Deutschland Büro', N'Almanya Ofisi', N'Germany Office', N'مكتب ألمانيا') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'contact.address_turkey_mersin', N'Türkei - Mersin', N'Türkiye - Mersin', N'Turkey - Mersin', N'تركيا - مرسين') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

MERGE dbo.wixi_Translation AS target
USING (SELECT 'contact.address_turkey_istanbul', N'Türkei - Istanbul', N'Türkiye - İstanbul', N'Turkey - Istanbul', N'تركيا - إسطنبول') AS src([Key], De, Tr, En, Ar)
ON (target.[Key] = src.[Key])
WHEN MATCHED THEN UPDATE SET ValueDe = src.De, ValueTr = src.Tr, ValueEn = src.En, ValueAr = src.Ar, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = N'seed'
WHEN NOT MATCHED THEN INSERT ([Key], ValueDe, ValueTr, ValueEn, ValueAr, CreatedAt, UpdatedAt, UpdatedBy)
VALUES (src.[Key], src.De, src.Tr, src.En, src.Ar, SYSUTCDATETIME(), SYSUTCDATETIME(), N'seed');

PRINT 'Translation seed completed.';


