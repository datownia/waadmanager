using System.Data.SqlClient;
using WaadManager.Common.Sql;

namespace WaadManager.Common.Models
{
    public class UserGroup : IDataReadable<UserGroup>
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Group { get; set; }

        public UserGroup FromDataReader(SqlDataReader reader)
        {
            Id = reader["datowniaId"].ToString();
            Username = reader["username"].ToString();
            Group = reader["groupname"].ToString();
            return this;
        }
    }
}
