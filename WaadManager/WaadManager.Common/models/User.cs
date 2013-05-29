using System.Data.SqlClient;
using WaadManager.Common.Sql;

namespace WaadManager.Common.Models
{
    public class User : IDataReadable<User>
    {
        public string Id { get; set; }
        public string Username { get;  set; }
        public string AdUsername{ get; set; }

        public User FromDataReader(SqlDataReader reader)
        {
            Id = reader["datowniaId"].ToString();
            Username = reader["username"].ToString();
            AdUsername= reader["adUsername"].ToString();
            return this;
        }
    }
}
