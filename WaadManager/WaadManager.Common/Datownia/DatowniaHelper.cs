using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaadManager.Common.Models;

namespace WaadManager.Common.Datownia
{
    public class DatowniaHelper : IDatowniaHelper
    {
        private DatowniaClient client = new DatowniaClient(WaadConfig.AppKey, WaadConfig.AppSecret);
        private int ReadLimit = 200;

        public IEnumerable<DtDelta> GetDeltas(string documentName, int currentDelta)
        {
            var offset = 0;
            var hasMore = true;
            var result = new List<DtDelta>();

            do
            {
                var docString = client.GetDelta(documentName, offset, ReadLimit, currentDelta);
                var jarray = JsonConvert.DeserializeObject<JArray>(docString);

	            if (jarray == null)
	            {
					hasMore = false;
					continue;
	            }

                if (jarray.Count == 1)
                {
					if (jarray[0].Type != JTokenType.Object)
					{
						hasMore = false;
						continue;
					}
                }

                        
                foreach (var delta in jarray)
                {
                    var datowniaDelta = new DtDelta().FromContentRow(delta);
                    result.Add(datowniaDelta);
                }

                if (jarray.Count < ReadLimit)
                    hasMore = false;
                else
                    offset += ReadLimit;

            } while (hasMore);

            return result;
        }

        public IEnumerable<DtUser> GetAllUsers(out int sequence)
        {
            var offset = 0;
            var hasMore = true;
            var result = new List<DtUser>();

            do
            {
                var jarray = new JArray(client.GetDocument(WaadConfig.UserApiFullName, offset, ReadLimit));
                var arr = JObject.Parse(jarray[0].Value<string>());
                var contents = arr["contents"].Value<JArray>();
                sequence = arr["seq"].Value<int>();

                foreach (var row in contents)
                {
                    var datarow = new DtUser().FromContentRow(row);
                    result.Add(datarow);
                }

                if (contents.Count < ReadLimit)
                    hasMore = false;
                else
                    offset += ReadLimit;

            } while (hasMore);

            return result;
        }

        public IEnumerable<DtEvent> GetAllEvents(out int sequence)
        {
            var offset = 0;
            var hasMore = true;
            var result = new List<DtEvent>();

            do
            {
                var jarray = new JArray(client.GetDocument(WaadConfig.ConfScheduleApiFullName, offset, ReadLimit));
                var arr = JObject.Parse(jarray[0].Value<string>());
                var contents = arr["contents"].Value<JArray>();
                sequence = arr["seq"].Value<int>();

                foreach (var row in contents)
                {
                    var datarow = new DtEvent().FromContentRow(row);
                    result.Add(datarow);
                }

                if (contents.Count < ReadLimit)
                    hasMore = false;
                else
                    offset += ReadLimit;

            } while (hasMore);

            return result;
        }

        public IEnumerable<DtGroup> GetAllGroups(out int sequence)
        {
            var offset = 0;
            var hasMore = true;
            var result = new List<DtGroup>();

            do
            {
                var jarray = new JArray(client.GetDocument(WaadConfig.GroupApiFullName, offset, ReadLimit));
                var arr = JObject.Parse(jarray[0].Value<string>());
                var contents = arr["contents"].Value<JArray>();
                sequence = arr["seq"].Value<int>();

                foreach (var row in contents)
                {
                    var datarow = new DtGroup().FromContentRow(row);
                    result.Add(datarow);
                }

                if (contents.Count < ReadLimit)
                    hasMore = false;
                else
                    offset += ReadLimit;

            } while (hasMore);

            return result;
        }

        public IEnumerable<DtUserGroup> GetAllUserGroups(out int sequence)
        {
            var offset = 0;
            var hasMore = true;
            var result = new List<DtUserGroup>();

            do
            {
                var jarray = new JArray(client.GetDocument(WaadConfig.UserGroupApiFullName, offset, ReadLimit));
                var arr = JObject.Parse(jarray[0].Value<string>());
                var contents = arr["contents"].Value<JArray>();
                sequence = arr["seq"].Value<int>();
                
                foreach (var row in contents)
                {
                    var datarow = new DtUserGroup().FromContentRow(row);
                    result.Add(datarow);
                }

                if (contents.Count < ReadLimit)
                    hasMore = false;
                else
                    offset += ReadLimit;

            } while (hasMore);

            return result;
        }
    }
}
