using Application.Comoon;
using Application.Interfaces.IServices;
using Domain.Entity;
using Domain.Enum;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.Dashboard;

namespace Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository<Product> _productRepository;
        private readonly IApplicationUserRepository<ApplicationUser> _userRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IOrderRepository orderRepository,
            IProductRepository<Product> productRepository,
            IApplicationUserRepository<ApplicationUser> userRepository,
            ILogger<DashboardService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<DashboardData>> GetDashboardDataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = _orderRepository.GetAllAsync();
                var products = _productRepository.GetAllAsync();
                var users = _userRepository.GetAllAsync();

                var dashboardData = new DashboardData
                {
                    TotalRevenue = (decimal)await orders.SumAsync(o => o.TotalAmount, cancellationToken),
                    TotalSellCount = (int)await products.SumAsync(p => p.SellCount, cancellationToken),
                    TotalOrderCount = await orders.CountAsync(cancellationToken),
                    TotalOrderCompleted = await orders.CountAsync(o => o.Status == OrderStatus.Completed, cancellationToken),
                    TotalOrderPending = await orders.CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken),
                    TotalProductAvailable = await products.CountAsync(p => !p.IsDeleted, cancellationToken),
                    TotalUserAvailable = await users.CountAsync(u => !u.IsDeleted, cancellationToken)
                };

                return Result<DashboardData>.Success("Get dashboard data success", dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(GetDashboardDataAsync), ex.Message);
                return Result<DashboardData>.Failure("Failed to get dashboard data", new DashboardData());
            }
        }
    }
}