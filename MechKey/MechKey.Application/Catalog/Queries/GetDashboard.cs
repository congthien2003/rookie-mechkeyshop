using Application.Comoon;
using Application.Interfaces;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.Dashboard;

namespace Application.Catalog.Queries;

public class GetDashboardQuery : IRequest<Result<DashboardData>>
{
}

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, Result<DashboardData>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<GetDashboardHandler> _logger;

    public GetDashboardHandler(
        IApplicationDbContext dbContext,
        ILogger<GetDashboardHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<DashboardData>> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        try
        {
            var orders = _dbContext.Orders.AsQueryable();
            var products = _dbContext.Products.AsQueryable();
            var users = _dbContext.ApplicationUsers.AsQueryable();

            var dashboardData = new DashboardData
            {
                TotalRevenue = (decimal)await orders.SumAsync(o => o.TotalAmount, ct),
                TotalSellCount = (int)await products.SumAsync(p => p.SellCount, ct),
                TotalOrderCount = await orders.CountAsync(ct),
                TotalOrderCompleted = await orders.CountAsync(o => o.Status == OrderStatus.Completed, ct),
                TotalOrderPending = await orders.CountAsync(o => o.Status == OrderStatus.Pending, ct),
                TotalProductAvailable = await products.CountAsync(p => !p.IsDeleted, ct),
                TotalUserAvailable = await users.CountAsync(u => !u.IsDeleted, ct)
            };

            return Result<DashboardData>.Success("Get dashboard data success", dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(Handle), ex.Message);
            return Result<DashboardData>.Failure("Failed to get dashboard data", new DashboardData());
        }
    }
}
