using graph_tutorial.Attributes;
using graph_tutorial.Helpers;
using graph_tutorial.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    [AzureAuthenticate("https://localhost:44397/Mail")]
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
        public async Task<ActionResult> Create(MessageViewModel model)
        {
            var message = new Message
            {
                Subject = model.Subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = model.Content
                },
                ToRecipients = new List<Recipient>()
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = model.Reciepient
                        }
                    }
                },
            };
            await GraphHelper.SendMail(message);
            return RedirectToAction("Index","Mail");
        }

        public async Task<ActionResult> Details(string id)
        {
            var model = await GraphHelper.GetMessage(id);
            return View(model);
        }

        public async Task<ActionResult> Delete(string id)
        {
            await GraphHelper.DeleteMessage(id);
            return RedirectToAction("Index","Mail");
        }
    }
}