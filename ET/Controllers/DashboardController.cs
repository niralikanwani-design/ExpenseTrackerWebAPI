using ET.Domain.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace ET.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController (IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("GetDashboardData/{userId}/{month}")]
        public async Task<IActionResult> GetDashboardData(int userId, int month)
        {
            var result = await _service.GetDashboardData(userId, month);
            return Ok(result);
        }

    }
}
