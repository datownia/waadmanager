using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaadManager.GraphStore.Models
{
    public interface IODataEntityCollection<T> where T:IODataEntity
    {
        string MetaData { get; set; }
        IEnumerable<T> Values { get; set; }
    }
}
