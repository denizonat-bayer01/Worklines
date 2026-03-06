using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using wixi.Business.Abstract;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.Business.Concrete
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly WixiDbContext _db;

        public EmailTemplateService(WixiDbContext db)
        {
            _db = db;
        }

        public async Task<List<EmailTemplate>> GetAllAsync()
        {
            return await _db.EmailTemplates
                .OrderBy(t => t.Key)
                .ToListAsync();
        }

        public async Task<EmailTemplate?> GetByKeyAsync(string key)
        {
            return await _db.EmailTemplates
                .FirstOrDefaultAsync(t => t.Key == key);
        }

        public async Task<EmailTemplate> UpsertAsync(EmailTemplate input, string? updatedBy = null)
        {
            var existing = await _db.EmailTemplates
                .FirstOrDefaultAsync(t => t.Key == input.Key);

            if (existing == null)
            {
                input.CreatedAt = System.DateTime.UtcNow;
                input.UpdatedAt = System.DateTime.UtcNow;
                input.UpdatedBy = updatedBy;
                _db.EmailTemplates.Add(input);
            }
            else
            {
                existing.Subject = input.Subject;
                existing.BodyHtml = input.BodyHtml;
                existing.Description = input.Description;
                existing.IsActive = input.IsActive;
                existing.UpdatedAt = System.DateTime.UtcNow;
                existing.UpdatedBy = updatedBy;
            }

            await _db.SaveChangesAsync();
            return existing ?? input;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var template = await _db.EmailTemplates.FindAsync(id);
            if (template == null)
                return false;

            _db.EmailTemplates.Remove(template);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

