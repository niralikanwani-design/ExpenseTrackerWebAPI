using System.ComponentModel.DataAnnotations;

namespace ET.Application.DTOs;

public class AuthRegisterModel
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class AuthLoginModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(100, ErrorMessage = "Email must be less than or equal to 100 characters.")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 6 characters long.")]
    [MaxLength(100, ErrorMessage = "Password must be less than or equal to 100 characters.")]
    public string Password { get; set; } = null!;
}
public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
}

public class GoogleLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class GoogleLoginModel
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string GoogleUserId { get; set; } = null!;
}

public class LimitModel
{
    public int UserId { get; set; }
    public int TotalBalance { get; set; }
    public int MaxLimit { get; set; }
}