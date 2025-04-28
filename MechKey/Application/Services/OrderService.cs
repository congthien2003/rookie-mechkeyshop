using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.MassTransit;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entity;
using Domain.Enum;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.Order;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderUnitOfWork _unitOfWork;
        private readonly IProductSalesTracker _productSalesTracker;
        private readonly IEventBus _eventBus;
        public OrderService(
            IOrderUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<OrderService> logger,
            IProductSalesTracker productSalesTracker,
            IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productSalesTracker = productSalesTracker;
            _eventBus = eventBus;
        }

        public async Task<Result<OrderModel>> CreateOrder(CreateOrderModel model)
        {
            try
            {
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

                await _unitOfWork.Orders.CreateAsync(order);

                foreach (var item in model.OrderItems)
                {
                    var orderItem = _mapper.Map<OrderItem>(item);
                    orderItem.OrderId = order.Id;
                    await _unitOfWork.OrderItems.CreateAsync(orderItem);
                }
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<OrderModel>(order);

                await _eventBus.PublishAsync(new OrderCreatedEvent
                {
                    Id = Guid.NewGuid(),
                    OrderModel = result,
                    CreatedAt = DateTime.UtcNow
                });

                return Result<OrderModel>.Success("Create order success", result);
            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();
                _logger.LogError(ex, "Error occurred in {Method}. UserId: {UserId}, Message: {Message}", nameof(CreateOrder), model.UserId, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<Result<PagedResult<OrderModel>>> GetAllOrders(PaginationReqModel pagiModel, string startDate, string endDate, string sortCol, bool ascending)
        {
            try
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
                        case "totalAmount":
                            query = ascending ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount);
                            break;
                        default: break;
                    }
                }
                var totalCount = query.Count();
                query = query.Skip((pagiModel.Page - 1) * pagiModel.PageSize).Take(pagiModel.PageSize);
                var list = await query.ProjectTo<OrderModel>(_mapper.ConfigurationProvider).ToListAsync();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(GetAllOrders), ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<OrderModel> GetOrdersById(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId)
                    ?? throw new OrderNotFoundException();

                return _mapper.Map<OrderModel>(order);
            }
            catch (OrderNotFoundException)
            {
                _logger.LogWarning("Order not found in {Method}. OrderId: {OrderId}", nameof(GetOrdersById), orderId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. OrderId: {OrderId}, Message: {Message}", nameof(GetOrdersById), orderId, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<OrderModel> UpdateOrder(UpdateInfoOrderModel model)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(model.Id)
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

                return _mapper.Map<OrderModel>(updatedOrder);
            }
            catch (OrderNotFoundException)
            {
                _logger.LogWarning("Order not found in {Method}. OrderId: {OrderId}", nameof(UpdateOrder), model.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. OrderId: {OrderId}, Message: {Message}", nameof(UpdateOrder), model.Id, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<bool> DeleteOrder(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing order in {Method}. OrderId: {OrderId}", nameof(DeleteOrder), orderId);
                    throw new OrderNotFoundException();
                }

                await _orderRepository.DeleteAsync(order);
                return true;
            }
            catch (OrderNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. OrderId: {OrderId}, Message: {Message}", nameof(DeleteOrder), orderId, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<IEnumerable<OrderModel>> GetAllOrdersByIdUser(Guid userId, PaginationReqModel pagiModel, string sortCol, bool ascending)
        {
            try
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
                var list = await Task.FromResult(query.ToList());

                return list.Select(order => _mapper.Map<OrderModel>(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in {Method}. UserId: {UserId}, Message: {Message}", nameof(GetAllOrdersByIdUser), userId, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

    }
}
