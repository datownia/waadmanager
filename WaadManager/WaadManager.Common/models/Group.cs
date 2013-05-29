using System.Data.SqlClient;
using WaadManager.Common.Sql;

namespace WaadManager.Common.Models
{
    public class Group : IDataReadable<Group>
    {
        public string Id { get; set; }
        public string Groupname { get; set; }

        public Group FromDataReader(SqlDataReader reader)
        {
            Id = reader["datowniaId"].ToString();
            Groupname = reader["groupname"].ToString();
            return this;
        }
    }
}
