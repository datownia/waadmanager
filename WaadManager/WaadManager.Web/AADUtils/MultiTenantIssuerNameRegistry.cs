using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens;
using System.Xml.Linq;

namespace WaadManager.Web.AADUtils
{
    public class MultiTenantIssuerNameRegistry : ValidatingIssuerNameRegistry
    {
        private static XDocument doc;
        private static string filePath;

        static MultiTenantIssuerNameRegistry()
        {
            filePath = HttpContext.Current.Server.MapPath("~/Content/tenants.xml");
            doc = XDocument.Load(filePath);
        }

        public static bool ContainsTenant(string tenantId)
        {
            return
            doc.Descendants("tenant").Where(x => x.Attribute("id").Value == tenantId).Any();
        }

        public static bool ContainsKey(string thumbprint)
        {
            return
            doc.Descendants("key").Where(x => x.Attribute("id").Value == thumbprint).Any();
        }

        protected override bool IsThumbprintValid(string thumbprint, string issuer)
        {
            string issuerID = issuer.TrimEnd('/').Split('/').Last();

            if (ContainsTenant(issuerID))
            {
                if (ContainsKey(thumbprint))
                    return true;
            }
            return false;
        }

        public static void RefreshKeys(string metadataAddress)
        {
            IssuingAuthority ia =
                   ValidatingIssuerNameRegistry.GetIssuingAuthority(metadataAddress);

            bool newKeys = false;
            foreach (string thumbp in ia.Thumbprints)
                if (!ContainsKey(thumbp))
                {
                    newKeys = true;
                    break;
                }

            if (newKeys)
            {
                XElement keysRoot =
                     (XElement)(from tt in doc.Descendants("keys") select tt).First();
                keysRoot.RemoveNodes();
                foreach (string thumbp in ia.Thumbprints)
                {
                    XElement node = new XElement("key", new XAttribute("id", thumbp));
                    keysRoot.Add(node);
                }
                doc.Save(filePath);
            }
        }

        public static void AddTenant(string tenantId)
        {
            if (!ContainsTenant(tenantId))
            {
                XElement node = new XElement("tenant", new XAttribute("id", tenantId));
                XElement tenantsRoot =
                        (XElement)(from tt in doc.Descendants("tenants") select tt).First();

                tenantsRoot.Add(node);
                doc.Save(filePath);
            }
        }
    }
}