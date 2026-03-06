using System.Threading.Tasks;
using wixi.Entities.Concrete;

namespace wixi.Business.Abstract
{
    public interface ISystemSettingsService
    {
        Task<SystemSettings?> GetAsync();
        Task<SystemSettings> UpsertAsync(SystemSettings input, string? updatedBy = null);
        void InvalidateCache();
    }
}

