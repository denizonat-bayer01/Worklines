using Microsoft.EntityFrameworkCore;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;

namespace wixi.WebAPI.Extensions
{
    public static class DbContextExt
    {
        public static IServiceCollection AddDbContextExt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WixiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}

