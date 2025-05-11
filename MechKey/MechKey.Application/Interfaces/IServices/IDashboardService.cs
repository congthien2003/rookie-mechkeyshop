using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Dashboard;

namespace Application.Interfaces.IServices
{
    public interface IDashboardService
    {
        Task<Result<DashboardData>> GetDashboardDataAsync(CancellationToken cancellationToken = default);
    }
}