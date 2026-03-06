using wixi.Entities.Concrete.Support;

namespace wixi.Business.Abstract
{
    /// <summary>
    /// Notification service interface for managing user notifications
    /// Note: This is a basic implementation. For production, consider using SignalR or push notifications
    /// </summary>
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(int userId, string title, string message, string type = "Info");
        Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<bool> MarkAsReadAsync(long notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> DeleteNotificationAsync(long notificationId);
    }
}

