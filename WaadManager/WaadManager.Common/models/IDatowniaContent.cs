using Newtonsoft.Json.Linq;

namespace WaadManager.Common.Models
{
    public abstract class DatowniaContent <T>
    {
        public abstract T FromContentRow(JToken row);
        
        protected string ParentDocumentId(string parent)
        {
            //replace "^~^" with '/'
            var reinsertSlash = parent.Replace("^~^", "/");
            //remove trailing version number
            var indexOfVersion = reinsertSlash.LastIndexOf('_');
            return reinsertSlash.Substring(0, indexOfVersion);
        }
    }
}
