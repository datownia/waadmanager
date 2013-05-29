using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class GroupList : IODataEntityCollection<Group>
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string MetaData { get; set; }

        [JsonProperty(PropertyName = "value")]
        public IEnumerable<Group> Values { get; set; }
    }
}
