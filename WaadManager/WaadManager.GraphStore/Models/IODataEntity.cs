using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public interface IODataEntity
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        string ODataType { get; set; }
    }
}
