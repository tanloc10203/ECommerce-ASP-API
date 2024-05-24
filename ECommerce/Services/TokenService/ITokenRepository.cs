using ECommerce.Models.Auth;
using ECommerce.Models.Domain;
using ECommerce.Models.Response;

namespace ECommerce.Services.TokenService
{
    public interface ITokenRepository
    {
        public Task<TokenModel> GenerateToken(ApplicationUser user);

        public string GenerateRefreshToken();

        public Task<ApiResponse> HandleRefreshToken(TokenModel tokenModel);
    }
}
