using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Order;

namespace Application.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderModel>> GetAllOrders(PaginationReqModel pagiModel, string sortCol, bool ascending);
        Task<OrderModel> GetOrdersById(Guid orderId);
        Task<OrderModel> UpdateOrder(UpdateInfoOrderModel model);
        Task<bool> DeleteOrder(Guid orderId);
        Task<Result<OrderModel>> CreateOrder(CreateOrderModel model);
        Task<IEnumerable<OrderModel>> GetAllOrdersByIdUser(Guid id, PaginationReqModel pagiModel, string sortCol, bool ascending);



    }
}
