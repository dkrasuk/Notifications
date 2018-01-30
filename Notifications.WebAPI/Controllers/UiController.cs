using System.Web.Mvc;

namespace Notifications.WebAPI.Controllers
{
    public class UiController : Controller
    {
        public ActionResult Index()
        {
            var file = System.Web.Hosting.HostingEnvironment.MapPath("~/views/ui/notificationJs.js");
            return File(System.IO.File.ReadAllBytes(file), "text/javascript");
        }
    }
}
