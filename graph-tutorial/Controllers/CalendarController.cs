using graph_tutorial.Attributes;
using graph_tutorial.Helpers;
using graph_tutorial.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

namespace graph_tutorial.Controllers
{
    [AzureAuthenticate("https://localhost:44397/Calendar/")]
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
            model.MonthName = now.ToString("MMMM");
            return View(model);
        }

        public async Task<ActionResult> Details(string d)
        {
            var model = new CalendarDetailsViewModel();
            var date = DateTime.Parse(d);
            var events = await GraphHelper.GetEventsByDay(date);
            model.Events = events.Select(_ =>
            {
                var m = new EventViewModel();
                m.Subject = _.Subject;
                m.Organizer = _.Organizer.EmailAddress.Name;
                m.Attendees = _.Attendees;
                m.Start = DateTime.Parse(_.Start.DateTime).ToShortTimeString();
                m.End = DateTime.Parse(_.End.DateTime).ToShortTimeString();
                return m;
            });
            
            return View(model);
        }

        public async Task<ActionResult> VisitDelve(string mail)
        {
            var user = await GraphHelper.GetUserByMail(mail);
            var url = $"https://eur.delve.office.com/?u={user.Id}&v=work";
            return Redirect(url);
        }
    }
}