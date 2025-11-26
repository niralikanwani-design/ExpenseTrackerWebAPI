using ET.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET.Domain.IRepository
{
    public interface IDashboardService
    {
        Task<DashboardModel> GetDashboardData(int userId, int month, string type);
    }
}
