using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaadManager.GraphStore;
using WaadManager.GraphStore.Models;

namespace WaadManager.Web.Controllers
{
    public class ProgrammeController : AbstractBaseController
    {
        AdGraphClient client = new AdGraphClient();

        public ActionResult ViewSchedule()
        {
            var sched = client.GetConfSchedule();
            var faves = client.GetFavourites(GetCurrentUpn(), string.Empty);
            sched.UpdateFavourites(faves.Values);

            //split into days for UI
            ViewBag.events14 = sched.Values.Where(e => e.Day.Substring(0, 2) == "14");
            ViewBag.events15 = sched.Values.Where(e => e.Day.Substring(0, 2) == "15");
            ViewBag.events16 = sched.Values.Where(e => e.Day.Substring(0, 2) == "16");
            ViewBag.events17 = sched.Values.Where(e => e.Day.Substring(0, 2) == "17");

            ViewBag.Schedule = sched;
            ViewBag.IsSpeaker = IsSpeaker();
            return View();
        }

        public ActionResult ViewFavourites()
        {
            var sched = client.GetConfSchedule();
            var faves = client.GetFavourites(GetCurrentUpn(), string.Empty);
            sched.UpdateFavourites(faves.Values);
            sched.Values = sched.Values.Where(e => e.IsFave);

            ViewBag.events14 = sched.Values.Where(e => e.Day.Substring(0, 2) == "14");
            ViewBag.events15 = sched.Values.Where(e => e.Day.Substring(0, 2) == "15");
            ViewBag.events16 = sched.Values.Where(e => e.Day.Substring(0, 2) == "16");
            ViewBag.events17 = sched.Values.Where(e => e.Day.Substring(0, 2) == "17");

            ViewBag.Schedule = sched;
            ViewBag.IsSpeaker = IsSpeaker();
            return View();
        }

        public ActionResult ViewMyEngagements()
        {
            var sched = client.GetConfSchedule();
            var username = GetCurrentEmailAddress();
            sched.Values = sched.Values.Where(s => !string.IsNullOrWhiteSpace(s.Speakers) && s.Speakers.Contains(username));
            
            ViewBag.events14 = sched.Values.Where(e => e.Day.Substring(0, 2) == "14");
            ViewBag.events15 = sched.Values.Where(e => e.Day.Substring(0, 2) == "15");
            ViewBag.events16 = sched.Values.Where(e => e.Day.Substring(0, 2) == "16");
            ViewBag.events17 = sched.Values.Where(e => e.Day.Substring(0, 2) == "17");
            
            ViewBag.Schedule = sched;
            ViewBag.IsSpeaker = IsSpeaker();
            return View();
        }

        public ActionResult AddAsFavourite(string eventCode)
        {
            var fav = new Favourite();
            fav.Upn = GetCurrentUpn();
            fav.EventCode = eventCode;
            client.CreateFavourite(fav);
            return new EmptyResult();
        }

        public ActionResult RemoveFavourite(string eventCode)
        {
            client.DeleteGPTuple(GetCurrentUpn(), eventCode);
            return new EmptyResult();
        }
    }
}
