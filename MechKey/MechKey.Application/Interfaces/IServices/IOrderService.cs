using Application.Comoon;
using Shared.Common;
using Shared.ViewModels.Order;

namespace Application.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<Result<PagedResult<OrderModel>>> GetAllOrders(PaginationReqModel pagiModel, string startDate = "", string endDate = "", string sortCol = "", bool ascending = true, CancellationToken cancellationToken = default);
        Task<OrderModel> GetOrdersById(Guid orderId, CancellationToken cancellationToken = default);
        Task<OrderModel> UpdateOrder(UpdateInfoOrderModel model, CancellationToken cancellationToken = default);
        Task<bool> DeleteOrder(Guid orderId, CancellationToken cancellationToken = default);
        Task<Result<OrderModel>> CreateOrder(CreateOrderModel model, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<OrderModel>>> GetAllOrdersByIdUser(Guid id, PaginationReqModel pagiModel, string sortCol, bool ascending, CancellationToken cancellationToken = default);
    }
}
