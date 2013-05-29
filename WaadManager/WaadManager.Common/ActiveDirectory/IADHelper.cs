using System.Collections.Generic;
using WaadManager.Common.Models;
using AdUser = Microsoft.WindowsAzure.ActiveDirectory.User;

namespace WaadManager.Common.ActiveDirectory
{
    public interface IADHelper
    {
        void CreateUser(ref DtUser dtUser);
        void CreateUserList(ref IEnumerable<DtUser> dtUserList);
        void DeleteUser(string userPrincipalName);
        bool UpdateUser(DtUser user);
        
        void AddUserGroup(string username, string group);
        void AddUserGroupList(IEnumerable<DtUserGroup> userGroups);
        void RemoveUserGroup(string username, string group);
        
        void CreateGroup(DtGroup dtGroup);
        void CreateGroupList(IEnumerable<DtGroup> dtGroupList);
        void DeleteGroup(string groupname);

        bool IsUserInGroup(string username, string groupName);

        AdUser GetUserByUpn(string upn);
    }
}
