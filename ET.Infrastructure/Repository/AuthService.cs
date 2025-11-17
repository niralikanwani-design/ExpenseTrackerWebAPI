using ET.Domain.DTO;
using ET.Domain.IRepository;
using ET.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ET.Infrastructure.Repository
{
    public class AuthService : IAuthService
    {
        private readonly ExpensTrackerContext _dbcontext;
        private readonly IConfiguration _config;
        public AuthService(ExpensTrackerContext dbcontext, IConfiguration config)
        {
            _dbcontext = dbcontext;
            _config = config;
        }

        public async Task<string> RegisterUser(AuthRegisterModel authRegisterModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authRegisterModel.Email))
                    return "Email is required";

                if (string.IsNullOrWhiteSpace(authRegisterModel.Password))
                    return "Password is required";

                if (string.IsNullOrWhiteSpace(authRegisterModel.FullName))
                    return "FullName is required";

                User user = new User();
                user.FullName = authRegisterModel.FullName;
                user.Email = authRegisterModel.Email;
                user.PasswordHash = authRegisterModel.Password;
                user.CreatedAt = authRegisterModel.CreatedAt;
                _dbcontext.AddAsync(user);
                await _dbcontext.SaveChangesAsync();
                string token = GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> LoginUser(AuthLoginModel authLoginModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authLoginModel.Email))
                    return "Email is required";

                if (string.IsNullOrWhiteSpace(authLoginModel.Password))
                    return "Password is required";

                var user = await _dbcontext.Users
                    .FirstOrDefaultAsync(u => u.Email == authLoginModel.Email);

                if (user == null)
                    return "Invalid email or password";

                if (authLoginModel.Password != user.PasswordHash)
                    return "Invalid email or password";
                
                string token = GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("FullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
