using graph_tutorial.Helpers;
using graph_tutorial.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    public class CalendarController : BaseController
    {
        // GET: Calendar
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var events = await GraphHelper.GetEventsAsync();

            // Change start and end dates from UTC to local time
            foreach (var ev in events)
            {
                ev.Start.DateTime = DateTime.Parse(ev.Start.DateTime).ToLocalTime().ToString();
                ev.Start.TimeZone = TimeZoneInfo.Local.Id;
                ev.End.DateTime = DateTime.Parse(ev.End.DateTime).ToLocalTime().ToString();
                ev.End.TimeZone = TimeZoneInfo.Local.Id;
            }

            return View(events);
        }

        public ActionResult Grid()
        {
            var model = new CalendarGridViewModel();
            var now = DateTime.Now;
            double days = DateTime.DaysInMonth(now.Year, now.Month);
            double cols = 6;
            model.Rows = (int)Math.Ceiling(days / cols);
            model.Days = (int)days;
            model.Cols = (int)cols;
            model.Month = now.Month;
            model.Year = now.Year;
            return View(model);
        }

        public async Task<ActionResult> Details(string d)
        {
            var date = DateTime.Parse(d);
            await GraphHelper.GetEventsByDay(date);
            ViewBag.Test = d;
            return View();
        }
    }
}