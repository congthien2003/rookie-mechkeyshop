﻿using Shared.ViewModels.Order;

namespace Notification.Application.Interfaces
{
    public interface IEmailService
    {
        bool SendEmailConfirm(string email, string token);
        bool SendEmailOrder(string email, OrderModel order);
    }
}
