using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kinepalisse.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly DashboardService _dash;
    public AdminController(DashboardService dash) => _dash = dash;

    [HttpGet("dashboard")]
    public Task<DashboardService.DashboardDto> Dashboard() => _dash.GetAsync();
}
