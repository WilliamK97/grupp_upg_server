using graph_tutorial.Attributes;
using graph_tutorial.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
  
    public class MailController : Controller
    {
        // GET: Mail
        public async Task<ActionResult> Index()
        {
            await GraphHelper.GetMail();
            return View();
        }

       
    }
}