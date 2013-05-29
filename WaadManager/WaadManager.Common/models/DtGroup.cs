using System;
using Newtonsoft.Json.Linq;

namespace WaadManager.Common.Models
{
    public class DtGroup : DatowniaContent<DtGroup>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        
        public string MailNickName
        {
            get { return String.Join(string.Empty, Name.Split(' ')); }
        }

        public override DtGroup FromContentRow(JToken row)
        {
            Id = row[WaadConfig.GroupIdFieldIndex].Value<string>();
            Name = row[WaadConfig.GroupNameFieldIndex].Value<string>();;
            return this;
        }
    }
}
