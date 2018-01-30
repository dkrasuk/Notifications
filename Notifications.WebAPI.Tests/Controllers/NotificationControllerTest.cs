using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using AlfaBank.Logger;
using Moq;
using Notifications.BusinessLayer.Services.Interfaces;
using Notifications.DataAccess.DTO;
using Notifications.DataAccess.Interfaces;
using Notifications.WebAPI.Controllers;
using Notifications.WebAPI.Tests.Moq;
using NUnit.Framework;

namespace Notifications.WebAPI.Tests.Controllers
{
    [TestFixture]
    public class NotificationControllerTest
    {
        private readonly string testUserName = "alFa\\TestUser1";
        private readonly INotificationData _notificationDataMock = new NotificationDataMock();
        private readonly ILogger _logger = new Mock<ILogger>().Object;
        private readonly ISmtpService _smtpService = new Mock<ISmtpService>().Object;
        private NotificationController _controller;

        [OneTimeSetUp]
        public void Init()
        {
            _controller = GetNotificationController();
            Thread.CurrentPrincipal = new GenericPrincipal
            (
                new GenericIdentity(testUserName, "Passport"),
                new[] {"managers", "executives"}
            );
        }
        
        [Test]
        public async Task CreateNotificationTest()
        {
            // Arrange

            // Act
            var id = 0;
            var notification = new NotificationDTO
            {
                Id = id,
                Receiver = testUserName
            };
            var response = await _controller.CreateNotification(notification);

            // Assert
            NotificationDTO notificationResponse;
            Assert.IsTrue(response.TryGetContentValue<NotificationDTO>(out notificationResponse));
            Assert.AreNotEqual(id, notificationResponse.Id);
            Assert.AreEqual(FormatUserName(testUserName), notificationResponse.Receiver);
        }
        
        [Test]
        public async Task GetNotificationsOnlyUnreadedByDefoultTest()
        {
            // Arrange
            
            // Act
            var response = await _controller.GetNotifications();
            
            // Assert
            IEnumerable<NotificationDTO> notificationsResponse;
            Assert.IsTrue(response.TryGetContentValue(out notificationsResponse));
                
            Assert.AreNotEqual(notificationsResponse, null);            
            Assert.AreNotEqual(notificationsResponse.Count(), 0);
            Assert.AreEqual(notificationsResponse.Count(i => i.IsReaded == true), 0);   
            
            Assert.AreEqual(notificationsResponse.Count(i => i.Receiver != FormatUserName(testUserName)), 0);   
        }
        
        [Test]
        public async Task GetNotificationsOnlyUnreadedTrueTest()
        {
            // Arrange
            
            // Act
            var response = await _controller.GetNotifications(true);
            
            // Assert
            IEnumerable<NotificationDTO> notificationsResponse;
            Assert.IsTrue(response.TryGetContentValue(out notificationsResponse));
                
            Assert.AreNotEqual(notificationsResponse, null);            
            Assert.AreNotEqual(notificationsResponse.Count(), 0);
            Assert.AreEqual(notificationsResponse.Count(i => i.IsReaded == true), 0);   
            
            Assert.AreEqual(notificationsResponse.Count(i => i.Receiver != FormatUserName(testUserName)), 0);   
        }

        [Test]
        public async Task GetNotificationsOnlyUnreadedFalseTest()
        {
            // Arrange    
            
            // Act
            var response = await _controller.GetNotifications(false);
            
            // Assert
            IEnumerable<NotificationDTO> notificationsResponse;
            Assert.IsTrue(response.TryGetContentValue(out notificationsResponse)); 
            
            Assert.AreNotEqual(notificationsResponse, null);            
            Assert.AreNotEqual(notificationsResponse.Count(), 0);
            Assert.AreNotEqual(notificationsResponse.Count(i => i.IsReaded == true), 0);   
            Assert.AreNotEqual(notificationsResponse.Count(i => i.IsReaded == false), 0);   
            
            Assert.AreEqual(notificationsResponse.Count(i => i.Receiver != FormatUserName(testUserName)), 0);            
        }

        [Test]
        public async Task GetCountOfUnreadedNotificationsTest()
        {  
            // Arrange    
            
            // Act
            var response = await _controller.GetCountOfUnreadedNotifications();
            
            // Assert
            int count;
            Assert.IsTrue(response.TryGetContentValue(out count)); 
                    
            Assert.AreNotEqual(count, 0);       
        }

        public async Task SetNotificationAsReadedTest()
        {
            // Arrange    
            
            // Act
            var testId = 1;
            var response = await _controller.SetNotificationAsReaded(testId);
            
            // Assert
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }


        private NotificationController GetNotificationController()
        {
            var controller = new NotificationController(_logger, _notificationDataMock, _smtpService );
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            
            return controller;
        }

        private string FormatUserName(string userName)
        {
            return userName.ToLower().Replace("alfa\\", "");
        }
    }
}