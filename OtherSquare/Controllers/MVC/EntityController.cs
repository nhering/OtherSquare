using OtherSquare.Models;
using OtherSquare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OtherSquare.Controllers
{
    public class EntityController : Controller
    {
        // GET: Entity/Search
        public ActionResult Search()
        {
            return View("EntitySearch", new EntitySearchViewModel());
        }

        // POST: Entity/Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string searchString)
        {
            return PartialView("_EntitySearchResults", new EntitySearchViewModel(searchString));
        }

        // GET: Entity/New
        [HttpGet]
        public ActionResult New()
        {
            return View("EntityDetails", new EntityViewModel());
        }

        // GET: Entity/Detail
        [HttpGet]
        public ActionResult Detail(Guid entityGuid)
        {
            return View("EntityDetails", new EntityViewModel(entityGuid));
        }

        // POST: Entity/Save
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Save(EntityViewModel model)
        {
            if (model.IsValid())
            {
                model.Update();
                if (model.IsDeleted)
                {
                    return RedirectToAction("Search");
                }
                else
                {
                    return View("EntityDetails", new EntityViewModel(model.Entity.Guid));
                }
            }
            else
            {
                //TODO return an error message
                //For now, just go back to the search page
                return RedirectToAction("Search");
            }
        }

        // GET: Entity/NewProperty
        [HttpGet]
        public ActionResult NewProperty()
        {
            return PartialView("_EntityProperty", new EntityProperty());
        }
    }
}