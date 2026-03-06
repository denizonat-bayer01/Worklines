using System.Threading.Tasks;
using wixi.Entities.Concrete;

namespace wixi.Business.Abstract
{
    public interface IContentSettingsService
    {
        Task<ContentSettings?> GetAsync();
        Task<ContentSettings> UpsertAsync(ContentSettings input, string? updatedBy = null);
        void InvalidateCache();
    }
}

