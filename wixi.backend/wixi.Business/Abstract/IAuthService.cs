using wixi.Entities.DTOs;
using wixi.Entities.Concrete;

namespace wixi.Business.Abstract
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(UserForLoginDto loginDto);
        Task<TokenDto> RegisterAsync(UserForRegisterDto registerDto);
        Task<TokenDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task<AppUser> GetUserByIdAsync(int userId);
    }
}

