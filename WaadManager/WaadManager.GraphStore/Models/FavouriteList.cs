using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class FavouriteList
    {
        [JsonProperty(PropertyName = "value")]
        public IEnumerable<Favourite> Values { get; set; }
    }
}
