﻿using Application.Comoon;
using Application.Events;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using Domain.Entity;
using Domain.Enum;
using Domain.Exceptions;
using Domain.IRepositories;
using EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Common;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Order;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IApplicationUserRepository<ApplicationUser> applicationUserRepository;
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderUnitOfWork _unitOfWork;
        private readonly IProductSalesTracker _productSalesTracker;
        private readonly IEventBus _eventBus;
        private readonly IOrderMapping _orderMapping;
        private readonly IOrderItemMapping _orderItemMapping;
        private readonly IProductRepository<Product> _productRepository;
        public OrderService(
            IOrderUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            ILogger<OrderService> logger,
            IProductSalesTracker productSalesTracker,
            IEventBus eventBus,
            IApplicationUserRepository<ApplicationUser> applicationUserRepository,
            IOrderMapping orderMapping,
            IOrderItemMapping orderItemMapping,
            IProductRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productSalesTracker = productSalesTracker;
            _eventBus = eventBus;
            this.applicationUserRepository = applicationUserRepository;
            _orderMapping = orderMapping;
            _orderItemMapping = orderItemMapping;
            _productRepository = productRepository;
        }

        public async Task<Result<OrderModel>> CreateOrder(CreateOrderModel model, CancellationToken cancellationToken)
        {

            // Validate product stock in order

            foreach (var item in model.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product.Stock - item.Quantity < 0)
                {
                    throw new InsufficientStockException(product.Name, item.Quantity, product.Stock);
                }
            }

            try
            {

                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = model.UserId,
                    Status = Domain.Enum.OrderStatus.Pending,
                    TotalAmount = model.TotalAmount,
                    OrderDate = model.OrderDate,
                    Phone = model.Phone,
                    Address = model.Address,

                };

                var orderCreated = await _unitOfWork.Orders.CreateAsync(order, cancellationToken);

                foreach (var item in model.OrderItems)
                {
                    var orderItem = _orderItemMapping.ToOrderItemFromCreatedOrderItemModel(item);
                    orderItem.OrderId = order.Id;
                    await _unitOfWork.OrderItems.CreateAsync(orderItem);
                }

                var user = await applicationUserRepository.GetByIdAsync(model.UserId);

                var result = _orderMapping.ToOrderModel(order);

                await _eventBus.PublishAsync(new OrderCreatedEvent
                {
                    Id = Guid.NewGuid(),
                    OrderModel = result,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                return Result<OrderModel>.Success("Create order success", result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw ex;
            }
        }

        public async Task<Result<PagedResult<OrderModel>>> GetAllOrders(PaginationReqModel pagiModel,
                                                                        string startDate,
                                                                        string endDate,
                                                                        string sortCol,
                                                                        bool ascending,
                                                                        CancellationToken cancellationToken)
        {

            var query = _orderRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(startDate))
            {
                query = query.Where(x => x.CreatedAt >= DateTime.Parse(startDate));
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                query = query.Where(x => x.CreatedAt <= DateTime.Parse(endDate).AddDays(1));

            }

            if (!string.IsNullOrEmpty(pagiModel.SearchTerm))
            {
                query = query.Where(o => o.User.Name.Contains(pagiModel.SearchTerm)
                || o.User.Email.Contains(pagiModel.SearchTerm)
                || o.Phone.Contains(pagiModel.SearchTerm));
            }


            if (!string.IsNullOrEmpty(sortCol))
            {
                switch (sortCol)
                {
                    case "date":
                        query = ascending ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt);
                        break;
                    case "amount":
                        query = ascending ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount);
                        break;
                    default:
                        query = query.OrderBy(x => x.Id);
                        break;
                }
            }
            var totalCount = query.Count();
            query = query.Skip((pagiModel.Page - 1) * pagiModel.PageSize).Take(pagiModel.PageSize);
            var list = await query.Select(o => new OrderModel
            {
                Id = o.Id,
                UserId = o.UserId,
                Name = o.User.Name,
                OrderDate = o.CreatedAt,
                Email = o.User.Email,
                Phone = o.Phone,
                Address = o.Address,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemModel
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Price = oi.Product.Price * oi.Quantity,
                    Quantity = oi.Quantity,
                    ImageUrl = oi.Product.ImageUrl,
                    Option = oi.Option.Length > 0 ? JsonConvert.DeserializeObject<OrderItemVariant>(oi.Option) : null,
                })
            }).ToListAsync(cancellationToken);

            return Result<PagedResult<OrderModel>>.Success("Get list order success", new PagedResult<OrderModel>
            {
                Items = list,
                TotalItems = totalCount,
                Page = pagiModel.Page,
                PageSize = pagiModel.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagiModel.PageSize)
            }
            );
        }

        public async Task<OrderModel> GetOrdersById(Guid orderId, CancellationToken cancellationToken = default)
        {

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
                ?? throw new OrderNotFoundException();

            return _orderMapping.ToOrderModel(order);

        }

        public async Task<OrderModel> UpdateOrder(UpdateInfoOrderModel model, CancellationToken cancellationToken = default)
        {

            var order = await _orderRepository.GetByIdAsync(model.Id, cancellationToken)
                ?? throw new OrderNotFoundException();


            if (order.Status != (OrderStatus)model.Status
                && (OrderStatus)model.Status == OrderStatus.Accepted)
            {
                await _productSalesTracker.ProductIncreaseSellCount(order.OrderItems);
            }
            order.ChangeStatus(model.Status);
            order.LastUpdatedAt = DateTime.UtcNow;
            order.Address = model.Address;
            order.Phone = model.Phone;

            var updatedOrder = await _orderRepository.UpdateAsync(order);

            await _unitOfWork.SaveChangesAsync();

            return _orderMapping.ToOrderModel(order);

        }

        public async Task<bool> DeleteOrder(Guid orderId, CancellationToken cancellationToken = default)
        {

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                _logger.LogWarning("Attempted to delete non-existing order in {Method}. OrderId: {OrderId}", nameof(DeleteOrder), orderId);
                throw new OrderNotFoundException();
            }

            await _orderRepository.DeleteAsync(order);
            return true;

        }

        public async Task<Result<IEnumerable<OrderModel>>> GetAllOrdersByIdUser(Guid userId, PaginationReqModel pagiModel, string sortCol, bool ascending, CancellationToken cancellationToken = default)
        {

            var query = _orderRepository.GetAllAsync().Where(x => x.UserId == userId).AsQueryable();

            if (!string.IsNullOrEmpty(pagiModel.SearchTerm))
            {
                query = query.Where(o => o.User.Name.Contains(pagiModel.SearchTerm));
            }

            if (!string.IsNullOrEmpty(sortCol))
            {
                switch (sortCol)
                {
                    case "createdAt":
                        query = ascending ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt);
                        break;
                    case "total":
                        query = ascending ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount);
                        break;
                    default: break;
                }
            }

            query = query.Skip((pagiModel.Page - 1) * pagiModel.PageSize).Take(pagiModel.PageSize);
            var list = await query.ToListAsync(cancellationToken);


            return Result<IEnumerable<OrderModel>>.Success("gest list order success", _orderMapping.ToListOrderModel(list));

        }

    }
}
