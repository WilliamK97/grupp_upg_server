using graph_tutorial.Helpers;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    public class UsersController : Controller
    {
        

        public ActionResult Index()
        {
            return View();
        }
    }
}