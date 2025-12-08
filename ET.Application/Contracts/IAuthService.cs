using ET.Application.DTOs;

namespace ET.Application.Contracts;

public interface IAuthService
{
    Task<AuthResponse> RegisterUser(AuthRegisterModel authRegisterModel);
    Task<AuthResponse> LoginUser(AuthLoginModel authLoginModel);
    Task<(bool Success, string Token, string Message)> LoginWithGoogleAsync(string idToken);
    Task<bool> AddLimit(LimitModel limitModel);
}
