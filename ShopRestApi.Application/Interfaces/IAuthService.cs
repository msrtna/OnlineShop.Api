using ShopRestApi.Application.DTOs.Auth;

namespace ShopRestApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<TokenResponseDto> LoginAsync(LoginDto dto);
        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
    }
}
