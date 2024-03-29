﻿using graph_tutorial.Attributes;
using graph_tutorial.Helpers;
using graph_tutorial.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    [AzureAuthenticate("https://localhost:44397")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Calendar");
        }

        public ActionResult About()
        {
            GraphHelper.TestRestAsync();
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
            var booking = new Booking();
            if (!string.IsNullOrEmpty(id))
            {
                 booking = await GraphHelper.GetBooking(id);
                
            }
            var folk = await GraphHelper.ListUser();
            var users = folk.Select(_ => new SelectListItem()
            {
                Text = _.DisplayName,
                Value = _.DisplayName
            });

            ViewBag.Users = users;
            return View(booking);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Booking b)
        {
            await GraphHelper.PostBooking(b);     
            return RedirectToAction("Test");
        }
        

        public async Task<ActionResult> Delete(string id)
        {
            await GraphHelper.DeleteBooking(id);

            return RedirectToAction("Test");
        }

    }
}