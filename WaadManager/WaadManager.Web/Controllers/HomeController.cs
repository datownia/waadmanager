using System.Configuration;
using System.Security.Claims;
using System.Web.Mvc;

namespace WaadManager.Web.Controllers
{
    public class HomeController : AbstractBaseController
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
	            var upn = GetCurrentUpn();
				if (upn == null)
				{
					ViewBag.Message = "User is not registered in the system.";
					return View();
				}
                return RedirectToAction("Landing");
            }

            return View();
        }

        [Authorize]
        public ActionResult Landing()
        {
            return RedirectToAction("ViewSchedule", "Programme");
        }
    }
}
