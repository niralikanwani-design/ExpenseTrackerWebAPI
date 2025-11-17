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
        Task<string> RegisterUser(AuthRegisterModel authRegisterModel);
        Task<string> LoginUser(AuthLoginModel authLoginModel);
    }
}
