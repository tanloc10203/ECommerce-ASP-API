using ECommerce.Models.Auth;
using ECommerce.Models.Response;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.AuthService
{
    public interface IAuthRepository
    {
        public Task<IdentityResult> RegisterAsync(RegisterModel model);

        public Task<TokenModel?> LoginAsync(LoginModel model);

        public Task<ApiResponse> RefreshTokenAsync(TokenModel tokenModel);

        public Task<ApiResponse> LogoutAsync();

        public Task<ApiResponse> GetCurrentUser(string userId);
    }
}
