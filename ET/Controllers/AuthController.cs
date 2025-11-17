using ET.Domain.DTO;
using ET.Domain.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _IAuthService;
        public AuthController(IAuthService IAuthService)
        {
            _IAuthService = IAuthService;
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult> RegisterUser(AuthRegisterModel authRegisterModel)
        {
            var result = await _IAuthService.RegisterUser(authRegisterModel);
            if (result == null || !result.Any())
                return BadRequest("Please enter valid data");
            return Ok(new
            {
                Token = result,
                Message = "User register successfull"
            });
        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult> LoginUser(AuthLoginModel authLoginModel)
        {
            var result = await _IAuthService.LoginUser(authLoginModel);
            if (result == null || !result.Any())
                return BadRequest("Please enter valid data");
            return Ok(new
            {
                Token = result,
                Message = "Login successfull"
            });
        }
    }
}
