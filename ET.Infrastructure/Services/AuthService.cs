using ET.Application.Contracts;
using ET.Application.DTOs;
using ET.Domain.Entities;
using ET.Infrastructure.Persistance.Context;
using ET.Infrastructure.Repository;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ET.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ExpenseTrackerContext _dbcontext;
    private readonly IConfiguration _config;
    private readonly IOptions<EmailSettings> _emailSettings;
    private readonly ICurrentUserService _currentUserService;

    public AuthService(ExpenseTrackerContext dbcontext, IConfiguration config, IOptions<EmailSettings> emailSettings, ICurrentUserService currentUserService)
    {
        _dbcontext = dbcontext;
        _config = config;
        _emailSettings = emailSettings;
        _currentUserService = currentUserService;
    }

    public async Task<AuthResponse> RegisterUser(AuthRegisterModel model)
    {
        var response = new AuthResponse();

        if (string.IsNullOrWhiteSpace(model.FullName))
            return new AuthResponse { Success = false, Message = "Full name is required" };

        if (string.IsNullOrWhiteSpace(model.Email))
            return new AuthResponse { Success = false, Message = "Email is required" };

        if (string.IsNullOrWhiteSpace(model.Password))
            return new AuthResponse { Success = false, Message = "Password is required" };

        // Check existing user
        if (await _dbcontext.Users.AnyAsync(u => u.Email == model.Email))
            return new AuthResponse { Success = false, Message = "Email already registered" };
        var passwordHasher = new PasswordHasher<User>();

        User user = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

        string token = await GenerateJwtToken(user);

        await _dbcontext.Users.AddAsync(user);
        await _dbcontext.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Token = token,
            Message = "User registered successfully"
        };
    }

    public async Task<AuthResponse> LoginUser(AuthLoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
            return new AuthResponse { Success = false, Message = "Email is required" };

        if (string.IsNullOrWhiteSpace(model.Password))
            return new AuthResponse { Success = false, Message = "Password is required" };

        var user = await _dbcontext.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
            return new AuthResponse { Success = false, Message = "Invalid email or password" };

        var passwordHasher = new PasswordHasher<User>();

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Failed)
            return new AuthResponse { Success = false, Message = "Invalid email or password" };

        string token = await GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Token = token,
            Message = "Login successful"
        };
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

        var role = await GetUserRole(user.UserId);

        var claims = new[]
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Email", user.Email),
            new Claim("FullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_config["Jwt:AccessTokenExpiryMinutes"])
            ),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(bool Success, string Token, string Message)> LoginWithGoogleAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            string email = payload.Email;
            string fullName = payload.Name;

            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
            var passwordHasher = new PasswordHasher<User>();
            if (user == null)
            {
                var newPassword = Guid.NewGuid().ToString().Substring(0, 8);
                user = new User
                {
                    FullName = fullName,
                    Email = email,
                    CreatedAt = DateTime.UtcNow,
                };
                user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
                _dbcontext.Users.Add(user);
                await _dbcontext.SaveChangesAsync();
                var emailBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Welcome to Expenss Tracker!</h2>
                        <p>Hi <strong>{fullName}</strong>,</p>
                        <p>Your account was created automatically when you used Google login.</p>
                        <p>Your generated password:</p>
                        <h3>{newPassword}</h3>
                        <p>You can change this password after login.</p>
                    </div>";

                await SmtpEmailHelper.SendEmailAsync(email: email,
                     subject: "Welcome to Expenss tracker - Your Password",
                message: emailBody,
                     emailSettings: _emailSettings.Value);
            }

            var token = await GenerateJwtToken(user);
            return (true, token, "Login successful");
        }
        catch (Exception ex)
        {
            return (false, "", ex.Message);
        }
    }

    public async Task<string> GetUserRole(int userId)
    {
        return await _dbcontext.Users
            .Where(u => u.UserId == userId)
            .Select(u => u.Role)
            .FirstOrDefaultAsync()
            ?? "User";
    }

    public async Task<bool> AddLimit(LimitModel limitModel)
    {
        try
        {
            var user = await _dbcontext.Users
                                     .FirstOrDefaultAsync(x => x.UserId == limitModel.UserId);

            if (user == null)
                return false; // user not found

            // Update fields
            user.TotalBalance = limitModel.TotalBalance;
            user.MaxLimit = limitModel.MaxLimit;

            // Save changes
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<User> GetUserData(int userId)
    {
        try
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return null;
            }

            return user;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<string> EditUserData(EditUserData editUserData)
    {
        try
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.UserId == editUserData.UserId);

            if (user == null)
                return "User not found";

            if (string.IsNullOrWhiteSpace(editUserData.FullName))
                return "Full Name cannot be empty";

            if (string.IsNullOrWhiteSpace(editUserData.Email))
                return "Email cannot be empty";

            user.FullName = editUserData.FullName;
            user.TotalBalance = editUserData.TotalBalance;
            user.MaxLimit = editUserData.MaxLimit;
            await _dbcontext.SaveChangesAsync();
            return "Your profile update successfully!!";

        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<bool> ChangePassword(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email cannot be empty.");

            var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                throw new Exception("User not found.");

            string name = user.FullName ?? "User";

            string resetLink = $"http://localhost:5173/ChangePassword?email={email}";

            string emailBody = $@"
        <table width='100%' cellspacing='0' cellpadding='0' 
               style='font-family: Arial, sans-serif; background:#f7f7f7; padding: 30px 0;'>
            <tr>
                <td align='center'>
                    <table width='600' cellspacing='0' cellpadding='0' 
                           style='background:#ffffff; border-radius:8px; padding:30px; box-shadow:0 4px 10px rgba(0,0,0,0.1);'>
                        
                        <tr>
                            <td style='text-align:center; padding-bottom:20px;'>
                                <h2 style='color:#333; margin:0;'>Expense Tracker</h2>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <p style='font-size:16px; color:#333;'>Hello <strong>{name}</strong>,</p>
                                <p style='font-size:15px; color:#555; line-height:1.6;'>
                                    You requested to reset your password.  
                                    Click the button below to set a new password.
                                </p>

                                <div style='text-align:center; margin:25px 0;'>
                                    <a href='{resetLink}'
                                       style='background:#0066ff; color:#ffffff; padding:12px 25px;
                                              font-size:16px; text-decoration:none; border-radius:6px;
                                              display:inline-block;'>
                                        Reset Password
                                    </a>
                                </div>

                                <p style='font-size:14px; color:#777; line-height:1.6;'>
                                    If you did not request this, you can safely ignore this email.
                                </p>

                                <p style='font-size:14px; color:#999; padding-top:20px;'>
                                    Thank you,<br>
                                    <strong>Expense Tracker Team</strong>
                                </p>
                            </td>
                        </tr>

                    </table>
                </td>
            </tr>
        </table>";

            emailBody = emailBody;

            await SmtpEmailHelper.SendEmailAsync(
                email: "yashvariya20@gmail.com",
                subject: "Expense Tracker - Reset Your Password",
                message: emailBody,
                emailSettings: _emailSettings.Value
            );

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("ChangePassword failed: " + ex.Message);
        }
    }

    public async Task<string> SetPassword(string email, string password)
    {
        try
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return "User not found";

            var passwordHasher = new PasswordHasher<User>();

            // Hash new password
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            _dbcontext.Users.Update(user);
            await _dbcontext.SaveChangesAsync();

            return "Password updated successfully";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
