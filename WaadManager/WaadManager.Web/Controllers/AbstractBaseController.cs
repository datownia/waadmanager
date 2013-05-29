using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WaadManager.Common.ActiveDirectory;
using WaadManager.Common.Sql;

namespace WaadManager.Web.Controllers
{
    public abstract class AbstractBaseController : Controller
    {
        public bool IsSpeaker()
        {
            var sqlHelper = new SqlHelper();
            var adHelper = new ADHelper();
            var username = GetCurrentEmailAddress();
            var user = sqlHelper.GetUserByUsername(username);
            return adHelper.IsUserInGroup(user.AdUsername, "Speaker");
        }

        public string GetCurrentEmailAddress()
        {
            var addressClaim =
                ClaimsPrincipal.Current.Claims.Where(
                    claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault();
            if (addressClaim == null)
                return string.Empty;

            return addressClaim.Value;
        }

        public string GetCurrentUpn()
        {
            var addressClaim =
                ClaimsPrincipal.Current.Claims.Where(
                    claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault();
            if (addressClaim == null)
                return string.Empty;
            
            var sqlHelper = new SqlHelper();
            var user = sqlHelper.GetUserByUsername(addressClaim.Value);

	        if (user == null)
		        return null;
            return user.AdUsername;
        }
    }
}

