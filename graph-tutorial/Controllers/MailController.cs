using graph_tutorial.Attributes;
using graph_tutorial.Helpers;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    [AzureAuthenticate]
    public class MailController : BaseController
    {
        // GET: Mail
        public async Task<ActionResult> Index()
        {
            var model = await GraphHelper.GetMessages();
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Message message)
        {
            //await GraphHelper.SendMail(subject, body, address);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Details(string id)
        {
            var model = await GraphHelper.GetMessage(id);
            return View(model);
        }
    }
}