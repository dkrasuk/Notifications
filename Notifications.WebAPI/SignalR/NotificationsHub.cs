using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Notifications.Common.Utils;

namespace Notifications.WebAPI.SignalR
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationsHub : Hub
    {
        private static readonly IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="message"></param>
        public async static Task SendNotification(string userLogin, object message)
        {
            await hubContext.Clients.Group(userLogin).SendMessage(message);
        }

        /// <summary>
        /// Set notification as readed
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="notificationId"></param>
        public async static Task SetNotificationAsReaded(string userLogin, object notificationId)
        {
            await hubContext.Clients.Group(userLogin).SetNotificationAsReaded(notificationId);
        }

        /// <summary>
        /// Register user
        /// </summary>
        public void Register()
        {
            string receiverLogin = Thread.CurrentPrincipal.Identity.Name.FormatUserName();

            hubContext.Groups.Add(Context.ConnectionId, receiverLogin);
        }
    }
}