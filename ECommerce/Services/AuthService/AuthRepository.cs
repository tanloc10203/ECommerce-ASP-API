using AutoMapper;
using ECommerce.Models;
using ECommerce.Models.Auth;
using ECommerce.Models.Domain;
using ECommerce.Models.Response;
using ECommerce.Services.TokenService;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.AuthService
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            ITokenRepository tokenRepository
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _roleManager = roleManager;
            _tokenRepository = tokenRepository;
        }

        public async Task<ApiResponse> GetCurrentUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ApiResponse
                {
                    StatusCode = 404,
                    Data = null,
                    Message = "Current User Not Found",
                    Success = false,
                };
            }

            var role = await _userManager.GetRolesAsync(user);

            var result = _mapper.Map<UserModel>(user);

            result.Role = role?.ToList();

            return new ApiResponse
            {
                StatusCode = 200,
                Data = result,
                Message = "Get Current User Success",
                Success = true,
            };
        }

        public async Task<ApiResponse> LogoutAsync()
        {
            await _signInManager.SignOutAsync();

            return new ApiResponse
            {
                Data = null,
                Message = "Logout Success",
                StatusCode = StatusCodes.Status200OK,
                Success = true,
            };
        }

        public async Task<ApiResponse> RefreshTokenAsync(TokenModel tokenModel)
        {
            return await _tokenRepository.HandleRefreshToken(tokenModel);
        }

        async Task<TokenModel?> IAuthRepository.LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return null;
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
            {
                return null;
            }

            return await _tokenRepository.GenerateToken(user);
        }

        async Task<IdentityResult> IAuthRepository.RegisterAsync(RegisterModel model)
        {
            var user = _mapper.Map<ApplicationUser>(model);

            user.UserName = model.Email;

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kiểm tra role Customer đã có chưa.
                if (!await _roleManager.RoleExistsAsync(UserRoles.Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));
                }

                // Kiểm tra role Admin đã có chưa.
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                }

                await _userManager.AddToRoleAsync(user, UserRoles.Customer);
            }

            return result;
        }
    }
}
