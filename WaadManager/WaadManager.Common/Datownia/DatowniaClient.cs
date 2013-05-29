using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;

namespace WaadManager.Common.Datownia
{
    public class DatowniaClient:  ClientBase
    {

        protected DatowniaClient(AuthorizationServerDescription authorizationServer, string clientIdentifier = null, string clientSecret = null)
            : base(authorizationServer, clientIdentifier, clientSecret)
        {
        }

        public DatowniaClient(string clientIdentifier = null, string clientCredential = null)
            : base(ApiDescription, clientIdentifier, clientCredential)
        {
        }

        public static AuthorizationServerDescription ApiDescription
        {
            get
            {
                return new AuthorizationServerDescription
                {
                    TokenEndpoint = new Uri(string.Format("https://{0}/oauth2/token", WaadConfig.OAuthEndpoint)),
                    AuthorizationEndpoint = new Uri(string.Format("https://{0}/oauth2/authorize", WaadConfig.OAuthEndpoint))
                };
            }
        }

        private string GetDocPath(string docName, int offset, int limit)
        {
            var docPath = new StringBuilder(string.Format("https://{0}/api/doc/{1}/vmax/{2}?", WaadConfig.ApiEndPoint, WaadConfig.Publisher, docName));
            if (offset > 0)
            {
                docPath.Append("offset=").Append(offset).Append("&");
            }

            if (limit > 0)
            {
                docPath.Append("limit=").Append(limit);
            }
            return docPath.ToString();
        }

        private string GetDeltaPath(string docName, int offset, int limit, int seq)
        {
            var docPath = new StringBuilder(string.Format("https://{0}/api/doc/{1}/vmax/delta/{2}?", WaadConfig.ApiEndPoint, WaadConfig.Publisher, docName));
            if (offset > 0)
            {
                docPath.Append("offset=").Append(offset);
                if (limit > 0)
                {
                    docPath.Append("&limit=").Append(limit);
                }
                if (seq > 0)
                {
                    docPath.Append("&seq=").Append(seq);
                }
            }
            else if (limit > 0)
            {
                docPath.Append("limit=").Append(limit);
                if (seq > 0)
                {
                    docPath.Append("&seq=").Append(seq);
                }
            }
            else if (seq > 0)
            {
                docPath.Append("seq=").Append(seq);
            }
            

            

            return docPath.ToString();
        }

        public string GetDocument(string docName, int offset, int limit)
        {
            var docPath = GetDocPath(docName, offset, limit);
            var scope = string.Format("Read|{0}|{1}", WaadConfig.Publisher, docName);
            return GetQuery(docPath, scope);
        }

        public string GetDelta(string docName, int offset, int limit, int seq)
        {
            var docPath = GetDeltaPath(docName, offset, limit, seq);
            var scope = string.Format("Read|{0}|{1}", WaadConfig.Publisher, docName);
            return GetQuery(docPath, scope);
        }

        private string GetQuery(string docPath, string scope)
        {
            ServicePointManager.ServerCertificateValidationCallback = TrustAllCallback;
            var scopes = new List<string>() { scope };
            var thisAccessToken = GetAndCacheAccessToken(scopes);
            var httpRequest = (HttpWebRequest)WebRequest.Create(docPath);
            httpRequest.Headers.Add("client_id", this.ClientIdentifier);
            httpRequest.Timeout = 360000; //6 mins. a bit long really, but needed for now
            AuthorizeRequest(httpRequest, thisAccessToken);
            WebResponse response = null;
            try
            {
                response = httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {

                    if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound) //return null for 404s
                    {
                        return string.Empty;
                    }
                }

                throw;
            }

            var result = DeserializeJsonResult<string>(response.GetResponseStream());
            return result;
        }

        private T DeserializeJsonResult<T>(Stream responseStream) where T : class
        {
            if (responseStream == null) return null;

            T result = null;

            var storeStream = new StreamReader(responseStream);
            using (storeStream)
            {
                var str = storeStream.ReadToEnd();

                if (typeof(T) == typeof(string))
                    return str as T;

                result = JsonConvert.DeserializeObject<T>(str);
            }

            return result;
        }

        private string GetAndCacheAccessToken(List<string> scopes)
        {
            IAuthorizationState authorizationState;
            try
            {
                authorizationState = this.GetClientAccessToken(scopes); //get an access token
            }
            catch (WebException ex)
            {
                var errorResponse = (HttpWebResponse)ex.Response;
//                LogError(ex);
                throw;
            }
            catch (DotNetOpenAuth.Messaging.ProtocolException ex)
            {
                throw;
//                LogError(ex);
            }

            var thisAccessToken = authorizationState.AccessToken;
            //CachedAccessToken = thisAccessToken;
            return thisAccessToken;
        }

        public static bool TrustAllCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //If it is really important, validate the certificate issuer here.
            //string resultsTrue = certificate.Issuer.ToString();
            //For now, accept any certificate
            return true;
        }

    }
}
