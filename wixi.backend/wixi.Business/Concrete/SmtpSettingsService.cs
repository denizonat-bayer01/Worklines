using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.Core.Utilities.Security.Protection;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.Business.Concrete
{
    public sealed class SmtpSettingsService : ISmtpSettingsService
    {
        private const string CacheKey = "smtp_settings_cache";
        private readonly WixiDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ISecretProtector _protector;

        public SmtpSettingsService(WixiDbContext db, IMemoryCache cache, ISecretProtector protector)
        {
            _db = db;
            _cache = cache;
            _protector = protector;
        }

        public async Task<SmtpSettings?> GetAsync()
        {
            if (_cache.TryGetValue(CacheKey, out SmtpSettings cached))
                return cached;

            var settings = await _db.SmtpSettings.AsNoTracking().FirstOrDefaultAsync();
            if (settings != null)
            {
                _cache.Set(CacheKey, settings, TimeSpan.FromMinutes(5));
            }
            return settings;
        }

        public async Task<SmtpSettings> UpsertAsync(SmtpSettings input, string? updatedBy = null)
        {
            var existing = await _db.SmtpSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                // Store password directly without encryption
                input.PasswordEnc = input.PasswordEnc ?? string.Empty;
                input.UpdatedAt = DateTime.UtcNow;
                input.UpdatedBy = updatedBy;
                _db.SmtpSettings.Add(input);
            }
            else
            {
                existing.Host = input.Host;
                existing.Port = input.Port;
                existing.UseSsl = input.UseSsl;
                existing.UserName = input.UserName;
                if (!string.IsNullOrWhiteSpace(input.PasswordEnc))
                {
                    // Store password directly without encryption
                    existing.PasswordEnc = input.PasswordEnc;
                }
                existing.FromName = input.FromName;
                existing.FromEmail = input.FromEmail;
                existing.TimeoutMs = input.TimeoutMs;
                existing.RetryCount = input.RetryCount;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = updatedBy;
            }

            await _db.SaveChangesAsync();
            InvalidateCache();
            return (await GetAsync())!;
        }

        public void InvalidateCache() => _cache.Remove(CacheKey);
    }
}

