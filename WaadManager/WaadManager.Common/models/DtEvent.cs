using Newtonsoft.Json.Linq;

namespace WaadManager.Common.Models
{
    public class DtEvent : DatowniaContent<DtEvent>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Speakers { get; set; }
        public string Time { get; set; }
        public string Area { get; set; }
        public string Day { get; set; }

        public override DtEvent FromContentRow(JToken row)
        {
            Id = row[WaadConfig.EventIdFieldIndex].Value<string>();
            Code = row[WaadConfig.EventCodeFieldIndex].Value<string>();
            Location = row[WaadConfig.EventLocationFieldIndex].Value<string>();
            Speakers = row[WaadConfig.EventSpeakerFieldIndex].Value<string>();
            Title = row[WaadConfig.EventTitleFieldIndex].Value<string>();
            Time = row[WaadConfig.EventTimeFieldIndex].Value<string>();
            Area = row[WaadConfig.EventAreaFieldIndex].Value<string>();
            Day = row[WaadConfig.EventDayFieldIndex].Value<string>();
            return this;
        }
    }
}
