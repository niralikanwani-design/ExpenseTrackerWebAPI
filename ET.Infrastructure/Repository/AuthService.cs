using ET.Domain.DTO;
using ET.Domain.IRepository;
using ET.Domain.Models;
using ET.Infrastructure.Context;
using ET.Infrastructure.Repository;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly ExpenseTrackerContext _dbcontext;
    private readonly IConfiguration _config;
    private readonly IOptions<EmailSettings> _emailSettings;

    public AuthService(ExpenseTrackerContext dbcontext, IConfiguration config, IOptions<EmailSettings> emailSettings)
    {
        _dbcontext = dbcontext;
        _config = config;
        _emailSettings = emailSettings;
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

        string token = GenerateJwtToken(user);

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
        if(user == null)
            return new AuthResponse { Success = false, Message = "Invalid email or password" };

        var passwordHasher = new PasswordHasher<User>();

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Failed)
            return new AuthResponse { Success = false, Message = "Invalid email or password" };

        string token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Success = true,
            Token = token,
            Message = "Login successful"
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

        var claims = new[]
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Email", user.Email),
            new Claim("FullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_config["Jwt:ExpiryMinutes"])
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

            // Generate JWT Token
            var token = GenerateJwtToken(user);
            return (true, token, "Login successful");
        }
        catch (Exception ex)
        {
            return (false, "", ex.Message);
        }
    }

}
