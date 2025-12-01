using ET.Domain.DTO;
using ET.Domain.Models;

namespace ET.Domain.IRepository
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterUser(AuthRegisterModel authRegisterModel);
        Task<AuthResponse> LoginUser(AuthLoginModel authLoginModel);
        Task<(bool Success, string Token, string Message)> LoginWithGoogleAsync(string idToken);
    }
}
