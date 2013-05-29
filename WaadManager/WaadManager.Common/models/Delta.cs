using System.Data.SqlClient;
using WaadManager.Common.Sql;

namespace WaadManager.Common.Models
{
    public class Delta : IDataReadable<Delta>
    {
        public int Id { get; set; }
        public string Environment { get; set; }
        public string ApiName { get; set; }
        public int DeltaVal { get; set; }
        public string Notes { get; set; }

        public Delta()
        {
        }

        public Delta GetNew()
        {
            return new Delta();
        }

        public Delta FromDataReader(SqlDataReader reader)
        {
            Id = int.Parse(reader["id"].ToString());
            Environment = reader["environment"].ToString();
            ApiName = reader["apiname"].ToString();
            if (!string.IsNullOrWhiteSpace(reader["delta"].ToString()))
                DeltaVal = int.Parse(reader["delta"].ToString());
            Notes = reader["notes"].ToString();
            return this;
        }
    }
}
