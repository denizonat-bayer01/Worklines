using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.Business.Concrete
{
    public sealed class TranslationService : ITranslationService
    {
        private readonly WixiDbContext _db;
        private readonly IMemoryCache _cache;
        private static string CacheKey(string lang) => $"i18n_{lang}";

        public TranslationService(WixiDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<Dictionary<string, string>> GetTranslationsAsync(string lang)
        {
            if (_cache.TryGetValue(CacheKey(lang), out Dictionary<string, string> cached))
                return cached;

            var dict = await _db.Translations.AsNoTracking()
                .Select(t => new { t.Key, Value = lang == "de" ? t.De : lang == "tr" ? t.Tr : lang == "en" ? t.En : t.Ar })
                .Where(x => x.Value != null)
                .ToDictionaryAsync(x => x.Key, x => x.Value!);

            _cache.Set(CacheKey(lang), dict, TimeSpan.FromMinutes(10));
            return dict;
        }

        public async Task<IReadOnlyList<Translation>> ListAsync(string? search = null, int page = 1, int pageSize = 50)
        {
            var q = _db.Translations.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(t => t.Key.Contains(search));
            }
            return await q
                .OrderBy(t => t.Key)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Translation?> GetByKeyAsync(string key)
        {
            return _db.Translations.AsNoTracking().FirstOrDefaultAsync(t => t.Key == key);
        }

        public async Task<Translation> UpsertAsync(Translation item, string? updatedBy = null)
        {
            var existing = await _db.Translations.FirstOrDefaultAsync(t => t.Key == item.Key);
            if (existing == null)
            {
                item.UpdatedAt = DateTime.UtcNow;
                item.UpdatedBy = updatedBy;
                _db.Translations.Add(item);
            }
            else
            {
                existing.De = item.De;
                existing.Tr = item.Tr;
                existing.En = item.En;
                existing.Ar = item.Ar;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = updatedBy;
            }
            await _db.SaveChangesAsync();
            InvalidateCache(null);
            return (await GetByKeyAsync(item.Key))!;
        }

        public async Task DeleteAsync(long id)
        {
            var e = await _db.Translations.FindAsync(id);
            if (e == null) return;
            _db.Translations.Remove(e);
            await _db.SaveChangesAsync();
            InvalidateCache(null);
        }

        public void InvalidateCache(string? lang = null)
        {
            if (lang != null)
            {
                _cache.Remove(CacheKey(lang));
                return;
            }
            foreach (var l in new[] { "de", "tr", "en", "ar" })
            {
                _cache.Remove(CacheKey(l));
            }
        }
    }
}


