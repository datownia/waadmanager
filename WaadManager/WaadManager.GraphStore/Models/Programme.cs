using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class Programme
    {
        public Programme(int date)
        {
            Date = date;
        }
        
        public Programme()
        {
            Date = 0;
        }

        [JsonIgnoreAttribute]
        public int Date { get; set; }

        [JsonProperty(PropertyName = "_Item1")]
        public string Item1
        {
            get { return "conference"; }
        }

        [JsonProperty(PropertyName = "_Item2")]
        public string Item2
        {
            get { return string.Format("schedule{0}", Date); }
        }

        [JsonProperty(PropertyName = "value")]
        public IEnumerable<Event> Values { get; set; }

        public void UpdateFavourites(IEnumerable<Favourite> faves)
        {
            if (faves == null)
                return;
            foreach (var e in Values)
            {
                if (faves.Any(f => f.EventCode.ToUpperInvariant() == e.Code.ToUpperInvariant()))
                    e.IsFave = true;
            }
        }
    }
}
