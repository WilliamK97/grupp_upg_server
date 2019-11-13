using graph_tutorial.Attributes;
using graph_tutorial.Helpers;
using graph_tutorial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    [AzureAuthenticate("https://localhost:44397")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Error(string message, string debug)
        {
            Flash(message, debug);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Test()
        {
            var axel = await GraphHelper.GetBookings();
            var a = await GraphHelper.GetMe();
            var photo = await GraphHelper.GetMyPhoto();
            
            ViewBag.Bookings = axel;
            
            ViewBag.Mail = a.Mail;
            ViewBag.Name = a.DisplayName;
            ViewBag.Photo = photo;
            

            var folk = await GraphHelper.ListUser();
            ViewBag.Users = folk;        

            return View();
        }

        [HttpGet]
        
        public async Task<ActionResult> Create(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var booking = await GraphHelper.GetBooking(id);
                return View(booking);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Booking b)
        {
            if (string.IsNullOrEmpty(b.Id))
            {
                await GraphHelper.PostBooking(b);
            }
            else
            {
                await GraphHelper.UpdateBooking(b);
            }
            
            return RedirectToAction("Test");
        }
        

        public async Task<ActionResult> Delete(string id)
        {
            await GraphHelper.DeleteBooking(id);

            return RedirectToAction("Test");
        }

    }
}