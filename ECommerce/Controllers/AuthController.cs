using ECommerce.Models.Auth;
using ECommerce.Models.Response;
using ECommerce.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                var result = await _authRepository.RegisterAsync(registerModel);

                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created, new ApiResponse
                    {
                        Data = result,
                        Message = "Register success",
                        Success = true
                    });
                }

                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                {
                    Data = null,
                    Message = string.Join("", result.Errors.Select((t) => t.Description)),
                    Success = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var result = await _authRepository.LoginAsync(loginModel);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Data = null,
                        Message = "Wrong email or password",
                        Success = false,
                        StatusCode = StatusCodes.Status401Unauthorized
                    });
                }

                return StatusCode(StatusCodes.Status200OK, new ApiResponse
                {
                    Data = result,
                    Message = "Authenticate success",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                });
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            try
            {
                var response = await _authRepository.RefreshTokenAsync(tokenModel);

                int status = response.StatusCode ?? StatusCodes.Status200OK;

                return StatusCode(status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                });
            }
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var response = await _authRepository.LogoutAsync();

                int status = response.StatusCode ?? StatusCodes.Status200OK;

                return StatusCode(status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                });
            }
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public async Task<IActionResult> CurrentUser()
        {
            try
            {
                var userId = HttpContext.User.FindFirstValue("Id");

                var response = await _authRepository.GetCurrentUser(userId!);

                int status = response.StatusCode ?? StatusCodes.Status200OK;

                return StatusCode(status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                });
            }
        }
    }
}
