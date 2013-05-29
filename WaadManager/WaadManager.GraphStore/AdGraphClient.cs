using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaadManager.GraphStore.Models;

namespace WaadManager.GraphStore
{
    public class AdGraphClient
    {
        private AADJWTToken _token;
        private string TenantGraphRoot { get; set; }
        public static string TenantGraph = "waaddemo";

        public AdGraphClient()
        {
            _token = GetToken();
            TenantGraphRoot = string.Format("https://graphstore.windows.net/{0}/", ConfigurationManager.AppSettings["TenantDomainName"]);
        }

        private AADJWTToken GetToken()
        {
            if (_token == null || _token.IsExpired)
                NewToken();

            return _token;
        }

        private void NewToken()
        {
            _token = DirectoryDataServiceAuthorizationHelper.GetAuthorizationToken(ConfigurationManager.AppSettings["TenantDomainName"],
                ConfigurationManager.AppSettings["AppPrincipalId"], ConfigurationManager.AppSettings["Password"]);
        }

        private WebRequest CreateGPPostRequest(string graphName)
        {
            var req = WebRequest.Create(string.Format("{0}{1}", TenantGraphRoot, graphName));
            //var token = GetToken();
            //string authzHeader = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", token.TokenType, " ", token.AccessToken);
            //req.Headers.Add("Authorization", authzHeader);
            req.Method = "POST";
            req.ContentType = "application/json";
            return req;
        }

        private WebRequest CreateGPPutRequest(string graphName, string item1, string item2)
        {
            var req = WebRequest.Create(string.Format("{0}{1}/{2}/{3}", TenantGraphRoot, graphName, item1, item2));
            //var token = GetToken();
            //string authzHeader = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", token.TokenType, " ", token.AccessToken);
            //req.Headers.Add("Authorization", authzHeader);
            req.Method = "PUT";
            req.ContentType = "application/json";
            return req;
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

        public void PostProgramme(Programme pg)
        {
            PostToGPGraph(pg, TenantGraph);
        }

        public void PutProgramme(Programme entireProgramme)
        {
            //split into days
            var pg14 = new Programme(14);
            var pg15 = new Programme(15);
            var pg16 = new Programme(16);
            var pg17 = new Programme(17);

            SortIntoDays(entireProgramme, ref pg14, ref pg15, ref pg16, ref pg17);

            PutToGPGraph(pg14, TenantGraph, pg14.Item1, pg14.Item2);
            PutToGPGraph(pg15, TenantGraph, pg15.Item1, pg15.Item2);
            PutToGPGraph(pg16, TenantGraph, pg16.Item1, pg16.Item2);
            PutToGPGraph(pg17, TenantGraph, pg17.Item1, pg17.Item2);
        }

        private bool SortIntoDays(Programme programme,
                                  ref Programme prog14,
                                  ref Programme prog15,
                                  ref Programme prog16,
                                  ref Programme prog17)
        {
            var events14 = new List<Event>();
            var events15 = new List<Event>();
            var events16 = new List<Event>();
            var events17 = new List<Event>();

            foreach (var _event in programme.Values)
            {
                switch (_event.Day.Substring(0, 2))
                {
                    case "14":
                        events14.Add(_event);
                        break;
                    case "15":
                        events15.Add(_event);
                        break;
                    case "16":
                        events16.Add(_event);
                        break;
                    case "17":
                        events17.Add(_event);
                        break;
                }
            }

            var totalEventCount = events14.Count + events15.Count + events16.Count() + events17.Count();
            prog14.Values = events14;
            prog15.Values = events15;
            prog16.Values = events16;
            prog17.Values = events17;

            return programme.Values.Count() == totalEventCount;
        }

        public void CreateFavourite(Favourite fav)
        {
            PostToGPGraph(fav, TenantGraph);
        }
        
        //bit of a hack to cater for splitting schedule into four tuples...
        public Programme GetConfSchedule()
        {
            var sched14 = GetConfSchedule(14);
            var sched15 = GetConfSchedule(15);
            var sched16 = GetConfSchedule(16);
            var sched17 = GetConfSchedule(17);
            var values = new List<Event>();
            values.AddRange(sched14.Values ?? new List<Event>());
            values.AddRange(sched15.Values ?? new List<Event>());
            values.AddRange(sched16.Values ?? new List<Event>());
            values.AddRange(sched17.Values ?? new List<Event>());
            var sched = new Programme(0);
            sched.Values = values;
            return sched;
        }

        public Programme GetConfSchedule(int date)
        {
            var prog = new Programme(date);
            var url = string.Format("{0}{1}/{2}/{3}", TenantGraphRoot, TenantGraph, prog.Item1, prog.Item2);
            var req = WebRequest.Create(url);
            var resp = req.GetResponse().GetResponseStream();
            var result = DeserializeJsonResult<Programme>(resp);
            result.Date = date;
            return result;
        }

        public void DeleteGPTuple(string item1, string item2)
        {
            var req = WebRequest.Create(string.Format("{0}{1}/{2}/{3}", TenantGraphRoot, TenantGraph, item1, item2));
            req.Method = "DELETE";
            req.ContentType = "application/json";
            req.GetResponse();
        }

        /// <summary>
        /// Both arguments must be supplied
        /// </summary>
        /// <param name="upn"></param>
        /// <param name="eventCode"></param>
        /// <returns></returns>
        public Favourite GetFavourite(string upn, string eventCode)
        {
            if (string.IsNullOrWhiteSpace(upn) || string.IsNullOrWhiteSpace(eventCode))
                return null;

            var url = string.Format("{0}{1}/{2}/{3}", TenantGraphRoot, TenantGraph, upn, eventCode);
            var req = WebRequest.Create(url);
            var resp = req.GetResponse().GetResponseStream();
            var result = DeserializeJsonResult<Favourite>(resp);
            return result;
        }

        /// <summary>
        /// Providing a wildcard (*) or empty argument in either field will return a directed query.
        /// At least one argument must be supplied.
        /// </summary>
        /// <param name="upn"></param>
        /// <param name="eventCode"></param>
        /// <returns></returns>
        public FavouriteList GetFavourites(string upn, string eventCode)
        {
            if (string.IsNullOrWhiteSpace(upn) && string.IsNullOrWhiteSpace(eventCode))
                return null;

            upn = string.IsNullOrWhiteSpace(upn) ? "*" : upn;
            eventCode= string.IsNullOrWhiteSpace(eventCode) ? "*" : eventCode;

            var url = string.Format("{0}{1}/{2}/{3}", TenantGraphRoot, TenantGraph, upn, eventCode);
            var req = WebRequest.Create(url);
            var resp = req.GetResponse().GetResponseStream();
            var result = DeserializeJsonResult<FavouriteList>(resp);
            return result;
        }
        
        public string PostToGPGraph(object obj, string graphName)
        {
            var req = CreateGPPostRequest(graphName);
            var content = JsonConvert.SerializeObject(obj);
            var byteArray = Encoding.UTF8.GetBytes(content);

            using (var stream = req.GetRequestStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
            }

            return DeserializeJsonResult<string>(req.GetResponse().GetResponseStream());
        }

        public string PutToGPGraph(object obj, string graphName, string item1, string item2)
        {
            var req = CreateGPPutRequest(graphName, item1, item2);
            var content = JsonConvert.SerializeObject(obj);
            var byteArray = Encoding.UTF8.GetBytes(content);

            using (var stream = req.GetRequestStream())
            {
                stream.Write(byteArray, 0, byteArray.Length);
            }

            return DeserializeJsonResult<string>(req.GetResponse().GetResponseStream());
        }
    }
}
