namespace Notification.Domain.IRepository
{
    public interface INotificationRepository
    {
        Task AddAsync(Domain.Entities.Notification notification);
        Task<IEnumerable<Domain.Entities.Notification>> GetByUserIdAsync(string userId);
        Task<Domain.Entities.Notification?> GetByIdAsync(string id);
    }
}
