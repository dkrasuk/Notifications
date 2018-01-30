using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Notifications.BusinessLayer.Services;
using Notifications.BusinessLayer.Services.Interfaces;

namespace Notifications.BusinessLayer
{
    public class Bootstraper
    {
        public static void Register(IUnityContainer container)
        {
            AlfaBank.Logger.Bootstraper.Register(container);
            container.RegisterType<ISmtpService, SmtpService>();
        }
    }
}
