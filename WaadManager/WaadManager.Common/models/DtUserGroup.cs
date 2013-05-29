using Newtonsoft.Json.Linq;

namespace WaadManager.Common.Models
{
    public class DtUserGroup : DatowniaContent<DtUserGroup>
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Group { get; set; }

        public string AdUsername { get; set; }

        public override DtUserGroup FromContentRow(JToken row)
        {
            Id = row[WaadConfig.UserGroupIdFieldIndex].Value<string>();
            Group = row[WaadConfig.UserGroupGroupFieldIndex].Value<string>();
            Username = row[WaadConfig.UserGroupUsernameFieldIndex].Value<string>();
            return this;
        }
    }
}
