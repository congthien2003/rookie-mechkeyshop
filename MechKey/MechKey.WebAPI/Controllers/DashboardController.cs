using Application.Catalog.Queries;
using Application.Comoon;
using Asp.Versioning;
using MediatR;
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
        private readonly ISender _sender;

        public DashboardController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<Result<DashboardData>> GetDashboardData(CancellationToken cancellationToken = default)
        {
            return await _sender.Send(new GetDashboardQuery(), cancellationToken);
        }
    }
}
