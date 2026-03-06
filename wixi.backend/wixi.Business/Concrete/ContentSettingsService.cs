using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.Business.Concrete
{
    public sealed class ContentSettingsService : IContentSettingsService
    {
        private const string CacheKey = "content_settings_cache";
        private readonly WixiDbContext _db;
        private readonly IMemoryCache _cache;

        public ContentSettingsService(WixiDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<ContentSettings?> GetAsync()
        {
            if (_cache.TryGetValue(CacheKey, out ContentSettings cached))
                return cached;

            var settings = await _db.ContentSettings.AsNoTracking().FirstOrDefaultAsync();
            if (settings != null)
            {
                _cache.Set(CacheKey, settings, TimeSpan.FromMinutes(5));
            }
            return settings;
        }

        public async Task<ContentSettings> UpsertAsync(ContentSettings input, string? updatedBy = null)
        {
            var existing = await _db.ContentSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                input.UpdatedAt = DateTime.UtcNow;
                input.UpdatedBy = updatedBy;
                _db.ContentSettings.Add(input);
            }
            else
            {
                existing.FooterCompanyDescDe = input.FooterCompanyDescDe;
                existing.FooterCompanyDescTr = input.FooterCompanyDescTr;
                existing.FooterCompanyDescEn = input.FooterCompanyDescEn;
                existing.FooterCompanyDescAr = input.FooterCompanyDescAr;
                existing.FacebookUrl = input.FacebookUrl;
                existing.InstagramUrl = input.InstagramUrl;
                existing.TwitterUrl = input.TwitterUrl;
                existing.LinkedInUrl = input.LinkedInUrl;
                existing.AboutMissionText1De = input.AboutMissionText1De;
                existing.AboutMissionText1Tr = input.AboutMissionText1Tr;
                existing.AboutMissionText1En = input.AboutMissionText1En;
                existing.AboutMissionText1Ar = input.AboutMissionText1Ar;
                existing.AboutMissionText2De = input.AboutMissionText2De;
                existing.AboutMissionText2Tr = input.AboutMissionText2Tr;
                existing.AboutMissionText2En = input.AboutMissionText2En;
                existing.AboutMissionText2Ar = input.AboutMissionText2Ar;
                existing.ContactPhone = input.ContactPhone;
                existing.ContactEmail = input.ContactEmail;
                existing.AddressGermany = input.AddressGermany;
                existing.AddressTurkeyMersin = input.AddressTurkeyMersin;
                existing.AddressTurkeyIstanbul = input.AddressTurkeyIstanbul;
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

