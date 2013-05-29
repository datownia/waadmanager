using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaadManager.Web.Controllers
{
    public class AccountController : Controller
    {
        public void LogOn()
        {
            RedirectResult result;
            if (!Request.IsAuthenticated)
            {
                SignInRequestMessage sirm = FederatedAuthentication.WSFederationAuthenticationModule.CreateSignInRequest("", HttpContext.Request.RawUrl, false);
                result = Redirect(sirm.RequestUrl);
            }
            else
            {
                result = Redirect("~/");
            }
            result.ExecuteResult(this.ControllerContext);
        }
    }
}
