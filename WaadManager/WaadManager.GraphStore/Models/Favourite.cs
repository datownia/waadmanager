using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class Favourite : IODataEntity
    {
        [JsonProperty(PropertyName = "_Item1")]
        public string Upn { get; set; }

        [JsonProperty(PropertyName = "_Item2")]
        public string EventCode { get; set; }

        public string ODataType { get; set; }
    }
}
