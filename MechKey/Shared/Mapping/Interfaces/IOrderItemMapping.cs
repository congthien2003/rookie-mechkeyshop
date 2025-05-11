using Domain.Entity;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Order;

namespace Shared.Mapping.Interfaces
{
    public interface IOrderItemMapping
    {
        OrderItemModel ToOrderItemModel(OrderItem item);
        List<OrderItemModel> ToListOrderItemModel(List<OrderItem> items);
        OrderItem ToOrderItem(OrderItemModel model);
        List<OrderItem> ToListOrderItem(List<OrderItemModel> models);
        OrderItem ToOrderItemFromCreatedOrderItemModel(CreateOrderItemViewModel model);
    }
}