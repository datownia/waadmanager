using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WaadManager.Common.Models
{
    public class DtUser : DatowniaContent<DtUser>
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string MailNickname
        {
            get { return Username.Split('@')[0]; }
        }
        
        public string AdUsername { get; set; }

        public bool HasAlternateEmail
        {
            get { return AdUsername != Username; }
        }

        public override DtUser FromContentRow(JToken row)
        {
            Id = row[WaadConfig.UserIdFieldIndex].Value<string>();
            DisplayName = row[WaadConfig.UserDisplayNameFieldIndex].Value<string>();
            Username = row[WaadConfig.UserUsernameFieldIndex].Value<string>();
            return this;
        }
    }
}
