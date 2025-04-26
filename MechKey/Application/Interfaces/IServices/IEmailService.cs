using Shared.ViewModels.Order;

namespace Application.Interfaces.IServices
{
    public interface IEmailService
    {
        bool SendEmailConfirm(string email, string token);
        bool SendEmailOrder(string email, OrderModel order);
    }
}
