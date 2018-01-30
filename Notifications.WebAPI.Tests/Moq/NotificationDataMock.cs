using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notifications.DataAccess.DTO;
using Notifications.DataAccess.Interfaces;

namespace Notifications.WebAPI.Tests.Moq
{
    public class NotificationDataMock : INotificationData
    {
        readonly List<NotificationDTO> _testData = new List<NotificationDTO>();
        
        public NotificationDataMock()
        {
            CreateTestData(_testData);
        }

        public async Task<NotificationDTO> CreateNotification(NotificationDTO notification)
        {
            notification.Id = GetNewNotifyId();
            
            _testData.Add(notification);
            return notification;
        }

        public async Task<IEnumerable<NotificationDTO>> GetNotificationsByReceiver(
            string userLogin, bool onlyUnReaded = false)
        {
            var result = _testData.Where(i => i.Receiver == userLogin);
            if (onlyUnReaded)
            {
                result = result.Where(i => i.IsReaded == false);
            }
            return result;
        }

        public async Task<int> GetCountOfUnreadedNotifications(string userLogin)
        {
            return _testData.Count(i => i.Receiver == userLogin && i.IsReaded == false);
        }

        public async Task SetNotificationAsReaded(int notificationId)
        {
            var notify = _testData.FirstOrDefault(i => i.Id == notificationId);
            if (notify != null)
            {
                notify.IsReaded = true;
            }
        }

        private void CreateTestData(List<NotificationDTO> data)
        {
            var userName = "testuser1";
            for (int i = 1; i < 50; i++)
            {
                data.Add(CreateNewTestNotification(i, userName));
            }
            userName = "testuser2";
            for (int i = 50; i < 100; i++)
            {
                data.Add(CreateNewTestNotification(i, userName));
            }
        }

        private NotificationDTO CreateNewTestNotification(int id, string userName)
        {
            return new NotificationDTO
            {
                Body = "Test body",
                Channel = "channel",
                CreatedDate = DateTime.Now,
                Id = id,
                IsReaded = id % 2 == 0,
                Receiver = userName,
                Title = "Title",
                Type = "info"
            };
        }

        private long GetNewNotifyId()
        {
            return _testData.Max(i => i.Id) ?? 1;
        }
    }
}