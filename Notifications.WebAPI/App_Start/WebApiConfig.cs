using System.Net.Http.Headers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Notifications.Common;
using Swashbuckle.Application;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Notifications.WebAPI
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            formatter.SerializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                Formatting = Formatting.Indented
            };
            formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors); 

            RegisterDependencyResolver(config);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "swagger_root",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger"));

            config.Routes.MapHttpRoute(
                "API Default",
                "api/{controller}/{action}");
        }

        /// <summary>
        /// Registers the dependency resolver.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private static void RegisterDependencyResolver(HttpConfiguration config)
        {
            var container = new UnityContainer();

           Bootstraper.Register(container);

            config.DependencyResolver = new UnityResolver(container);

            var locator = new UnityServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}