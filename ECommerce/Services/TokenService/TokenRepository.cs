using ECommerce.Data;
using ECommerce.Models.Auth;
using ECommerce.Models.Domain;
using ECommerce.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Services.TokenService
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly BaseDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenRepository(
            IConfiguration configuration,
            BaseDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        public string GenerateRefreshToken()
        {
            var random = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        public async Task<TokenModel> GenerateToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", user.Id.ToString()),
            };

            // Lấy role ra và thêm vào payload jwt
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddSeconds(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            // Lưu database
            var refreshTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                Token = refreshToken,
                IsUsed = false,
                UserId = user.Id,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };

            await _context.UserRefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task<ApiResponse> HandleRefreshToken(TokenModel tokenModel)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!);

            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ValidateLifetime = false, // Không kiểm tra hết hạn token
            };


            // Check 1: AccessToken có đúng format ko?
            var tokenInVerification = jwtTokenHandler.ValidateToken(
                tokenModel.AccessToken,
                tokenValidateParam,
                out var validatedToken
            );

            // Check 2: Check thuận toán sử dụng có đúng ko?
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256Signature,
                    StringComparison.InvariantCultureIgnoreCase  // Check ko phân biệt chữ hoa thường
                );

                if (!result)
                {
                    return new ApiResponse
                    {
                        Message = "Invalid AccessToken",
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Data = null
                    };
                }
            }

            // Check 3: Check accessToken đã hết hạn chưa mà đi refreshToken?
            var tokenExp = tokenInVerification.Claims.FirstOrDefault(p
                => p.Type.Equals(JwtRegisteredClaimNames.Exp));

            var utcExpireDate = long.Parse(tokenExp!.Value);

            var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);

            if (expireDate >= DateTime.UtcNow)
            {
                return new ApiResponse
                {
                    Message = "Access token has not yet expired",
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }

            // Check 4: Check refreshToken exist in DB
            var storedToken = await _context.UserRefreshTokens.FirstOrDefaultAsync(x => x.Token.Equals(tokenModel.RefreshToken));

            if (storedToken == null)
            {
                return new ApiResponse
                {
                    Message = "Refresh token does not exists",
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }

            // Check 5. Check refreshToken đã sử dụng chưa hoặc đã thu hồi chưa
            if (storedToken.IsUsed)
            {
                return new ApiResponse
                {
                    Message = "Refresh token has been used",
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }

            if (storedToken.IsRevoked)
            {
                return new ApiResponse
                {
                    Message = "Refresh token has been revoked",
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };

            }

            // Check 6: Kiểm tra AccessToken Id = JwtId ở trong refreshToken (DB)
            var jti = tokenInVerification?.Claims?.FirstOrDefault(x => x.Type.Equals(JwtRegisteredClaimNames.Jti))?.Value;

            if (!storedToken.JwtId.Equals(jti))
            {
                return new ApiResponse
                {
                    Message = "Token doesn't match",
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }

            // Update token is used
            storedToken.IsUsed = true;
            storedToken.IsRevoked = true;

            _context.Update(storedToken);
            await _context.SaveChangesAsync();

            // Create new Token
            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            var token = await GenerateToken(user!);

            return new ApiResponse
            {
                Success = true,
                Message = "RefreshToken success",
                Data = token,
                StatusCode = StatusCodes.Status200OK,
            };
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            return DateTimeOffset.FromUnixTimeSeconds(utcExpireDate).UtcDateTime;
        }
    }
}
