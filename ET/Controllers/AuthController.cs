using ET.Domain.DTO;
using ET.Domain.IRepository;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ET.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(AuthRegisterModel model)
        {
            var result = await _authService.RegisterUser(model);

            if (result.Message == "Email already registered")
                return Ok(new { Message = result.Message, Islogin = false, Token = "" });

            if (!result.Success)
                return BadRequest(new { Message = result.Message, Islogin = false });

            return Ok(new { Message = "User registered successfully", Islogin = true, Token = result.Token });
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(AuthLoginModel model)
        {
            var result = await _authService.LoginUser(model);

            return !result.Success
                ? Ok(new { Message = result.Message, Token = "", Success = false })
                : (IActionResult)Ok(new
                {
                    Token = result.Token,
                    Success = true,
                    Message = "Login successful"
                });
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdToken))
                    return BadRequest(new { success = false, message = "Google token is required" });

                var result = await _authService.LoginWithGoogleAsync(request.IdToken);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new
                {
                    success = true,
                    token = result.Token,
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}