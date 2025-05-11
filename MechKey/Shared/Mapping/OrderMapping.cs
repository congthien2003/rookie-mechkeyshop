using Domain.Entity;
using Shared.ViewModels.Order;

namespace Shared.Mapping
{
    public static class OrderMapping
    {
        public static OrderModel ToOrderModel(Order order)
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
                    ? OrderItemMapping.ToListOrderItemModel(order.OrderItems.ToList())
                    : Enumerable.Empty<OrderItemModel>(),

                UpdateById = order.UpdateById,
            };
        }

        public static List<OrderModel> ToListOrderModel(List<Order> orders)
        {
            var result = new List<OrderModel>();
            foreach (var order in orders)
            {
                result.Add(ToOrderModel(order));
            }
            return result;
        }

        public static Order ToOrder(OrderModel model)
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
                    ? OrderItemMapping.ToListOrderItem(model.OrderItems.ToList())
                    : new List<OrderItem>(),
                UpdateById = model.UpdateById,
            };
        }
    }

}
