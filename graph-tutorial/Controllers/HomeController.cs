using graph_tutorial.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
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
            var a = await GraphHelper.GetMe();
            var photo = await GraphHelper.GetMyPhoto();
            ViewBag.Mail = a.Mail;
            ViewBag.Name = a.DisplayName;
            ViewBag.Photo = photo;

            return View();
        }


    }
}