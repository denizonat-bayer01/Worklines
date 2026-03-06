using Microsoft.AspNetCore.Identity;
using wixi.DataAccess.Concrete.EntityFramework.Contexts;
using wixi.Entities.Concrete;

namespace wixi.WebAPI.Extensions
{
    public static class IdentityExt
    {
        public static IServiceCollection AddIdentityExt(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<WixiDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}

