using Application.Comoon;
using Application.Interfaces.IServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Dashboard;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<Result<DashboardData>> GetDashboardData(CancellationToken cancellationToken = default)
        {
            return await _dashboardService.GetDashboardDataAsync(cancellationToken);
        }
    }
}