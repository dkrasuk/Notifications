using System.Configuration;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Notifications.Common;
using Owin;
using Sdl.SignalR.OracleMessageBus;

[assembly: OwinStartup(typeof(Notifications.WebAPI.Startup))]

namespace Notifications.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var corsPolicy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
                SupportsCredentials = true,
                PreflightMaxAge = 1728000
            };

            // Try and load allowed origins from web.config
            // If none are specified we'll allow all origins

            var origins = ConfigurationManager.AppSettings["CorsOriginsSettingKey"];

            if (origins != null)
            {
                foreach (var origin in origins.Split(';'))
                {
                    corsPolicy.Origins.Add(origin);
                }
            }
            else
            {
                corsPolicy.AllowAnyOrigin = true;
            }

            var corsOptions = new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(corsPolicy)
                }
            };


            app.UseCors(corsOptions);
             
            var hubConfig = new HubConfiguration();
            hubConfig.EnableJSONP = true;

            var connectionString = 
                ConfigurationManager.ConnectionStrings[Constants.NotificationConnectionStringName]?.ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                //add database sinchronize
                GlobalHost.DependencyResolver.UseOracle(connectionString);
            }
            app.MapSignalR(hubConfig);

           // app.UseWebApi();
            
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
