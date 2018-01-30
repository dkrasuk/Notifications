using System.Collections.Generic;
using Notifications.DataAccess.DTO;
using System.Threading.Tasks;

namespace Notifications.DataAccess.Interfaces
{
    public interface INotificationData
    {
        Task<NotificationDTO> CreateNotification(NotificationDTO notification);

        Task<IEnumerable<NotificationDTO>> GetNotificationsByReceiver(string userLogin, bool onlyUnReaded = false);

        Task<int> GetCountOfUnreadedNotifications(string userLogin);

        Task SetNotificationAsReaded(int notificationId);
    }
}