using Domain.Entity;

namespace Application.Interfaces.IServices
{
    public interface IProductSalesTracker
    {
        Task ProductIncreaseSellCount(IEnumerable<OrderItem> orders);
    }
}
