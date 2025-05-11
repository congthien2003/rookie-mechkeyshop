using Domain.Entity;
using Newtonsoft.Json;
using Shared.ViewModels.Order;

namespace Shared.Mapping
{
    public static class OrderItemMapping
    {
        public static OrderItemModel ToOrderItemModel(OrderItem item)
        {
            return new OrderItemModel
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                ImageUrl = item.Product?.ImageUrl ?? string.Empty,
                Price = item.Product?.Price ?? 0,
                Quantity = item.Quantity,
                Option = !string.IsNullOrWhiteSpace(item.Option)
                    ? JsonConvert.DeserializeObject<OrderItemVariant>(item.Option)
                    : null,
            };
        }

        public static List<OrderItemModel> ToListOrderItemModel(List<OrderItem> items)
        {
            var result = new List<OrderItemModel>();
            foreach (var item in items)
            {
                result.Add(ToOrderItemModel(item));
            }
            return result;
        }

        public static OrderItem ToOrderItem(OrderItemModel model)
        {
            return new OrderItem
            {
                Id = model.Id,
                OrderId = model.OrderId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                TotalAmount = model.Quantity * model.Price,
                Option = model.Option != null
                    ? JsonConvert.SerializeObject(model.Option)
                    : string.Empty
            };
        }

        public static List<OrderItem> ToListOrderItem(List<OrderItemModel> models)
        {
            var result = new List<OrderItem>();
            foreach (var model in models)
            {
                result.Add(ToOrderItem(model));
            }
            return result;
        }

        public static OrderItem ToOrderItemFromCreatedOrderItemModel(CreateOrderItemViewModel model)
        {
            return new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                TotalAmount = model.TotalAmount,
                Option = model.Option
            };
        }
    }

}
