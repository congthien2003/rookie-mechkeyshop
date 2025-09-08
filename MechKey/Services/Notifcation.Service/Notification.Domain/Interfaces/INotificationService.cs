namespace Notification.Core.Interfaces
{
    public interface INotificationService
    {
        Task LogNotificationAsync(Domain.Entities.Notification notification);
        Task<IEnumerable<Domain.Entities.Notification>> GetUserNotificationsAsync(string userId);
    }
}
