using Notification.Core.Interfaces;
using Notification.Domain.IRepository;

namespace Notification.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task LogNotificationAsync(Domain.Entities.Notification notification)
        {
            await _repository.AddAsync(notification);
        }

        public async Task<IEnumerable<Domain.Entities.Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }
    }
}
