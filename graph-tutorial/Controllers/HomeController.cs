﻿using graph_tutorial.Attributes;
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
    [AzureAuthenticate]
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
        public async Task<ActionResult> Create()
        {
            var folk = await GraphHelper.ListUser();
            var list = folk.Select(_ => new SelectListItem() { Text = _.DisplayName, Value = _.Id}).ToList();
            ViewBag.Users = list;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Booking b)
        {
            await GraphHelper.PostBooking(b);

            return RedirectToAction("Test");
        }


        //public async Task<ActionResult> GetUser()
        //{
        //    var folk = await GraphHelper.ListUser();
        //    ViewBag.Users = folk;

        //    return View("Test"); 
        //}





    }
}