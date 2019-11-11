using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    public class SPController : Controller
    {
        // GET: SP
        public ActionResult Index()
        {
            using (var sp = new SharepointContext())
            {
                var booking = sp.bookings.OrderBy(c => c.Title);
                return View(booking);
            }
        }

    }
}