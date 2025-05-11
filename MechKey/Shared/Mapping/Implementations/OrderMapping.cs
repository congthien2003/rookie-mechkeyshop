using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Order;

namespace Shared.Mapping.Implementations
{
    public class OrderMapping : IOrderMapping
    {
        private readonly IOrderItemMapping _orderItemMapping;

        public OrderMapping(IOrderItemMapping orderItemMapping)
        {
            _orderItemMapping = orderItemMapping;
        }

        public OrderModel ToOrderModel(Order order)
        {
            return new OrderModel
            {
                Id = order.Id,
                UserId = order.UserId,
                Name = order.User?.Name ?? string.Empty,
                Email = order.User?.Email ?? string.Empty,
                Status = order.Status,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Address = order.Address,
                Phone = order.Phone,
                CreatedAt = order.CreatedAt,
                LastUpdatedAt = order.LastUpdatedAt,
                OrderItems = order.OrderItems != null
                    ? _orderItemMapping.ToListOrderItemModel(order.OrderItems.ToList())
                    : Enumerable.Empty<OrderItemModel>(),
                UpdateById = order.UpdateById,
            };
        }

        public List<OrderModel> ToListOrderModel(List<Order> orders)
        {
            var result = new List<OrderModel>();
            foreach (var order in orders)
            {
                result.Add(ToOrderModel(order));
            }
            return result;
        }

        public Order ToOrder(OrderModel model)
        {
            return new Order
            {
                Id = model.Id,
                UserId = model.UserId,
                Status = model.Status,
                OrderDate = model.OrderDate,
                TotalAmount = model.TotalAmount,
                Address = model.Address,
                Phone = model.Phone,
                CreatedAt = model.CreatedAt,
                LastUpdatedAt = model.LastUpdatedAt,
                OrderItems = model.OrderItems != null
                    ? _orderItemMapping.ToListOrderItem(model.OrderItems.ToList())
                    : new List<OrderItem>(),
                UpdateById = model.UpdateById,
            };
        }
    }
}