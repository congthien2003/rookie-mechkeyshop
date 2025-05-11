using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Order;

namespace Shared.Mapping.Interfaces
{
    public interface IOrderMapping
    {
        OrderModel ToOrderModel(Order order);
        List<OrderModel> ToListOrderModel(List<Order> orders);
        Order ToOrder(OrderModel model);
    }
}