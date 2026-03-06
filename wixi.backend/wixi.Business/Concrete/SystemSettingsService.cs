using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.Business.Concrete
{
    public sealed class SystemSettingsService : ISystemSettingsService
    {
        private const string CacheKey = "system_settings_cache";
        private readonly WixiDbContext _db;
        private readonly IMemoryCache _cache;

        public SystemSettingsService(WixiDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<SystemSettings?> GetAsync()
        {
            if (_cache.TryGetValue(CacheKey, out SystemSettings cached))
                return cached;

            var settings = await _db.SystemSettings.AsNoTracking().FirstOrDefaultAsync();
            if (settings != null)
            {
                _cache.Set(CacheKey, settings, TimeSpan.FromMinutes(5));
            }
            return settings;
        }

        public async Task<SystemSettings> UpsertAsync(SystemSettings input, string? updatedBy = null)
        {
            var existing = await _db.SystemSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                input.UpdatedAt = DateTime.UtcNow;
                input.UpdatedBy = updatedBy;
                _db.SystemSettings.Add(input);
            }
            else
            {
                existing.SiteName = input.SiteName;
                existing.SiteUrl = input.SiteUrl;
                existing.AdminEmail = input.AdminEmail;
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

