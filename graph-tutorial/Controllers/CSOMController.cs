using graph_tutorial.Attributes;
using graph_tutorial.Helpers;
using graph_tutorial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace graph_tutorial.Controllers
{
    [AzureAuthenticate("https://localhost:44397/CSOM")]
    public class CSOMController : BaseController
    {
        // GET: CSOM
        public ActionResult Index()
        {
            var lists = SharepointHelper.GetLists();
            ViewBag.Lists = lists.Select(_ => _.Title);
            return View();
        }

        public ActionResult ListDetails(string listName)
        {
            var list = SharepointHelper.GetList(listName);
            var model = new ListDetailsViewModel();
            model.Title = listName;
            model.Items = list.Select(_ => _["Title"].ToString());
            return View(model);
        }

        [HttpGet]
        public ActionResult CreateList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateList(CreateListViewModel model)
        {
            SharepointHelper.CreateList(model.Title);
            return RedirectToAction("Index");
        }

        public ActionResult CreateListItem(string listName)
        {
            var model = new CreateListItemViewModel();
            model.ListTitle = listName;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateListItem(CreateListItemViewModel model)
        {
            SharepointHelper.AddItemToList(model.ListTitle, model.Title);
            return RedirectToAction("ListDetails", new { listName = model.ListTitle });
        }
    }
}