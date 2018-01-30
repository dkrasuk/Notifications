using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notifications.WebAPI
{
    /// <summary>
    /// Class Bootstraper.
    /// </summary>
    public class Bootstraper
    {
        /// <summary>
        /// Registers the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        public static void Register(IUnityContainer container)
        {
            AlfaBank.Logger.Bootstraper.Register(container);

            DataAccess.Bootstraper.Register(container);
            BusinessLayer.Bootstraper.Register(container);
        }
    }
}