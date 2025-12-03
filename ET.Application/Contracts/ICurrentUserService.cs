namespace ET.Application.Contracts;

public interface ICurrentUserService
{
    string GetCurrentUserName();
    int GetCurrentUserId();
}
