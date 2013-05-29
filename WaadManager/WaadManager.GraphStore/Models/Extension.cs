using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class Extension : Tuple<string, string>
    {
        public Extension(string item1, string item2) : base(item1, item2)
        {
        }

        [JsonProperty(PropertyName = "_Item1")]
        public new string Item1
        {
            get { return base.Item1; }
        }

        [JsonProperty(PropertyName = "_Item2")]
        public new string Item2
        {
            get { return base.Item2; }
        }

        [JsonProperty(PropertyName = "OwningTenant")]
        public string OwningTenant { get; set; }

        [JsonProperty(PropertyName = "ValueFormat")]
        public string ValueFormat { get; set; }
    }
}
