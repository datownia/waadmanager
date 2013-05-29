using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure.ActiveDirectory;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using WaadManager.Common.Models;
using AdUser = Microsoft.WindowsAzure.ActiveDirectory.User;
using AdGroup = Microsoft.WindowsAzure.ActiveDirectory.Group;

namespace WaadManager.Common.ActiveDirectory
{
    public class ADHelper : IADHelper
    {
        private DirectoryDataService directoryDataService = CreateDirectoryDataService();

        public static DirectoryDataService CreateDirectoryDataService()
        {
            AADJWTToken token = null;

            token = DirectoryDataServiceAuthorizationHelper.GetAuthorizationToken(ConfigurationManager.AppSettings["TenantDomainName"],
                ConfigurationManager.AppSettings["AppPrincipalId"], ConfigurationManager.AppSettings["Password"]);

            return new DirectoryDataService(ConfigurationManager.AppSettings["TenantDomainName"], token);
        }

        public void CreateUser(ref DtUser dtUser)
        {
            dtUser.AdUsername = GetFreshUsername(dtUser.Username);
            var user = new AdUser();
            user.displayName = dtUser.DisplayName;
            user.userPrincipalName = dtUser.AdUsername;
            user.mailNickname = dtUser.MailNickname;
            user.accountEnabled = true;

            var pwProfile = new PasswordProfile();
            pwProfile.password = WaadConfig.DefaultPassword;
            pwProfile.forceChangePasswordNextLogin = true;
            user.passwordProfile = pwProfile;
            
            if (dtUser.HasAlternateEmail)
                user.otherMails.Add(dtUser.Username);
            
            directoryDataService.AddTousers(user);

            directoryDataService.SaveChanges();
        }

        public void CreateUserList(ref IEnumerable<DtUser> dtUserList)
        {
            foreach (var dtUser in dtUserList)
            {
                dtUser.AdUsername = GetFreshUsername(dtUser.Username);
                var user = new AdUser();
                user.displayName = dtUser.DisplayName;
                user.userPrincipalName = dtUser.AdUsername;
                user.mailNickname = dtUser.MailNickname;
                user.accountEnabled = true;
                var pwProfile = new PasswordProfile();
                pwProfile.password = WaadConfig.DefaultPassword;
                pwProfile.forceChangePasswordNextLogin = true;
                user.passwordProfile = pwProfile;
                
                if (dtUser.HasAlternateEmail)
                    user.otherMails.Add(dtUser.Username);
                
                directoryDataService.AddTousers(user);
            }

            directoryDataService.SaveChanges();
        }

        private string GetFreshUsername(string username)
        {
            var counter = 0;
            string usernameToInsert;
            bool usernameExistsInAd = true;

            do
            {
                usernameToInsert = DomainSafeUsername(username, counter);

                if (directoryDataService.users.Where(u => u.userPrincipalName == usernameToInsert).FirstOrDefault() != null)
                {
                    counter++;
                }
                else
                    //break out of loop
                    usernameExistsInAd = false;

            } while (usernameExistsInAd);
            
            return usernameToInsert;
        }

        public void DeleteUser(string userPrincipalName)
        {
            AdUser user = directoryDataService.users.Where(it => (it.userPrincipalName == userPrincipalName)).SingleOrDefault();
            if (user == null)
                return;

            directoryDataService.DeleteObject(user);
            directoryDataService.SaveChanges();
        }

        public bool UpdateUser(DtUser user)
        {
            AdUser refreshedUser = directoryDataService.directoryObjects.OfType<AdUser>().Where(it => (it.userPrincipalName == user.AdUsername)).SingleOrDefault();

            if (refreshedUser == null)
                return false;

            refreshedUser.displayName = user.DisplayName;
            directoryDataService.UpdateObject(refreshedUser);
            directoryDataService.SaveChanges(System.Data.Services.Client.SaveChangesOptions.PatchOnUpdate);
            return true;
        }

        public void AddUserGroup(string username, string group)
        {
            AdGroup refreshedGroup = directoryDataService.groups.Where(it => it.displayName == group).SingleOrDefault();
            if (refreshedGroup == null)
                return;
            var users = directoryDataService.users.ToList();
            directoryDataService.AddLink(refreshedGroup, "members", users.Single(it => (it.userPrincipalName == username)));
            directoryDataService.SaveChanges();
        }

        public void AddUserGroupList(IEnumerable<DtUserGroup> userGroups)
        {
            var users = directoryDataService.users.ToList();

            foreach (var usergroup in userGroups)
            {
                AdGroup refreshedGroup = directoryDataService.groups.Where(it => it.displayName == usergroup.Group).SingleOrDefault();
                if (refreshedGroup == null)
                    continue;
                directoryDataService.AddLink(refreshedGroup, "members", users.Single(it => (it.userPrincipalName == usergroup.AdUsername)));
                directoryDataService.SaveChanges();
            }
        }

        public void RemoveUserGroup(string username, string group)
        {
            AdGroup refreshedGroup = directoryDataService.groups.Where(it => it.displayName == group).SingleOrDefault();
            var users = directoryDataService.users.ToList();
            directoryDataService.DeleteLink(refreshedGroup, "members", users.Single(it => (it.userPrincipalName == username)));
            directoryDataService.SaveChanges();
        }

        public void CreateGroup(DtGroup dtGroup)
        {
            if (directoryDataService.groups.Where(u => u.displayName == dtGroup.Name).FirstOrDefault() != null)
                return;

            var group = new AdGroup();
            group.displayName = dtGroup.Name;
            group.mailNickname = dtGroup.MailNickName;
            //make all groups security enabled (i.e. not mailing lists)
            group.securityEnabled = true;
            group.mailEnabled = false;
            directoryDataService.AddTogroups(group);
            directoryDataService.SaveChanges();
        }

        public void CreateGroupList(IEnumerable<DtGroup> dtGroupList)
        {
            foreach (var dtGroup in dtGroupList)
            {
                if (directoryDataService.groups.Where(gp => gp.displayName == dtGroup.Name).FirstOrDefault() != null)
                    continue;

                var group = new AdGroup();
                group.displayName = dtGroup.Name;
                group.mailNickname = dtGroup.MailNickName;
                //make all groups security enabled (i.e. not mailing lists)
                group.securityEnabled = true;
                group.mailEnabled = false;
                directoryDataService.AddTogroups(group);
            }
            
            directoryDataService.SaveChanges();
        }

        public void DeleteGroup(string groupname)
        {
            AdGroup group = directoryDataService.groups.Where(it => (it.displayName == groupname)).SingleOrDefault();
            if (group == null)
                return;
            directoryDataService.DeleteObject(group);
            directoryDataService.SaveChanges();
        }

        public bool IsUserInGroup(string upn, string groupName)
        {
            // Get the Group object.
            AdGroup group = directoryDataService.groups.Where(it => (it.displayName == groupName)).SingleOrDefault();

            if (group == null)
                return false;
            
            directoryDataService.LoadProperty(group, "members");
            
            var currentGroupUsers = group.members.OfType<AdUser>().ToList<AdUser>();

            return currentGroupUsers.Where(cu => cu.userPrincipalName == upn).SingleOrDefault() != null;
        }

        public AdUser GetUserByUpn(string upn)
        {
            return directoryDataService.users.Where(u => u.userPrincipalName == upn).FirstOrDefault();
        }

        private static string DomainSafeUsername(string username, int suffix)
        {
            var name = username.Split('@')[0];
            return string.Format(
                "{0}{1}@{2}",
                name,
                suffix == 0 ? string.Empty : suffix.ToString(),
                WaadConfig.TenantDomainName);
        }
    }
}

