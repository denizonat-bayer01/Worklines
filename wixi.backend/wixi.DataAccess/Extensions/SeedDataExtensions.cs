using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete.Client;
using wixi.Entities.Concrete.Document;
using wixi.Entities.Concrete.Application;
using wixi.Entities.Concrete.Support;

namespace wixi.DataAccess.Extensions
{
    public static class SeedDataExtensions
    {
        public static async Task SeedDocumentTrackingDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WixiDbContext>();

            // Seed Education Types
            await SeedEducationTypesAsync(context);
            
            // Seed Document Types
            await SeedDocumentTypesAsync(context);
            
            // Seed Application Templates
            await SeedApplicationTemplatesAsync(context);
            
            // Seed FAQs
            await SeedFAQsAsync(context);
            
            await context.SaveChangesAsync();
        }

        private static async Task SeedEducationTypesAsync(WixiDbContext context)
        {
            if (await context.EducationTypes.AnyAsync()) return;

            var educationTypes = new[]
            {
                new EducationType
                {
                    Code = "university",
                    Name = "Üniversite Mezunu",
                    NameEn = "University Graduate",
                    Description = "Lisans, Yüksek Lisans veya Doktora mezunu adaylar için",
                    IsActive = true,
                    DisplayOrder = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new EducationType
                {
                    Code = "vocational",
                    Name = "Meslek Lisesi Mezunu",
                    NameEn = "Vocational School Graduate",
                    Description = "Meslek lisesi mezunu adaylar için",
                    IsActive = true,
                    DisplayOrder = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new EducationType
                {
                    Code = "masterCraftsman",
                    Name = "Kalfalık/Ustalık Belgesi",
                    NameEn = "Master Craftsman Certificate",
                    Description = "Kalfalık veya ustalık belgesi olan adaylar için",
                    IsActive = true,
                    DisplayOrder = 3,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.EducationTypes.AddRangeAsync(educationTypes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedDocumentTypesAsync(WixiDbContext context)
        {
            if (await context.DocumentTypes.AnyAsync()) return;

            var universityEducationType = await context.EducationTypes.FirstAsync(x => x.Code == "university");
            var vocationalEducationType = await context.EducationTypes.FirstAsync(x => x.Code == "vocational");
            var craftsmanEducationType = await context.EducationTypes.FirstAsync(x => x.Code == "masterCraftsman");

            var documentTypes = new List<DocumentType>
            {
                // University Document Types
                new DocumentType { Code = "passport", Name = "Pasaport (Renkli Fotokopi - PDF)", NameEn = "Passport (Color Copy - PDF)", EducationTypeId = universityEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 1, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "cv", Name = "CV (Almanca veya İngilizce)", NameEn = "CV (German or English)", EducationTypeId = universityEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf,.doc,.docx", MaxFileSizeBytes = 5242880, RequiresApproval = true, DisplayOrder = 2, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "residence", Name = "İkamet Belgesi (E-Devlet PDF)", NameEn = "Residence Certificate", EducationTypeId = universityEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 5242880, RequiresApproval = true, DisplayOrder = 3, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "university_diploma", Name = "Üniversite Diploması", NameEn = "University Diploma", EducationTypeId = universityEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 4, IconName = "award", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "yok_certificate", Name = "YÖK Mezun Belgesi (E-Devlet)", NameEn = "YÖK Graduate Certificate", EducationTypeId = universityEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 5242880, RequiresApproval = true, DisplayOrder = 7, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "transcript", Name = "Supplement - Transkript (E-Devlet)", NameEn = "Supplement - Transcript", EducationTypeId = universityEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 8, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "highschool_diploma", Name = "Lise Diploması/Mezun Belgesi (E-Devlet)", NameEn = "High School Diploma", EducationTypeId = universityEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 10, IconName = "award", IsActive = true, CreatedAt = DateTime.UtcNow },
                
                // Vocational Document Types
                new DocumentType { Code = "passport_vocational", Name = "Pasaport (Renkli Fotokopi - PDF)", NameEn = "Passport (Color Copy - PDF)", EducationTypeId = vocationalEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 1, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "cv_vocational", Name = "CV (Almanca veya İngilizce)", NameEn = "CV (German or English)", EducationTypeId = vocationalEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf,.doc,.docx", MaxFileSizeBytes = 5242880, RequiresApproval = true, DisplayOrder = 2, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "vocational_diploma", Name = "Meslek Lisesi Diploması", NameEn = "Vocational School Diploma", EducationTypeId = vocationalEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 3, IconName = "award", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "sgk_service", Name = "SGK Hizmet Dökümü (E-Devlet)", NameEn = "SGK Service Summary", EducationTypeId = vocationalEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 5242880, RequiresApproval = true, DisplayOrder = 5, IconName = "file-text", Note = "Çalışma geçmişinizi gösterir", IsActive = true, CreatedAt = DateTime.UtcNow },
                
                // Craftsman Document Types
                new DocumentType { Code = "passport_craftsman", Name = "Pasaport (Renkli Fotokopi - PDF)", NameEn = "Passport (Color Copy - PDF)", EducationTypeId = craftsmanEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 1, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "cv_craftsman", Name = "CV (Almanca veya İngilizce)", NameEn = "CV (German or English)", EducationTypeId = craftsmanEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf,.doc,.docx", MaxFileSizeBytes = 5242880, RequiresApproval = true, DisplayOrder = 2, IconName = "file-text", IsActive = true, CreatedAt = DateTime.UtcNow },
                new DocumentType { Code = "apprentice_certificate", Name = "Kalfalık Belgesi", NameEn = "Apprenticeship Certificate", EducationTypeId = craftsmanEducationType.Id, IsRequired = true, AllowedFileTypes = ".pdf", MaxFileSizeBytes = 10485760, RequiresApproval = true, DisplayOrder = 3, IconName = "award", IsActive = true, CreatedAt = DateTime.UtcNow }
            };

            await context.DocumentTypes.AddRangeAsync(documentTypes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedApplicationTemplatesAsync(WixiDbContext context)
        {
            if (await context.ApplicationTemplates.AnyAsync()) return;

            var template1 = new ApplicationTemplate
            {
                Name = "Denklik İşlem Süreci",
                NameEn = "Recognition Process",
                Description = "Diploma denklik işlemleri için standart süreç",
                Type = ApplicationType.Recognition,
                IsActive = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            };

            var template2 = new ApplicationTemplate
            {
                Name = "İş Bulma ve Çalışma Müsadesi Süreci",
                NameEn = "Work Permit Process",
                Description = "İş arama ve çalışma izni başvuru süreci",
                Type = ApplicationType.WorkPermit,
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            };

            var template3 = new ApplicationTemplate
            {
                Name = "Vize İşlem Süreci",
                NameEn = "Visa Process",
                Description = "Vize başvurusu ve takip süreci",
                Type = ApplicationType.Visa,
                IsActive = true,
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            };

            await context.ApplicationTemplates.AddRangeAsync(new[] { template1, template2, template3 });
            await context.SaveChangesAsync();

            // Add step templates
            var stepTemplate1 = new ApplicationStepTemplate
            {
                ApplicationTemplateId = template1.Id,
                Title = "Denklik İşlemleri",
                TitleEn = "Recognition Process",
                Description = "Diploma denklik işlemleri",
                StepOrder = 1,
                IconName = "file-check",
                IsRequired = true
            };

            var stepTemplate2 = new ApplicationStepTemplate
            {
                ApplicationTemplateId = template2.Id,
                Title = "İş Bulma ve Çalışma Müsadesi İşlemleri",
                TitleEn = "Work Permit Process",
                Description = "İş arama ve çalışma izni süreçleri",
                StepOrder = 1,
                IconName = "briefcase",
                IsRequired = true
            };

            var stepTemplate3 = new ApplicationStepTemplate
            {
                ApplicationTemplateId = template3.Id,
                Title = "Vize İşlemleri",
                TitleEn = "Visa Process",
                Description = "Vize başvurusu ve onay süreci",
                StepOrder = 1,
                IconName = "globe",
                IsRequired = true
            };

            await context.ApplicationStepTemplates.AddRangeAsync(new[] { stepTemplate1, stepTemplate2, stepTemplate3 });
            await context.SaveChangesAsync();

            // Add sub-step templates for Recognition
            var subSteps1 = new[]
            {
                new ApplicationSubStepTemplate { StepTemplateId = stepTemplate1.Id, Name = "Belgeler Yüklendi", NameEn = "Documents Uploaded", Description = "Gerekli belgeler sisteme yüklendi", SubStepOrder = 1, IsRequired = true },
                new ApplicationSubStepTemplate { StepTemplateId = stepTemplate1.Id, Name = "Denklik Başvurusu Yapıldı", NameEn = "Recognition Application Submitted", Description = "Denklik başvurusu ilgili kuruma yapıldı", SubStepOrder = 2, IsRequired = true },
                new ApplicationSubStepTemplate { StepTemplateId = stepTemplate1.Id, Name = "Denklik Harc Ödemesi Yapıldı", NameEn = "Recognition Fee Paid", Description = "Denklik işlem ücreti ödendi", SubStepOrder = 3, IsRequired = true },
                new ApplicationSubStepTemplate { StepTemplateId = stepTemplate1.Id, Name = "Denklik Belgesi Hazır", NameEn = "Recognition Certificate Ready", Description = "Denklik belgesi onaylandı ve hazır", SubStepOrder = 4, IsRequired = true }
            };

            await context.ApplicationSubStepTemplates.AddRangeAsync(subSteps1);
            await context.SaveChangesAsync();
        }

        private static async Task SeedFAQsAsync(WixiDbContext context)
        {
            if (await context.FAQs.AnyAsync()) return;

            var faqs = new[]
            {
                new FAQ
                {
                    Question = "Başvuru süreci ne kadar sürer?",
                    QuestionEn = "How long does the application process take?",
                    Answer = "Başvuru süreci ortalama 3-6 ay arasında tamamlanır.",
                    AnswerEn = "The application process typically takes 3-6 months to complete.",
                    Category = FAQCategory.General,
                    DisplayOrder = 1,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow
                },
                new FAQ
                {
                    Question = "Hangi belgeleri yüklemem gerekiyor?",
                    QuestionEn = "Which documents do I need to upload?",
                    Answer = "Yüklemeniz gereken belgeler eğitim durumunuza göre değişir.",
                    AnswerEn = "Required documents vary based on your education level.",
                    Category = FAQCategory.Documents,
                    DisplayOrder = 2,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow
                },
                new FAQ
                {
                    Question = "Başvuru durumumu nasıl takip edebilirim?",
                    QuestionEn = "How can I track my application status?",
                    Answer = "Dashboard sayfasından başvurunuzun tüm aşamalarını takip edebilirsiniz.",
                    AnswerEn = "You can track all stages of your application from the Dashboard page.",
                    Category = FAQCategory.Process,
                    DisplayOrder = 3,
                    IsActive = true,
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.FAQs.AddRangeAsync(faqs);
            await context.SaveChangesAsync();
        }
    }
}

