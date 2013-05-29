using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class Group : IODataEntity
    {
        [JsonProperty(PropertyName = "odata.type")]
        public string ODataType { get; set; }

        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; set; }

        [JsonProperty(PropertyName = "objectId")]
        public string ObjectId { get; set; }

        [JsonProperty(PropertyName = "description")]
        public object Description { get; set; }

        [JsonProperty(PropertyName = "dirSyncEnabled")]
        public object DirSyncEnabled { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "lastDirSyncTime")]
        public object LastDirSyncTime { get; set; }

        [JsonProperty(PropertyName = "mail")]
        public object Mail { get; set; }

        [JsonProperty(PropertyName = "mailNickname")]
        public string MailNickname { get; set; }

        [JsonProperty(PropertyName = "mailEnabled")]
        public bool MailEnabled { get; set; }

//        public List<object> provisioningErrors { get; set; }
//        public List<object> proxyAddresses { get; set; }

        [JsonProperty(PropertyName = "securityEnabled")]
        public bool SecurityEnabled { get; set; }
    }
}
