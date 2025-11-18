using ET.Domain.DTO;
using ET.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;

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
                return Ok(new { result.Message, Islogin = false });

            if (!result.Success)
                return BadRequest(new { result.Message, Islogin = false });

            return Ok(new { Message = "User registered successfully", Islogin = true });
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(AuthLoginModel model)
        {
            var result = await _authService.LoginUser(model);

            return !result.Success
                ? Ok(new { result.Message, Token = "", Success = false })
                : (IActionResult)Ok(new
                {
                    result.Token,
                    Success = true,
                    Message = "Login successful"
                });
        }
    }
}