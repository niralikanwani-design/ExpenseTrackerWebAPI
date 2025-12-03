using ET.Application.DTOs;

namespace ET.Application.Contracts;

public interface IDashboardService
{
    Task<DashboardModel> GetDashboardData(int userId, int month, string type);
}
