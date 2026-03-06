using System.Collections.Generic;
using System.Threading.Tasks;
using wixi.Entities.Concrete;

namespace wixi.Business.Abstract
{
    public interface IEmailTemplateService
    {
        Task<List<EmailTemplate>> GetAllAsync();
        Task<EmailTemplate?> GetByKeyAsync(string key);
        Task<EmailTemplate> UpsertAsync(EmailTemplate template, string? updatedBy = null);
        Task<bool> DeleteAsync(int id);
    }
}

