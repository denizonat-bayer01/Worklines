using System.Threading.Tasks;
using wixi.Entities.Concrete;

namespace wixi.Business.Abstract
{
    public interface IUserPreferenceService
    {
        Task<UserPreference> GetOrCreateAsync(string userId);
        Task<UserPreference> UpsertAsync(string userId, string? language, string? theme);
    }
}


