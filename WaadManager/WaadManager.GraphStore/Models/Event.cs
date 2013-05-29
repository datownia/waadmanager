using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class Event
    {
        [JsonProperty(PropertyName = "_code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "_title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "_area")]
        public string Area { get; set; }

        [JsonProperty(PropertyName = "_day")]
        public string Day { get; set; }

        [JsonProperty(PropertyName = "_location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "_speakers")]
        public string Speakers { get; set; }

        [JsonProperty(PropertyName = "_time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "_dtId")]
        public string DatowniaId { get; set; }

        [JsonIgnoreAttribute]
        public bool IsFave { get; set; }
    }
}
