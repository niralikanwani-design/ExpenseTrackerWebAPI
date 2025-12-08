using ET.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ET.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    //[Authorize(Roles = "Admin")]
    [HttpGet("GetDashboardData/{userId}/{month}/{type}")]
    public async Task<IActionResult> GetDashboardData(int userId, int month, string type)
    {
        var result = await _service.GetDashboardData(userId, month, type);
        return Ok(result);
    }

}
