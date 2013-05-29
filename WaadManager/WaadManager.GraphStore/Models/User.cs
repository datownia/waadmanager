using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaadManager.GraphStore.Models
{
    public class User : IODataEntity
    {
        [JsonProperty(PropertyName = "odata.type")]
        public string ODataType { get; set; }

        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; set; }
        
        [JsonProperty(PropertyName = "objectId")]
        public string ObjectId { get; set; }

        [JsonProperty(PropertyName = "accountEnabled")]
        public bool AccountEnabled { get; set; }

//        public List<object> assignedLicenses { get; set; }
//        public List<object> assignedPlans { get; set; }
//        public List<object> provisionedPlans { get; set; }
//        public List<object> provisioningErrors { get; set; }
//        public List<object> proxyAddresses { get; set; }

        [JsonProperty(PropertyName = "city")]
        public object City { get; set; }

        [JsonProperty(PropertyName = "country")]
        public object Country { get; set; }

        [JsonProperty(PropertyName = "department")]
        public object Department { get; set; }

        [JsonProperty(PropertyName = "dirSyncEnabled")]
        public object DirSyncEnabled { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "facsimileTelephoneNumber")]
        public object FacsimileTelephoneNumber { get; set; }

        [JsonProperty(PropertyName = "givenName")]
        public string GivenName { get; set; }

        [JsonProperty(PropertyName = "jobTitle")]
        public object JobTitle { get; set; }

        [JsonProperty(PropertyName = "lastDirSyncTime")]
        public object LastDirSyncTime { get; set; }

        [JsonProperty(PropertyName = "mail")]
        public object Mail { get; set; }

        [JsonProperty(PropertyName = "mailNickname")]
        public string MailNickname { get; set; }

        [JsonProperty(PropertyName = "mobile")]
        public string Mobile { get; set; }

        [JsonProperty(PropertyName = "otherMails")]
        public List<string> OtherMails { get; set; }

        [JsonProperty(PropertyName = "passwordPolicies")]
        public string PasswordPolicies { get; set; }

        [JsonProperty(PropertyName = "passwordProfile")]
        public object PasswordProfile { get; set; }

        [JsonProperty(PropertyName = "physicalDeliveryOfficeName")]
        public object PhysicalDeliveryOfficeName { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public object PostalCode { get; set; }

        [JsonProperty(PropertyName = "preferredLanguage")]
        public object PreferredLanguage { get; set; }

        [JsonProperty(PropertyName = "state")]
        public object State { get; set; }

        [JsonProperty(PropertyName = "streetAddress")]
        public object StreetAddress { get; set; }

        [JsonProperty(PropertyName = "surname")]
        public string Surname { get; set; }

        [JsonProperty(PropertyName = "telephoneNumber")]
        public object TelephoneNumber { get; set; }

        [JsonProperty(PropertyName = "thumbnailPhoto@odata.mediaContentType")]
        public string thumbnailPhoto { get; set; }

        [JsonProperty(PropertyName = "usageLocation")]
        public object UsageLocation { get; set; }

        [JsonProperty(PropertyName = "userPrincipalName")]
        public string UserPrincipalName { get; set; }
    }
}
