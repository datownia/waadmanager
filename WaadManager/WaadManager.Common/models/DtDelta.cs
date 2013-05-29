using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WaadManager.Common.Models
{
    public class DtDelta : DatowniaContent<DtDelta>
    {
        public const string InsertAction = "insert";
        public const string DeleteAction = "delete";
        public string Id { get; set; }
        public string Parent { get; set; }
        public string Rev { get; set; }
        public int Seq { get; set; }

        public string Action { get; set; }
        public string Data { private get; set; }

        private JArray DataArray
        {
            get {
                try
                {
                    return JArray.Parse(Data);
                }
                catch (JsonReaderException)
                {
                    if (IsDelete)
                        return new JArray() { Data };
                    return new JArray();
                }
            }
        }

        public string DataForDelete
        {
            get
            {
                return Data;
            }
        }

        public DtUser DataAsUser
        {
            get
            {
                if (IsDelete)
                    throw new Exception("Cannot create user from this delta data");

                var user = new DtUser();
                //indices from deltas
                user.Id = DataArray[WaadConfig.UserIdFieldIndex].ToString();
                user.DisplayName = DataArray[WaadConfig.UserDisplayNameFieldIndex].ToString();
                user.Username = DataArray[WaadConfig.UserUsernameFieldIndex].ToString();
                return user;
            }
        }

        public DtGroup DataAsGroup
        {
            get
            {
                if (IsDelete)
                    throw new Exception("Cannot create role from this delta data");

                var group = new DtGroup();
                //indices from deltas
                group.Id = DataArray[WaadConfig.GroupIdFieldIndex].ToString();
                group.Name = DataArray[WaadConfig.GroupNameFieldIndex].ToString();
                return group;
            }
        }

        public DtUserGroup DataAsUserGroup
        {
            get
            {
                if (IsDelete)
                    throw new Exception("Cannot create userrole from this delta data");

                var usergroup = new DtUserGroup();
                //indices from deltas
                usergroup.Id = DataArray[WaadConfig.UserGroupIdFieldIndex].ToString();
                usergroup.Group = DataArray[WaadConfig.UserGroupGroupFieldIndex].ToString();
                usergroup.Username = DataArray[WaadConfig.UserGroupUsernameFieldIndex].ToString();
                return usergroup;
            }
        }

        public DtEvent DataAsEvent
        {
            get
            {
                if (IsDelete)
                    throw new Exception("Cannot create event from this delta data");

                var _event = new DtEvent().FromContentRow(DataArray);
                return _event;
            }
        }

        public bool IsDelete
        {
            get { return Action == DeleteAction; }
        }

        public override DtDelta FromContentRow(JToken row)
        {
            Id = row["_id"].Value<string>();
            Rev = row["_rev"].Value<string>();
            Parent = ParentDocumentId(row["parent"].Value<string>());
            Seq = int.Parse(row["seq"].Value<string>());
            Action = row["action"].Value<string>();
            Data = row["data"].ToString();
            return this;
        }
    }
}
