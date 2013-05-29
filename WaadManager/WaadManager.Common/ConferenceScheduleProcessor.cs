using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaadManager.Common.Datownia;
using WaadManager.Common.Sql;
using WaadManager.Common.models;
using WaadManager.GraphStore;
using WaadManager.GraphStore.Models;

namespace WaadManager.Common
{
    public class ConferenceScheduleProcessor
    {
        private IDatowniaHelper datowniaService;
        private ISqlHelper sqlService;
        private AdGraphClient client;

        public ConferenceScheduleProcessor()
        {
            datowniaService = new DatowniaHelper();
            sqlService = new SqlHelper();
            client = new AdGraphClient();
        }

        public ConferenceScheduleProcessor(IDatowniaHelper datowniaHelper, ISqlHelper sqlHelper)
        {
            datowniaService = datowniaHelper;
            sqlService = sqlHelper;
        }

        public void RunUpdate()
        {
            if (!Initalize())
                return;

            //get schedule data from tenant graph
            var programme = client.GetConfSchedule();

            if (!programme.Values.Any())
            {
                //there is no event data in graph
                
                //get the latest program data from datownia
                int seq = 0;
                var data = datowniaService.GetAllEvents(out seq);
                
                if (!data.Any())
                    return;

                //update the programme in the graph
                var eventList = new List<Event>();
                foreach (var dtEvent in data)
                {
                    var newEvent = new Event();
                    newEvent.DatowniaId = dtEvent.Id;
                    newEvent.Location = dtEvent.Location;
                    newEvent.Time = dtEvent.Time;
                    newEvent.Speakers = dtEvent.Speakers;
                    newEvent.Title = dtEvent.Title;
                    newEvent.Code = dtEvent.Code;
                    newEvent.Area = dtEvent.Area;
                    newEvent.Day = dtEvent.Day;
                    eventList.Add(newEvent);
                }

                programme.Values = eventList;
                //need to split into four separate programmes
                client.PutProgramme(programme);
                sqlService.SetLatestLocalSeq(WaadConfig.ConfScheduleApiFullName, seq);
            }
            else
            {
                //get sequence from sql
                var latestScheduleSeq = sqlService.GetLatestLocalSeq(WaadConfig.ConfScheduleApiFullName);

                //get data from datownia
                var eventDeltas = datowniaService.GetDeltas(WaadConfig.ConfScheduleApiFullName, latestScheduleSeq);

                if (!eventDeltas.Any())
                    return;

                var eventList = programme.Values.ToList();

                //apply deltas in sequence to local object
                foreach (var eventDelta in eventDeltas)
                {
                    if (eventDelta.IsDelete)
                    {
                        var valToDelete = eventList.Where(e => e.DatowniaId == eventDelta.DataForDelete).FirstOrDefault();
                        if (valToDelete != null)
                            eventList.Remove(valToDelete);
                    }
                    else
                    {
                        var dtEvent = eventDelta.DataAsEvent;
                        var newEvent = new Event();
                        newEvent.DatowniaId = dtEvent.Id;
                        newEvent.Location = dtEvent.Location;
                        newEvent.Time = dtEvent.Time;
                        newEvent.Speakers = dtEvent.Speakers;
                        newEvent.Title = dtEvent.Title;
                        newEvent.Code = dtEvent.Code;
                        newEvent.Area = dtEvent.Area;
                        newEvent.Day = dtEvent.Day;
                        eventList.Add(newEvent);
                    }
                }

                //update tenant graph with programme object
                programme.Values = eventList;

                client.PutProgramme(programme);

                var latestScheduleSeqFromApi = eventDeltas.OrderByDescending(ud => ud.Seq).First().Seq;
                if (latestScheduleSeqFromApi > latestScheduleSeq)
                    sqlService.SetLatestLocalSeq(WaadConfig.ConfScheduleApiFullName, latestScheduleSeqFromApi);
            }
        }

        private bool Initalize()
        {
            try
            {
                sqlService.TryConnectDb();

                sqlService.EnsureConfScheduleDeltaRecordExists();
            }
            catch (Exception ex)
            {
                //TODO: Log Exception
                return false;
            }

            return true;
        }
    }
}
