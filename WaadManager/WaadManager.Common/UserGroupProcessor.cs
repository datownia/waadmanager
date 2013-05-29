using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WaadManager.Common.ActiveDirectory;
using WaadManager.Common.Datownia;
using WaadManager.Common.Models;
using WaadManager.Common.Sql;
using WaadManager.Common.models;

namespace WaadManager.Common
{
    /// <summary>
    /// This is where the magic happens.
    /// </summary>
    public class UserGroupProcessor : IUserGroupProcessor
    {
        private IDatowniaHelper datowniaService;
        private IADHelper adservice;
        private ISqlHelper sqlService;

        public UserGroupProcessor()
        {
            datowniaService = new DatowniaHelper();
            adservice = new ADHelper();
            sqlService = new SqlHelper();
        }

        public UserGroupProcessor(IDatowniaHelper datowniaHelper, IADHelper graphHelper, ISqlHelper sqlHelper)
        {
            datowniaService = datowniaHelper;
            adservice = graphHelper;
            sqlService = sqlHelper;
        }

        public void RunUpdate()
        {
            if (!Initalize())
                return;

			//try
			//{
                var latestUserSeq = sqlService.GetLatestLocalSeq(WaadConfig.UserApiFullName);
                var latestGroupSeq = sqlService.GetLatestLocalSeq(WaadConfig.GroupApiFullName);
                var latestUserGroupSeq = sqlService.GetLatestLocalSeq(WaadConfig.UserGroupApiFullName);

                var latestUserSeqFromApi = latestUserSeq;
                var latestGroupSeqFromApi = latestGroupSeq;
                var latestUserGroupSeqFromApi = latestUserGroupSeq;

                var userDeltas = datowniaService.GetDeltas(WaadConfig.UserApiFullName, latestUserSeq);
                var groupDeltas = datowniaService.GetDeltas(WaadConfig.GroupApiFullName, latestGroupSeq);
                var userGroupDeltas = datowniaService.GetDeltas(WaadConfig.UserGroupApiFullName, latestUserGroupSeq);

                //1. run user update (to create and delete)
                var useractions = new List<DeltaAction>();
                if (userDeltas.Any())
                {
                    useractions = PairUserDeltas(userDeltas);
                    foreach (var deltaAction in useractions)
                        ResolveUserDelta(deltaAction);
                    
                    //update sequence number
                    latestUserSeqFromApi = userDeltas.OrderByDescending(ud => ud.Seq).First().Seq;
                    if (latestUserSeqFromApi > latestUserSeq)
                        sqlService.SetLatestLocalSeq(WaadConfig.UserApiFullName, latestUserSeqFromApi);
                }

                //2.a run role update (to create new)
                var groupactions = new List<DeltaAction>();
                var groupDeletions = new List<DtDelta>();
                var groupInsertions = new List<DtDelta>();
                if (groupDeltas.Any())
                {
                    latestGroupSeqFromApi = groupDeltas.OrderByDescending(ud => ud.Seq).First().Seq;
                    groupactions = PairGroupDeltas(groupDeltas);
                    SplitRoleActions(groupactions, groupInsertions, groupDeletions);
                    foreach (var insert in groupInsertions)
                        CreateGroup(insert);
                }

                //2.b run userrole update (to add new)
                //2.c run userrole update (to delete corresponding role deletions)
                if (userGroupDeltas.Any())
                {
                    var userRoleactions = RemoveRedundantDeltas(userGroupDeltas);
                    //process userrole updates
                    foreach (var userRoleaction in userRoleactions.OrderBy(a => a.Seq))
                        ResolveUserGroupDelta(userRoleaction);

                    //update sequence number
                    latestUserGroupSeqFromApi = userGroupDeltas.OrderByDescending(ud => ud.Seq).First().Seq;
                    if (latestUserGroupSeqFromApi > latestUserGroupSeq)
                        sqlService.SetLatestLocalSeq(WaadConfig.UserGroupApiFullName, latestUserGroupSeqFromApi);
                }

                //3. run role update (to delete unwanted)
                foreach (var insert in groupDeletions)
                    DeleteGroup(insert);

                //update sequence number
                if (latestGroupSeqFromApi > latestGroupSeq)
                    sqlService.SetLatestLocalSeq(WaadConfig.GroupApiFullName, latestGroupSeqFromApi);
			//}
			//catch (Exception ex)
			//{
			//	//TODO: Log exception
			//}
        }

        private bool Initalize()
        {
			//try
			//{
                sqlService.TryConnectDb();

                sqlService.EnsureUserGroupDeltaRecordsExist();

                //Is user data set up?
                if (!sqlService.IsUserTablePopulated())
                {
                    var seq = 0;
                    var userList = datowniaService.GetAllUsers(out seq);
                    adservice.CreateUserList(ref userList);
                    sqlService.InsertUserList(userList);
                    sqlService.SetLatestLocalSeq(WaadConfig.UserApiFullName, seq);
                    //removed auto email
                    //emailservice.SendWelcomeEmail(userList);
                }

                //Is role data set up?
                if (!sqlService.IsGroupTablePopulated())
                {
                    var seq = 0;
                    var groupList = datowniaService.GetAllGroups(out seq);
                    sqlService.InsertGroupList(groupList);
                    adservice.CreateGroupList(groupList);
                    sqlService.SetLatestLocalSeq(WaadConfig.GroupApiFullName, seq);
                }

                //Is userrole data set up?
                if (!sqlService.IsUserGroupTablePopulated())
                {
                    var seq = 0;
                    var usergroupList = datowniaService.GetAllUserGroups(out seq);
                    sqlService.InsertUserGroupList(usergroupList);
                    //need to update usergrouplist with the adUsername values
                    sqlService.GetAdUsernames(ref usergroupList);
                    adservice.AddUserGroupList(usergroupList);
                    sqlService.SetLatestLocalSeq(WaadConfig.UserGroupApiFullName, seq);
                }
			//}
			//catch (Exception ex)
			//{
			//	//TODO: Log Exception
			//	return false;
			//}

            return true;
        }

        private List<DeltaAction> PairUserDeltas(IEnumerable<DtDelta> deltas)
        {
            var inserts = deltas.Where(d => !d.IsDelete);
            var deletes = deltas.Where(d => d.IsDelete);
            var result = new List<DeltaAction>(); //old values, new values
            
            foreach (var rootDelta in deltas)
            {
                //this is the root entry
                //recurse through other entries to find final related entry
                var foundEnd = false;
                var latestDelta = rootDelta;
                do
                {
                    DtDelta temp = null;
                    temp = FindNextRelatedUserDelta(latestDelta, inserts, deletes);

                    if (temp != null)
                        latestDelta = temp;
                    else
                        foundEnd = true;
                } while (!foundEnd);

                InsertDedupeDeltaAction(rootDelta, latestDelta, result);
            }

            return result;
        }

        private List<DeltaAction> PairGroupDeltas(IEnumerable<DtDelta> deltas)
        {
            var inserts = deltas.Where(d => !d.IsDelete);
            var deletes = deltas.Where(d => d.IsDelete);
            var result = new List<DeltaAction>(); //old values, new values

            foreach (var rootDelta in deltas)
            {
                //this is the root entry
                //recurse through other entries to find final related entry
                var foundEnd = false;
                var latestDelta = rootDelta;
                do
                {
                    DtDelta temp = null;
                    temp = FindNextRelatedGroupDelta(latestDelta, inserts, deletes);

                    if (temp != null)
                        latestDelta = temp;
                    else
                        foundEnd = true;
                } while (!foundEnd);

                InsertDedupeDeltaAction(rootDelta, latestDelta, result);
            }

            return result;
        }

        private List<DtDelta> RemoveRedundantDeltas(IEnumerable<DtDelta> deltas)
        {
            //normally, we follow the chain of alternate deletes and inserts
            //resulting in:
            //a delete then insert => this is an update
            //or a delete then delete (via one or more inserts) => this is a delete of the old value
            //or an insert then delete => cancels out
            //or an insert then insert (via one or more deletes) => only insert the latest value

            //BUT
            //in this case, we need to do this:
            //if it's a delete we can't check for subsequent inserts, due to compound key.
            //  we cannot find a chain. this means we process the delete on AD individually.
            //if it's an insert, we can look for a corresponding deletion and ignore both
            //  if no deletion is found, then we're looking at a standalone insertion. process as such.

            var deletes = deltas.Where(d => d.IsDelete);
            var result = new List<DtDelta>(); //old values, new values

            foreach (var rootDelta in deltas)
            {
                DtDelta nextDelta = null;

                if (rootDelta.IsDelete)
                {
                    //it is a delete.
                    result.Add(rootDelta);
                }
                else
                {
                    nextDelta = FindSubsequentUserRoleDeletion(rootDelta, deletes);
                    if (nextDelta == null)
                    {
                        //data is an insert.
                        result.Add(rootDelta);
                    }
                    //else the insert gets deleted so can be ignored
                }
            }

            return result;
        }

        private void InsertDedupeDeltaAction(DtDelta rootDelta, DtDelta latestDelta, List<DeltaAction> result)
        {
            if (rootDelta.Seq == latestDelta.Seq)
            {
                //if rootDelta == latestDelta, then there are no later sequenced deltas.
                //but there may be prior ones in the results.
                //  check if rootDelta exists as new delta. if not, then add
                if (!result.Exists(d => d.NewDelta.Seq == rootDelta.Seq))
                    result.Add(new DeltaAction(rootDelta, latestDelta));
                //else can ignore, as will be picked up by resolver
            }
            else
            {
                if (result.Exists(d => d.NewDelta.Seq == latestDelta.Seq))
                {
                    //if latestDelta exists as new delta
                    //then old delta should be identical. check, then set oldest as rootDelta.
                    var existing = result.First(d => d.NewDelta.Seq == latestDelta.Seq);
                    if (existing.OldDelta.Seq > rootDelta.Seq)
                    {
                        result.Remove(existing);
                        result.Add(new DeltaAction(rootDelta, latestDelta));
                    }
                }
                else
                    result.Add(new DeltaAction(rootDelta, latestDelta));
            }
        }

        private DtDelta FindNextRelatedUserDelta(DtDelta rootDtDelta, IEnumerable<DtDelta> allInserts, IEnumerable<DtDelta> allDeletes)
        {
            DtDelta result = null;

            if (rootDtDelta.Action == DtDelta.DeleteAction)
            {
                //get username for this deletion id
                var usernameToDelete = string.Empty;
                var earlierInsert = allInserts.
                    FirstOrDefault(i => i.Seq < rootDtDelta.Seq
                               && i.DataAsUser.Id == rootDtDelta.DataForDelete);

                if (earlierInsert != null)
                    usernameToDelete = earlierInsert.DataAsUser.Username;

                //if there's no value in the array, there may be one in the db
                if (string.IsNullOrWhiteSpace(usernameToDelete))
                    usernameToDelete = sqlService.GetUserForId(rootDtDelta.DataForDelete).Username;

                //if you can't find a record for this Id, you can't proceed. Something is wrong with data.
                if (string.IsNullOrWhiteSpace(usernameToDelete))
                    throw new Exception(string.Format("No user insert record found for this id: {0}", rootDtDelta.DataForDelete));

                //search for next insert for same username as delete
                var insertsForUsername = allInserts.
                    Where(i => i.Seq > rootDtDelta.Seq
                               && i.DataAsUser.Username == usernameToDelete)
                                                   .OrderBy(a => a.Seq);
                if (insertsForUsername.Any())
                    result = insertsForUsername.First();
            }
            else if (rootDtDelta.Action == DtDelta.InsertAction)
            {
                //search for delete action for same id
                var deletesForId = allDeletes.
                    Where(i => i.Seq > rootDtDelta.Seq
                               && i.DataForDelete == rootDtDelta.DataAsUser.Id);
                if (deletesForId.Any())
                    result = deletesForId.First();
            }

            return result;
        }

        private DtDelta FindNextRelatedGroupDelta(DtDelta rootDtDelta, IEnumerable<DtDelta> allInserts, IEnumerable<DtDelta> allDeletes)
        {
            DtDelta result = null;

            if (rootDtDelta.Action == DtDelta.DeleteAction)
            {
                //get username for this deletion id
                var groupnameToDelete = string.Empty;
                var earlierInsert = allInserts.
                    FirstOrDefault(i => i.Seq < rootDtDelta.Seq
                               && i.DataAsGroup.Id == rootDtDelta.DataForDelete);

                if (earlierInsert != null)
                    groupnameToDelete = earlierInsert.DataAsGroup.Name;

                //if there's no value in the array, there may be one in the db
                if (string.IsNullOrWhiteSpace(groupnameToDelete))
                    groupnameToDelete = sqlService.GetGroupnameForId(rootDtDelta.DataForDelete);

                //if you can't find a record for this Id, you can't proceed. Something is wrong with data.
                if (string.IsNullOrWhiteSpace(groupnameToDelete))
                    throw new Exception(string.Format("No role insert record found for this id: {0}", rootDtDelta.DataForDelete));

                //search for next insert for same username as delete
                var insertsForGroupname = allInserts.
                    Where(i => i.Seq > rootDtDelta.Seq
                               && i.DataAsGroup.Name == groupnameToDelete)
                                                   .OrderBy(a => a.Seq);
                if (insertsForGroupname.Any())
                    result = insertsForGroupname.First();
            }
            else if (rootDtDelta.Action == DtDelta.InsertAction)
            {
                //search for delete action for same id
                var deletesForId = allDeletes.
                    Where(i => i.Seq > rootDtDelta.Seq
                               && i.DataForDelete == rootDtDelta.DataAsGroup.Id);
                if (deletesForId.Any())
                    result = deletesForId.First();
            }

            return result;
        }

        private DtDelta FindSubsequentUserRoleDeletion(DtDelta rootDtDelta, IEnumerable<DtDelta> allDeletes)
        {
            DtDelta result = null;

            if (!rootDtDelta.IsDelete)
            {
                //search for delete action for same id
                result = allDeletes.
                    FirstOrDefault(i => i.Seq > rootDtDelta.Seq
                               && i.DataForDelete == rootDtDelta.DataAsUserGroup.Id);
            }

            return result;
        }

        private void ResolveUserDelta(DeltaAction action)
        {
            if (action.OldDelta.IsDelete)
            {
                //...and final is delete, delete AD (update sql)
                if (action.NewDelta.IsDelete)
                {
                    DeleteUser(action.OldDelta);
                }
                //...and final is insert, update AD (if AD entry exists for username)
                else
                {
                    UpdateUser(action.NewDelta);
                }
            }
            //if root is insert...
            else
            {
                //...and final is insert, insert AD
                if (!action.NewDelta.IsDelete)
                {
                    CreateUser(action.NewDelta);
                }
                //...and final is delete, there's nothing to do
            }
        }

        private void ResolveUserGroupDelta(DtDelta action)
        {
            if (action.IsDelete)
            {
                //remove from AD
                var userRole = sqlService.GetUserGroupForId(action.DataForDelete);
                if (userRole != null)
                {
                    var user = sqlService.GetUserByUsername(userRole.Username);
                    if (user != null)
                    {
                        var adUsername = user.AdUsername;
                        adservice.RemoveUserGroup(adUsername, userRole.Group);
                    }

                    //remove from sql
                    sqlService.RemoveUserGroupRecord(action.DataForDelete);
                }
            }
            else
            {
                //add to AD
                var userRole = action.DataAsUserGroup;
	            var userByUsername = sqlService.GetUserByUsername(userRole.Username);
				if (userByUsername == null)
				{
					Trace.WriteLine("Could not find user related to group");
					return;
				}

	            var adUsername = userByUsername.AdUsername;
                adservice.AddUserGroup(adUsername, userRole.Group);

                //insert into sql
                sqlService.CreateUserGroupRecord(userRole);
            }
        }

        private static void SplitRoleActions(IList<DeltaAction> pairs, IList<DtDelta> inserts, IList<DtDelta> deletes)
        {
            foreach (var action in pairs)
            {
                if (action.OldDelta.IsDelete)
                {
                    //...and final is delete, process first delete
                    if (action.NewDelta.IsDelete)
                    {
                        deletes.Add(action.OldDelta);
                    }
                    //...and final is insert, proces delete then insert (in seq order)
                    else
                    {
                        deletes.Add(action.OldDelta);
                        inserts.Add(action.NewDelta);
                    }
                }
                //if root is insert...
                else
                {
                    //...and final is insert, insert new role
                    if (!action.NewDelta.IsDelete)
                    {
                        inserts.Add(action.NewDelta);
                    }
                    //...and final is delete, there's nothing to do
                }
            }
        }

        private void DeleteUser(DtDelta delta)
        {
            var adusername = sqlService.GetUserForId(delta.DataForDelete).AdUsername;
            adservice.DeleteUser(adusername);
            sqlService.RemoveUserRecord(delta.DataForDelete);
        }

        private void CreateUser(DtDelta delta)
        {
            var user = delta.DataAsUser;
            adservice.CreateUser(ref user);
            sqlService.CreateUserRecord(user);
            //removed autoemail
            //emailservice.SendWelcomeEmail(user);
        }

        private void UpdateUser(DtDelta delta)
        {
            var userData = delta.DataAsUser;
            var savedUser = sqlService.GetUserForId(userData.Id);
            userData.AdUsername = savedUser.AdUsername;
            if (adservice.UpdateUser(delta.DataAsUser))
                sqlService.UpdateUserRecord(userData);
        }

        private void DeleteGroup(DtDelta delta)
        {
            var username = sqlService.GetGroupnameForId(delta.DataForDelete);
            adservice.DeleteGroup(username);
            sqlService.RemoveGroupRecord(username);
        }

        private void CreateGroup(DtDelta delta)
        {
            adservice.CreateGroup(delta.DataAsGroup);
            sqlService.CreateGroupRecord(delta.DataAsGroup);
        }
    }
}
