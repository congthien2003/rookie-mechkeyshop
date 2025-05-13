using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using Application.Services;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using EventBus;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Order;

namespace Application.Test
{
    public class OrderServiceTest
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly Mock<IOrderUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductSalesTracker> _mockProductSalesTracker;
        private readonly Mock<IApplicationUserRepository<ApplicationUser>> _mockApplicationUserRepository;
        private readonly Mock<IProductRepository<Product>> _mockProductRepository;
        private readonly Mock<IEventBus> _mockEventBus;
        private readonly Mock<IOrderMapping> _mockMapping;
        private readonly Mock<IOrderItemMapping> _mockItemMapping;
        private readonly OrderService _orderService;
        private readonly CancellationToken _cancellationToken;

        public OrderServiceTest()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _mockUnitOfWork = new Mock<IOrderUnitOfWork>();
            _mockProductSalesTracker = new Mock<IProductSalesTracker>();
            _mockEventBus = new Mock<IEventBus>();
            _mockMapping = new Mock<IOrderMapping>();
            _mockItemMapping = new Mock<IOrderItemMapping>();
            _mockProductRepository = new Mock<IProductRepository<Product>>();
            _cancellationToken = new CancellationToken();
            _mockApplicationUserRepository = new Mock<IApplicationUserRepository<ApplicationUser>>();
            _orderService = new OrderService(
                _mockUnitOfWork.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockProductSalesTracker.Object,
                _mockEventBus.Object,
                _mockApplicationUserRepository.Object,
                _mockMapping.Object,
                _mockItemMapping.Object,
                _mockProductRepository.Object
            );
        }

        [Fact]
        public async Task CreateOrder_ShouldHandleException_AndRollback()
        {
            // Arrange
            var createOrderModel = new CreateOrderModel
            {
                UserId = Guid.NewGuid(),
                TotalAmount = 300,
                OrderDate = DateTime.Now,
                Phone = "0909090909",
                Address = "123 Main St, Anytown, USA",
                OrderItems = new List<CreateOrderItemViewModel>
                {
                    new CreateOrderItemViewModel
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 2
                    }
                }
            };

            var listItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Quantity = 2,
                    ProductId = Guid.NewGuid(),
                    OrderId = Guid.NewGuid(),
                    TotalAmount = 300
                }
            };

            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TotalAmount = 300,
                OrderDate = DateTime.Now,
                Phone = "0909090909",
                Address = "123 Main St, Anytown, USA",
                OrderItems = listItems
            };

            var orderModel = new OrderModel()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TotalAmount = 300,
                OrderDate = DateTime.Now,
                Phone = "0909090909",
                Address = "123 Main St, Anytown, USA",
                OrderItems = new List<OrderItemModel>
                {
                    new OrderItemModel
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 2
                    }
                }
            };

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                Quantity = 2,
                ProductId = Guid.NewGuid(),
                TotalAmount = 300,
                OrderId = Guid.NewGuid()
            };

            var orderItemModel = new OrderItemModel
            {
                ProductId = createOrderModel.OrderItems.First().ProductId,
                Quantity = createOrderModel.OrderItems.First().Quantity
            };

            var createOrderItemViewModel = new CreateOrderItemViewModel
            {
                Quantity = 2,
                ProductId = Guid.NewGuid(),
                TotalAmount = 300
            };

            _mockItemMapping.Setup(r => r.ToOrderItemFromCreatedOrderItemModel(createOrderItemViewModel)).Returns(orderItem);
            _mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync(_cancellationToken)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(r => r.Orders.CreateAsync(order, _cancellationToken)).ReturnsAsync(order);
            _mockUnitOfWork.Setup(r => r.OrderItems.CreateAsync(orderItem, _cancellationToken)).ThrowsAsync(new OrderHandleFailedException());
            _mockUnitOfWork.Setup(uow => uow.RollbackAsync(_cancellationToken)).Returns(Task.CompletedTask);

            // Act & Assert
            var result = Assert.ThrowsAsync<OrderHandleFailedException>(() => _orderService.CreateOrder(createOrderModel, _cancellationToken));

            Assert.Equal("Handle Order failed", result.Result.Message);
            _mockUnitOfWork.Verify(uow => uow.RollbackAsync(_cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockUnitOfWork.Setup(r => r.Orders.GetByIdAsync(orderId, _cancellationToken)).ReturnsAsync((Order)null);

            // Act
            var exception = Assert.ThrowsAsync<OrderNotFoundException>(() => _orderService.DeleteOrder(orderId, _cancellationToken));

            // Assert
            Assert.Equal("Order not found", exception.Result.Message);
        }
    }
}
