using Notifications.DataAccess.DTO;
using Notifications.DataAccess.Interfaces;
using Notifications.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Notifications.DataAccess.Data
{
    class NotificationData : INotificationData
    {
        /// <summary>
        /// Create the notification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <returns>int</returns>
        public async Task<NotificationDTO> CreateNotification(NotificationDTO notification)
        {
            using (var context = new NotificationsContext())
            {
                var newNotification = new Notification
                {
                    Id = notification.Id,
                    Channel = notification.Channel,
                    Receiver = notification.Receiver,
                    Type = notification.Type,
                    Title = notification.Title,
                    Body = notification.Body,
                    IsReaded = notification.IsReaded,
                    CreatedDate = DateTime.Now,
                    Protocol = notification.Protocol
                };

                context.Notifications.Add(newNotification);

                await context.SaveChangesAsync();

                notification.Id = newNotification.Id;
                notification.CreatedDate = newNotification.CreatedDate;

                return notification;
            }
        }

        /// <summary>
        /// Get notifications by user login.
        /// </summary>
        /// <param name="userLogin">The receiver login.</param>
        /// <param name="onlyUnReaded">Take only unreaded notifications.</param>
        /// <returns>IEnumerable\<NotificationDTO\></returns>
        public async Task<IEnumerable<NotificationDTO>> GetNotificationsByReceiver(string userLogin, bool onlyUnReaded = false)
        {
            using (var context = new NotificationsContext())
            {
                return await context.Notifications.Where(t => t.Receiver == userLogin && ((t.IsReaded == false) || !onlyUnReaded)).OrderByDescending(t => t.Id)
                    .Select(t => new NotificationDTO
                    {
                        Id = t.Id,
                        Channel = t.Channel,
                        Receiver = t.Receiver,
                        Type = t.Type,
                        Title = t.Title,
                        Body = t.Body,
                        IsReaded = t.IsReaded,
                        CreatedDate = t.CreatedDate,
                        Protocol = t.Protocol
                    }).ToListAsync();
            }
        }

        /// <summary>
        /// Get the count of unreaded notifications by user login.
        /// </summary>
        /// <param name="userLogin">The user login.</param>
        /// <returns>int</returns>
        public async Task<int> GetCountOfUnreadedNotifications(string userLogin)
        {
            using (var context = new NotificationsContext())
            {
                return await context.Notifications.Where(t => (t.Receiver == userLogin) && (t.IsReaded == false)).CountAsync();
            }
        }

        /// <summary>
        /// Set notification as Readed.
        /// </summary>
        /// <param name="notificationId">The notification Id.</param>
        /// <returns>void</returns>
        public async Task SetNotificationAsReaded(int notificationId)
        {
            using (var context = new NotificationsContext())
            {
                var notificationToEdit = context.Notifications.FirstOrDefault(t => t.Id == notificationId);
                if (notificationToEdit != null)
                {
                    notificationToEdit.IsReaded = true;
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}