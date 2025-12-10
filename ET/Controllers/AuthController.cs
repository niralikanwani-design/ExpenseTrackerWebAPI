using ET.Application.Contracts;
using ET.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ET.WebAPI.Controllers;

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

    [HttpPost("AddLimit")]
    public async Task<IActionResult> AddLimit([FromBody] LimitModel model)
    {
        var result = await _authService.AddLimit(model);

        if (!result)
            return BadRequest("User not found or update failed");

        return Ok("Limit updated successfully");
    }

    [HttpGet("GetUserData")]
    public async Task<IActionResult> GetUserData(int userid)
    {
        var result = await _authService.GetUserData(userid);

        if (result == null)
            return BadRequest("User not found");

        return Ok(result);
    }

    [HttpPost("EditUserData")]
    public async Task<IActionResult> EditUserData(EditUserData editUserData)
    {
        var result = await _authService.EditUserData(editUserData);
        return Ok(result);
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _authService.ChangePassword(request.Email);

        if (result)
            return Ok("Email sent successfully");

        return BadRequest("Something went wrong");
    }

    [HttpPost("SetPassword")]
    public async Task<IActionResult> SetPassword([FromBody] Dictionary<string, string> data)
    {
        if (!data.ContainsKey("email") || !data.ContainsKey("password"))
            return BadRequest("Email and password required.");

        string email = data["email"];
        string password = data["password"];

        var result = await _authService.SetPassword(email, password);

        if (result == "")
            return BadRequest("Failed to update password.");

        return Ok(new { message = "Password changed successfully" });
    }

}
