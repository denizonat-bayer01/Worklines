using System.Collections.Generic;
using System.Threading.Tasks;
using wixi.Entities.Concrete;

namespace wixi.Business.Abstract
{
    public interface ITranslationService
    {
        Task<Dictionary<string, string>> GetTranslationsAsync(string lang);
        Task<IReadOnlyList<Translation>> ListAsync(string? search = null, int page = 1, int pageSize = 50);
        Task<Translation?> GetByKeyAsync(string key);
        Task<Translation> UpsertAsync(Translation item, string? updatedBy = null);
        Task DeleteAsync(long id);
        void InvalidateCache(string? lang = null);
    }
}


