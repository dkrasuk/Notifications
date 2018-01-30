using Microsoft.Practices.Unity;
using Notifications.DataAccess.Data;
using Notifications.DataAccess.Interfaces;

namespace Notifications.DataAccess
{
    public class Bootstraper
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<INotificationData, NotificationData>();
        }
    }
}
