using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ActiveDirectory;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using WaadManager.GraphStore;
using AADJWTToken = Microsoft.WindowsAzure.ActiveDirectory.GraphHelper.AADJWTToken;
using DirectoryDataServiceAuthorizationHelper = Microsoft.WindowsAzure.ActiveDirectory.GraphHelper.DirectoryDataServiceAuthorizationHelper;

namespace WaadManager.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public ActionResult List()
        {

            /*HELPER CONSTRUCTOR*/
            //get the tenantName
            string tenantName = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            // retrieve the clientId and password values from the Web.config file
            string clientId = ConfigurationManager.AppSettings["AppPrincipalId"];
            string password = ConfigurationManager.AppSettings["Password"];
            

            /*NEEDS TO BE CACHED*/
            // get a token using the helper
            AADJWTToken token = DirectoryDataServiceAuthorizationHelper.GetAuthorizationToken(tenantName, clientId, password);
            // initialize a graphService instance using the token acquired from previous step
            DirectoryDataService graphService = new DirectoryDataService(tenantName, token);

            //  get Users
            var users = graphService.users;
            QueryOperationResponse<User> response;
            response = users.Execute() as QueryOperationResponse<User>;
            List<User> userList = response.ToList();
            ViewBag.userList = userList;


            //  For subsequent Graph Calls, the existing token should be used.
            //  The following checks to see if the existing token is expired or about to expire in 2 mins
            //  if true, then get a new token and refresh the graphService
            //
            int tokenMins = 2;
            if (token.IsExpired || token.WillExpireIn(tokenMins))
            {
                AADJWTToken newToken = DirectoryDataServiceAuthorizationHelper.GetAuthorizationToken(tenantName, clientId, password);
                token = newToken;
                graphService = new DirectoryDataService(tenantName, token);
            }

            //  get tenant information
            var tenant = graphService.tenantDetails;
            QueryOperationResponse<TenantDetail> responseTenantQuery;
            responseTenantQuery = tenant.Execute() as QueryOperationResponse<TenantDetail>;
            List<TenantDetail> tenantInfo = responseTenantQuery.ToList();
            ViewBag.OtherMessage = "User List from tenant: " + tenantInfo[0].displayName;


            return View(userList);
        }

    }
}
