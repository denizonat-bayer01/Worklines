using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.Business.Concrete
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly WixiDbContext _db;

        public UserPreferenceService(WixiDbContext db)
        {
            _db = db;
        }

        public async Task<UserPreference> GetOrCreateAsync(string userId)
        {
            var pref = await _db.UserPreferences.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId);
            if (pref != null) return pref;
            var created = new UserPreference { UserId = userId, Language = "de", Theme = "light", UpdatedAt = DateTime.UtcNow };
            _db.UserPreferences.Add(created);
            await _db.SaveChangesAsync();
            return created;
        }

        public async Task<UserPreference> UpsertAsync(string userId, string? language, string? theme)
        {
            var pref = await _db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (pref == null)
            {
                pref = new UserPreference { UserId = userId };
                _db.UserPreferences.Add(pref);
            }
            if (!string.IsNullOrWhiteSpace(language)) pref.Language = language!;
            if (!string.IsNullOrWhiteSpace(theme)) pref.Theme = theme!;
            pref.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return pref;
        }
    }
}


