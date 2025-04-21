using Application.Comoon;
using Application.Interfaces.IServices;
using Application.Interfaces.IUnitOfWork;
using AutoMapper;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderUnitOfWork _unitOfWork;
        public OrderService(
            IOrderUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
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
                return Result<OrderModel>.Success("Create order success", _mapper.Map<OrderModel>(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<IEnumerable<OrderModel>> GetAllOrders(PaginationReqModel pagiModel, string sortCol, bool ascending)
        {
            try
            {
                var query = _orderRepository.GetAllAsync();

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
                _logger.LogError(ex, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<OrderModel> GetOrdersById(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException();

            return _mapper.Map<OrderModel>(order);
        }

        public async Task<OrderModel> UpdateOrder(UpdateInfoOrderModel model)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(model.Id);
                if (order == null)
                    throw new OrderNotFoundException();

                // Update basic properties
                order.ChangeStatus(model.Status);
                order.LastUpdatedAt = DateTime.UtcNow;

                var updated = await _orderRepository.UpdateAsync(order);
                return _mapper.Map<OrderModel>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new OrderHandleFailedException();
            }
        }

        public async Task<bool> DeleteOrder(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException();

            await _orderRepository.DeleteAsync(order);
            return true;
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
                _logger.LogError(ex, ex.Message);
                throw new OrderHandleFailedException();
            }
        }
    }
}
