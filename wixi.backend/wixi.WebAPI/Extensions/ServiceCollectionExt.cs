using wixi.Business.Abstract;
using wixi.Business.Concrete;
using wixi.Core.Utilities.Security.JWT;

namespace wixi.WebAPI.Extensions
{
    public static class ServiceCollectionExt
    {
        public static IServiceCollection AddServiceCollectionExt(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthManager>();
            services.AddScoped<ISmtpSettingsService, SmtpSettingsService>();
            services.AddScoped<ISystemSettingsService, SystemSettingsService>();
            services.AddScoped<IContentSettingsService, ContentSettingsService>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<IUserPreferenceService, UserPreferenceService>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<ISmtpHealthCheck, SmtpHealthCheck>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IDocumentReviewService, DocumentReviewService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ISupportService, SupportService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddMemoryCache();
            services.AddScoped<ITokenHelper, JwtHelper>();

            return services;
        }
    }
}

