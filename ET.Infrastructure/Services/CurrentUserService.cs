using ET.Application.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ET.Infrastructure.Services;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ClaimsPrincipal CurrentUser;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = httpContextAccessor;
        CurrentUser = _contextAccessor.HttpContext.User;
    }

    public string GetCurrentUserName()
    {
        if (CurrentUser.Identity?.IsAuthenticated ?? false)
        {
            return CurrentUser.FindFirstValue("") ?? "Unknown User";
        }

        return "Unknown User";
    }

    public int GetCurrentUserId()
    {
        if (CurrentUser.Identity?.IsAuthenticated ?? false)
        {
            return int.Parse(CurrentUser.FindFirstValue("UserId") ?? "0");
        }

        return 0;
    }

    public string GetCurrentUserRole()
    {
        if (CurrentUser.Identity?.IsAuthenticated ?? false)
        {
            return CurrentUser.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        return string.Empty;
    }
}
