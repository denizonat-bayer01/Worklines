using System.Threading.Tasks;
using wixi.Entities.Concrete;

namespace wixi.Business.Abstract
{
    public interface ISmtpSettingsService
    {
        Task<SmtpSettings?> GetAsync();
        Task<SmtpSettings> UpsertAsync(SmtpSettings input, string? updatedBy = null);
        void InvalidateCache();
    }
}

