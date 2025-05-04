using Shared.ViewModels.Order;

namespace Application.Interfaces.IApiClient.Smtp
{
    public interface IEmailService
    {
        bool SendEmailConfirm(string email, string token);
        bool SendEmailOrder(string email, OrderModel order);
    }
}
