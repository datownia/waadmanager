using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Claims;

namespace WaadManager.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

//            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            string wsFamRedirectLocation = HttpContext.Current.Response.RedirectLocation;
            if (wsFamRedirectLocation != null &&
                wsFamRedirectLocation.Contains("ReturnUrl") &&
                ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                HttpContext.Current.Response.RedirectLocation =
                            HttpUtility.ParseQueryString(
                              wsFamRedirectLocation.Split('?')[1])["ReturnUrl"];
            }
        }
    }
}