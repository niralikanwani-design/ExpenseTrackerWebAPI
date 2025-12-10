using ET.Application.DTOs;
using ET.Domain.Entities;

namespace ET.Application.Contracts;

public interface IAuthService
{
    Task<AuthResponse> RegisterUser(AuthRegisterModel authRegisterModel);
    Task<AuthResponse> LoginUser(AuthLoginModel authLoginModel);
    Task<(bool Success, string Token, string Message)> LoginWithGoogleAsync(string idToken);
    Task<bool> AddLimit(LimitModel limitModel);
    Task<User> GetUserData(int userId);
    Task<string> EditUserData(EditUserData editUserData);
    Task<bool> ChangePassword(string email);
    Task<string> SetPassword(string email, string password);
}
