using wixi.Entities.Concrete;
using wixi.Entities.Concrete.Identity;
using wixi.Entities.DTOs;

namespace wixi.Core.Utilities.Security.JWT
{
    public interface ITokenHelper
    {
        Task<TokenDto> CreateToken(AppUser user);
    }
}

