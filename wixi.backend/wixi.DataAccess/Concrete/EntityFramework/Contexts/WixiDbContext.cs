using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wixi.Entities.Concrete;
using wixi.Entities.Concrete.Client;
using wixi.Entities.Concrete.Document;
using wixi.Entities.Concrete.Application;
using wixi.Entities.Concrete.Support;
using wixi.Entities.Concrete.Identity;
using wixi.Entities.Concrete.Core;
using wixi.Entities.Concrete.Content;
using wixi.Entities.Concrete.Email;
using wixi.Entities.Concrete.Form;

namespace wixi.DataAccess.Concrete.EntityFramework.Contexts
{
    public class WixiDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public WixiDbContext(DbContextOptions<WixiDbContext> options) : base(options)
        {
        }

        public DbSet<TokenBlacklist> TokenBlacklist { get; set; }
        public DbSet<wixi.Entities.Concrete.Core.ApplicationLog> ApplicationLogs { get; set; }
        public DbSet<SmtpSettings> SmtpSettings { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<ContactFormSubmission> ContactFormSubmissions { get; set; }
        public DbSet<EmployerFormSubmission> EmployerFormSubmissions { get; set; }
        public DbSet<EmployeeFormSubmission> EmployeeFormSubmissions { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<ContentSettings> ContentSettings { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        // Document Tracking System
        // Client
        public DbSet<Entities.Concrete.Client.Client> Clients { get; set; }
        public DbSet<EducationType> EducationTypes { get; set; }
        public DbSet<EducationInfo> EducationInfos { get; set; }
        public DbSet<PendingClientCode> PendingClientCodes { get; set; }
        
        // Document
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Entities.Concrete.Document.Document> Documents { get; set; }
        public DbSet<DocumentReview> DocumentReviews { get; set; }
        public DbSet<FileStorage> FileStorages { get; set; }
        
        // Application
        public DbSet<Entities.Concrete.Application.Application> Applications { get; set; }
        public DbSet<ApplicationTemplate> ApplicationTemplates { get; set; }
        public DbSet<ApplicationStep> ApplicationSteps { get; set; }
        public DbSet<ApplicationStepTemplate> ApplicationStepTemplates { get; set; }
        public DbSet<ApplicationSubStep> ApplicationSubSteps { get; set; }
        public DbSet<ApplicationSubStepTemplate> ApplicationSubStepTemplates { get; set; }
        public DbSet<ApplicationHistory> ApplicationHistories { get; set; }
        
        // Support
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<SupportMessage> SupportMessages { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global cascade delete restriction to prevent circular cascade paths
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // TokenBlacklist için yapılandırmalar
            modelBuilder.Entity<TokenBlacklist>(entity =>
            {
                entity.ToTable("TokenBlacklist");
                entity.Property(t => t.Token).IsRequired();
                entity.Property(t => t.BlacklistedAt).IsRequired();
                entity.Property(t => t.ExpirationDate).IsRequired();
            });

            // ApplicationLogs için yapılandırmalar
            // NOT: Bu tablo Serilog tarafından otomatik yönetilir (AutoCreateSqlTable = true)
            // EF Core migrations'a dahil edilmez, sadece querying için DbSet olarak tanımlıdır
            modelBuilder.Entity<wixi.Entities.Concrete.Core.ApplicationLog>(entity =>
            {
                entity.ToTable("ApplicationLogs");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Message).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Timestamp).IsRequired();
                
                // Indexes for better query performance
                // NOT: Bu indexler Serilog tarafından oluşturulmaz, ihtiyaç duyulursa manuel eklenmeli
                // entity.HasIndex(e => e.Timestamp).HasDatabaseName("IX_ApplicationLogs_Timestamp");
                // entity.HasIndex(e => e.Level).HasDatabaseName("IX_ApplicationLogs_Level");
                // entity.HasIndex(e => e.UserId).HasDatabaseName("IX_ApplicationLogs_UserId");
                // entity.HasIndex(e => e.RequestPath).HasDatabaseName("IX_ApplicationLogs_RequestPath");
                // entity.HasIndex(e => new { e.Timestamp, e.Level }).HasDatabaseName("IX_ApplicationLogs_Timestamp_Level");
                
                // Tabloyu EF migrations'dan hariç tut - Serilog yönetir
                entity.Metadata.SetIsTableExcludedFromMigrations(true);
            });

            // wixi_SmtpSettings configuration
            modelBuilder.Entity<SmtpSettings>(entity =>
            {
                entity.ToTable("wixi_SmtpSettings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Host).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Port).IsRequired();
                entity.Property(e => e.UseSsl).IsRequired();
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordEnc).IsRequired();
                entity.Property(e => e.FromName).HasMaxLength(255);
                entity.Property(e => e.FromEmail).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RetryCount).HasDefaultValue(3);
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            // wixi_EmailLog configuration
            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.ToTable("wixi_EmailLog");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FromEmail).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FromName).HasMaxLength(255);
                entity.Property(e => e.ToEmails).IsRequired();
                entity.Property(e => e.Subject).HasMaxLength(500);
                entity.Property(e => e.SmtpHost).HasMaxLength(255);
                entity.Property(e => e.UsedUserName).HasMaxLength(255);
                entity.Property(e => e.TemplateKey).HasMaxLength(100);
                entity.Property(e => e.RequestIp).HasMaxLength(64);
                entity.Property(e => e.UserAgent).HasMaxLength(512);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.Status).HasDefaultValue((byte)0);
                entity.Property(e => e.AttemptCount).HasDefaultValue(0);

                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_wixi_EmailLog_CreatedAt");
                entity.HasIndex(e => new { e.Status, e.CreatedAt }).HasDatabaseName("IX_wixi_EmailLog_Status_CreatedAt");
                entity.HasIndex(e => e.CorrelationId).HasDatabaseName("IX_wixi_EmailLog_CorrelationId");
            });

            // wixi_ContactFormSubmission configuration
            modelBuilder.Entity<ContactFormSubmission>(entity =>
            {
                entity.ToTable("wixi_ContactFormSubmission");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nationality).HasMaxLength(100);
                entity.Property(e => e.Education).HasMaxLength(50);
                entity.Property(e => e.FieldOfStudy).HasMaxLength(200);
                entity.Property(e => e.WorkExperience).HasMaxLength(50);
                entity.Property(e => e.GermanLevel).HasMaxLength(10);
                entity.Property(e => e.EnglishLevel).HasMaxLength(10);
                entity.Property(e => e.Interest).HasMaxLength(50);
                entity.Property(e => e.PreferredCity).HasMaxLength(100);
                entity.Property(e => e.Timeline).HasMaxLength(50);
                entity.Property(e => e.RequestIp).HasMaxLength(64);
                entity.Property(e => e.UserAgent).HasMaxLength(512);
                entity.Property(e => e.Language).HasMaxLength(10);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_wixi_ContactFormSubmission_CreatedAt");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_wixi_ContactFormSubmission_Email");
            });

            // wixi_EmployerFormSubmission configuration
            modelBuilder.Entity<EmployerFormSubmission>(entity =>
            {
                entity.ToTable("wixi_EmployerFormSubmission");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ContactPerson).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Industry).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CompanySize).HasMaxLength(50);
                entity.Property(e => e.Positions).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Requirements).IsRequired();
                entity.Property(e => e.RequestIp).HasMaxLength(64);
                entity.Property(e => e.UserAgent).HasMaxLength(512);
                entity.Property(e => e.Language).HasMaxLength(10);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_wixi_EmployerFormSubmission_CreatedAt");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_wixi_EmployerFormSubmission_Email");
                entity.HasIndex(e => e.Industry).HasDatabaseName("IX_wixi_EmployerFormSubmission_Industry");
            });

            // wixi_EmployeeFormSubmission configuration
            modelBuilder.Entity<EmployeeFormSubmission>(entity =>
            {
                entity.ToTable("wixi_EmployeeFormSubmission");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Salutation).HasMaxLength(20);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Profession).HasMaxLength(200);
                entity.Property(e => e.Education).HasMaxLength(50);
                entity.Property(e => e.GermanLevel).HasMaxLength(10);
                entity.Property(e => e.CvFileName).HasMaxLength(255);
                entity.Property(e => e.CvFilePath).HasMaxLength(500);
                entity.Property(e => e.RequestIp).HasMaxLength(64);
                entity.Property(e => e.UserAgent).HasMaxLength(512);
                entity.Property(e => e.Language).HasMaxLength(10);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_wixi_EmployeeFormSubmission_CreatedAt");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_wixi_EmployeeFormSubmission_Email");
                entity.HasIndex(e => e.Profession).HasDatabaseName("IX_wixi_EmployeeFormSubmission_Profession");
            });

            // wixi_EmailTemplate configuration
            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.ToTable("wixi_EmailTemplate");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
                entity.Property(e => e.BodyHtml).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                // Unique constraint on Key
                entity.HasIndex(e => e.Key).IsUnique().HasDatabaseName("IX_wixi_EmailTemplate_Key");
            });

            // wixi_TeamMember configuration
            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.ToTable("wixi_TeamMember");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(50);
                entity.Property(e => e.Experience).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PositionDe).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PositionTr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PositionEn).HasMaxLength(200);
                entity.Property(e => e.LocationDe).IsRequired().HasMaxLength(200);
                entity.Property(e => e.LocationTr).IsRequired().HasMaxLength(200);
                entity.Property(e => e.LocationEn).HasMaxLength(200);
                entity.Property(e => e.EducationDe).IsRequired().HasMaxLength(500);
                entity.Property(e => e.EducationTr).IsRequired().HasMaxLength(500);
                entity.Property(e => e.EducationEn).HasMaxLength(500);
                entity.Property(e => e.BioDe).IsRequired();
                entity.Property(e => e.BioTr).IsRequired();
                entity.Property(e => e.BioEn);
                entity.Property(e => e.PhilosophyDe);
                entity.Property(e => e.PhilosophyTr);
                entity.Property(e => e.PhilosophyEn);
                entity.Property(e => e.SpecializationsDe);
                entity.Property(e => e.SpecializationsTr);
                entity.Property(e => e.SpecializationsEn);
                entity.Property(e => e.LanguagesDe);
                entity.Property(e => e.LanguagesTr);
                entity.Property(e => e.LanguagesEn);
                entity.Property(e => e.AchievementsDe);
                entity.Property(e => e.AchievementsTr);
                entity.Property(e => e.AchievementsEn);
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => e.Slug).IsUnique().HasDatabaseName("IX_wixi_TeamMember_Slug");
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_wixi_TeamMember_IsActive");
                entity.HasIndex(e => e.DisplayOrder).HasDatabaseName("IX_wixi_TeamMember_DisplayOrder");
            });

            // wixi_NewsItem configuration
            modelBuilder.Entity<NewsItem>(entity =>
            {
                entity.ToTable("wixi_NewsItem");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TitleDe).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleTr).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleEn).HasMaxLength(500);
                entity.Property(e => e.TitleAr).HasMaxLength(500);
                entity.Property(e => e.ExcerptDe).IsRequired();
                entity.Property(e => e.ExcerptTr).IsRequired();
                entity.Property(e => e.ExcerptEn);
                entity.Property(e => e.ExcerptAr);
                entity.Property(e => e.ContentDe);
                entity.Property(e => e.ContentTr);
                entity.Property(e => e.ContentEn);
                entity.Property(e => e.ContentAr);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Featured).HasDefaultValue(false);
                entity.Property(e => e.Slug).HasMaxLength(300);
                entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => e.Slug).HasDatabaseName("IX_wixi_NewsItem_Slug");
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_wixi_NewsItem_IsActive");
                entity.HasIndex(e => e.Featured).HasDatabaseName("IX_wixi_NewsItem_Featured");
                entity.HasIndex(e => e.DisplayOrder).HasDatabaseName("IX_wixi_NewsItem_DisplayOrder");
                entity.HasIndex(e => e.PublishedAt).HasDatabaseName("IX_wixi_NewsItem_PublishedAt");
                entity.HasIndex(e => e.Category).HasDatabaseName("IX_wixi_NewsItem_Category");
            });

            // wixi_SystemSettings configuration
            modelBuilder.Entity<SystemSettings>(entity =>
            {
                entity.ToTable("wixi_SystemSettings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SiteName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.SiteUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.AdminEmail).IsRequired().HasMaxLength(255);
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            // wixi_ContentSettings configuration
            modelBuilder.Entity<ContentSettings>(entity =>
            {
                entity.ToTable("wixi_ContentSettings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FooterCompanyDescDe).IsRequired();
                entity.Property(e => e.FooterCompanyDescTr).IsRequired();
                entity.Property(e => e.FooterCompanyDescEn);
                entity.Property(e => e.FooterCompanyDescAr);
                entity.Property(e => e.FacebookUrl).HasMaxLength(500);
                entity.Property(e => e.InstagramUrl).HasMaxLength(500);
                entity.Property(e => e.TwitterUrl).HasMaxLength(500);
                entity.Property(e => e.LinkedInUrl).HasMaxLength(500);
                entity.Property(e => e.AboutMissionText1De).IsRequired();
                entity.Property(e => e.AboutMissionText1Tr).IsRequired();
                entity.Property(e => e.AboutMissionText1En);
                entity.Property(e => e.AboutMissionText1Ar);
                entity.Property(e => e.AboutMissionText2De).IsRequired();
                entity.Property(e => e.AboutMissionText2Tr).IsRequired();
                entity.Property(e => e.AboutMissionText2En);
                entity.Property(e => e.AboutMissionText2Ar);
                entity.Property(e => e.ContactPhone).HasMaxLength(50);
                entity.Property(e => e.ContactEmail).HasMaxLength(255);
                entity.Property(e => e.AddressGermany);
                entity.Property(e => e.AddressTurkeyMersin);
                entity.Property(e => e.AddressTurkeyIstanbul);
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            // wixi_Translation configuration
            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("wixi_Translation");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(300);
                entity.Property(e => e.De);
                entity.Property(e => e.Tr);
                entity.Property(e => e.En);
                entity.Property(e => e.Ar);
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasIndex(e => e.Key).IsUnique().HasDatabaseName("IX_wixi_Translation_Key");
            });

            // wixi_UserPreference configuration
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("wixi_UserPreference");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Language).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Theme).IsRequired().HasMaxLength(10);
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("IX_wixi_UserPreference_UserId");
            });

            // ==================== DOCUMENT TRACKING SYSTEM ====================

            // Client configuration
            modelBuilder.Entity<Entities.Concrete.Client.Client>(entity =>
            {
                entity.ToTable("wixi_Client");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ClientCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.User).WithOne().HasForeignKey<Entities.Concrete.Client.Client>(e => e.UserId);
                entity.HasOne(e => e.EducationType).WithMany(e => e.Clients).HasForeignKey(e => e.EducationTypeId);
                
                entity.HasIndex(e => e.ClientCode).IsUnique().HasDatabaseName("IX_wixi_Client_ClientCode");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_wixi_Client_Email");
                entity.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("IX_wixi_Client_UserId");
            });

            // EducationType configuration
            modelBuilder.Entity<EducationType>(entity =>
            {
                entity.ToTable("wixi_EducationType");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("IX_wixi_EducationType_Code");
            });

            // PendingClientCode configuration
            modelBuilder.Entity<PendingClientCode>(entity =>
            {
                entity.ToTable("wixi_PendingClientCode");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClientCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ExpirationDate).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => e.ClientCode).IsUnique().HasDatabaseName("IX_wixi_PendingClientCode_ClientCode");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_wixi_PendingClientCode_Email");
                entity.HasIndex(e => e.ExpirationDate).HasDatabaseName("IX_wixi_PendingClientCode_ExpirationDate");
                entity.HasIndex(e => e.IsUsed).HasDatabaseName("IX_wixi_PendingClientCode_IsUsed");
            });

            // EducationInfo configuration
            modelBuilder.Entity<EducationInfo>(entity =>
            {
                entity.ToTable("wixi_EducationInfo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Degree).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Institution).IsRequired().HasMaxLength(300);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Client).WithMany(e => e.EducationHistory).HasForeignKey(e => e.ClientId);
                
                entity.HasIndex(e => e.ClientId).HasDatabaseName("IX_wixi_EducationInfo_ClientId");
            });

            // DocumentType configuration
            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("wixi_DocumentType");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(300);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.EducationType).WithMany(e => e.RequiredDocuments).HasForeignKey(e => e.EducationTypeId);
                
                entity.HasIndex(e => e.Code).HasDatabaseName("IX_wixi_DocumentType_Code");
                entity.HasIndex(e => e.EducationTypeId).HasDatabaseName("IX_wixi_DocumentType_EducationTypeId");
            });

            // Document configuration
            modelBuilder.Entity<Entities.Concrete.Document.Document>(entity =>
            {
                entity.ToTable("wixi_Document");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.StoredFileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.FileExtension).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Client).WithMany(e => e.Documents).HasForeignKey(e => e.ClientId);
                entity.HasOne(e => e.DocumentType).WithMany(e => e.Documents).HasForeignKey(e => e.DocumentTypeId);
                entity.HasOne(e => e.Review).WithOne(e => e.Document).HasForeignKey<DocumentReview>(e => e.DocumentId);
                
                entity.HasIndex(e => e.ClientId).HasDatabaseName("IX_wixi_Document_ClientId");
                entity.HasIndex(e => e.DocumentTypeId).HasDatabaseName("IX_wixi_Document_DocumentTypeId");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_wixi_Document_Status");
                entity.HasIndex(e => e.UploadedAt).HasDatabaseName("IX_wixi_Document_UploadedAt");
            });

            // DocumentReview configuration
            modelBuilder.Entity<DocumentReview>(entity =>
            {
                entity.ToTable("wixi_DocumentReview");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReviewedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Reviewer).WithMany().HasForeignKey(e => e.ReviewerId);
                
                entity.HasIndex(e => e.DocumentId).IsUnique().HasDatabaseName("IX_wixi_DocumentReview_DocumentId");
                entity.HasIndex(e => e.ReviewerId).HasDatabaseName("IX_wixi_DocumentReview_ReviewerId");
            });

            // FileStorage configuration
            modelBuilder.Entity<FileStorage>(entity =>
            {
                entity.ToTable("wixi_FileStorage");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.StoredFileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasIndex(e => new { e.EntityType, e.EntityId }).HasDatabaseName("IX_wixi_FileStorage_Entity");
            });

            // Application configuration
            modelBuilder.Entity<Entities.Concrete.Application.Application>(entity =>
            {
                entity.ToTable("wixi_Application");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ApplicationNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Client).WithMany(e => e.Applications).HasForeignKey(e => e.ClientId);
                entity.HasOne(e => e.Template).WithMany(e => e.Applications).HasForeignKey(e => e.ApplicationTemplateId);
                
                entity.HasIndex(e => e.ApplicationNumber).IsUnique().HasDatabaseName("IX_wixi_Application_ApplicationNumber");
                entity.HasIndex(e => e.ClientId).HasDatabaseName("IX_wixi_Application_ClientId");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_wixi_Application_Status");
            });

            // ApplicationTemplate configuration
            modelBuilder.Entity<ApplicationTemplate>(entity =>
            {
                entity.ToTable("wixi_ApplicationTemplate");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            });

            // ApplicationStep configuration
            modelBuilder.Entity<ApplicationStep>(entity =>
            {
                entity.ToTable("wixi_ApplicationStep");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Application).WithMany(e => e.Steps).HasForeignKey(e => e.ApplicationId);
                entity.HasOne(e => e.Template).WithMany(e => e.Steps).HasForeignKey(e => e.StepTemplateId);
                
                entity.HasIndex(e => e.ApplicationId).HasDatabaseName("IX_wixi_ApplicationStep_ApplicationId");
            });

            // ApplicationStepTemplate configuration
            modelBuilder.Entity<ApplicationStepTemplate>(entity =>
            {
                entity.ToTable("wixi_ApplicationStepTemplate");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Template).WithMany(e => e.StepTemplates).HasForeignKey(e => e.ApplicationTemplateId);
            });

            // ApplicationSubStep configuration
            modelBuilder.Entity<ApplicationSubStep>(entity =>
            {
                entity.ToTable("wixi_ApplicationSubStep");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(300);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Step).WithMany(e => e.SubSteps).HasForeignKey(e => e.ApplicationStepId);
                entity.HasOne(e => e.Template).WithMany(e => e.SubSteps).HasForeignKey(e => e.SubStepTemplateId);
                
                entity.HasIndex(e => e.ApplicationStepId).HasDatabaseName("IX_wixi_ApplicationSubStep_ApplicationStepId");
            });

            // ApplicationSubStepTemplate configuration
            modelBuilder.Entity<ApplicationSubStepTemplate>(entity =>
            {
                entity.ToTable("wixi_ApplicationSubStepTemplate");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.StepTemplate).WithMany(e => e.SubStepTemplates).HasForeignKey(e => e.StepTemplateId);
            });

            // ApplicationHistory configuration
            modelBuilder.Entity<ApplicationHistory>(entity =>
            {
                entity.ToTable("wixi_ApplicationHistory");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Application).WithMany(e => e.History).HasForeignKey(e => e.ApplicationId);
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
                
                entity.HasIndex(e => e.ApplicationId).HasDatabaseName("IX_wixi_ApplicationHistory_ApplicationId");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_wixi_ApplicationHistory_CreatedAt");
            });

            // SupportTicket configuration
            modelBuilder.Entity<SupportTicket>(entity =>
            {
                entity.ToTable("wixi_SupportTicket");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TicketNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Client).WithMany(e => e.SupportTickets).HasForeignKey(e => e.ClientId);
                entity.HasOne(e => e.AssignedTo).WithMany().HasForeignKey(e => e.AssignedToId);
                
                entity.HasIndex(e => e.TicketNumber).IsUnique().HasDatabaseName("IX_wixi_SupportTicket_TicketNumber");
                entity.HasIndex(e => e.ClientId).HasDatabaseName("IX_wixi_SupportTicket_ClientId");
                entity.HasIndex(e => e.Status).HasDatabaseName("IX_wixi_SupportTicket_Status");
            });

            // SupportMessage configuration
            modelBuilder.Entity<SupportMessage>(entity =>
            {
                entity.ToTable("wixi_SupportMessage");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Ticket).WithMany(e => e.Messages).HasForeignKey(e => e.TicketId);
                entity.HasOne(e => e.Sender).WithMany().HasForeignKey(e => e.SenderId);
                
                entity.HasIndex(e => e.TicketId).HasDatabaseName("IX_wixi_SupportMessage_TicketId");
            });

            // FAQ configuration
            modelBuilder.Entity<FAQ>(entity =>
            {
                entity.ToTable("wixi_FAQ");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Question).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.QuestionEn).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Answer).IsRequired();
                entity.Property(e => e.AnswerEn).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.Author).WithMany().HasForeignKey(e => e.AuthorId);
                
                entity.HasIndex(e => e.Category).HasDatabaseName("IX_wixi_FAQ_Category");
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_wixi_FAQ_IsActive");
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("wixi_Notification");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
                
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_wixi_Notification_UserId");
                entity.HasIndex(e => e.IsRead).HasDatabaseName("IX_wixi_Notification_IsRead");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_wixi_Notification_CreatedAt");
            });
        }
    }
}

