using ET.Domain.DTO;

namespace ET.Domain.IRepository
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterUser(AuthRegisterModel authRegisterModel);
        Task<AuthResponse> LoginUser(AuthLoginModel authLoginModel);
    }
}
