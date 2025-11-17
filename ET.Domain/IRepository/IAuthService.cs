using ET.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET.Domain.IRepository
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterUser(AuthRegisterModel authRegisterModel);
        Task<AuthResponse> LoginUser(AuthLoginModel authLoginModel);
    }
}
