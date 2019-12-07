using OtherSquare.Models;
using OtherSquare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OtherSquare.Controllers.MVC
{
    public class StudyController : Controller
    {
        //GET
        public ActionResult Index()
        {
            return View("StudyIndex", new FlashCard_Index_ViewModel());
        }

        [HttpGet]
        public ActionResult SCF_List(string type)
        {
            return PartialView("_SCF_List", new SCF_List(type));
        }

        #region Subject
        
        [HttpGet]
        public ActionResult Subject_Create()
        {
            return PartialView("_Subject", new SubjectViewModel());
        }

        [HttpGet]
        public ActionResult Subject_Read(Guid guid)
        {
            SubjectViewModel model = new SubjectViewModel(guid);
            return PartialView("_Subject", model);
        }

        [HttpGet]
        public ActionResult Subject_RemoveCategory(Guid subjectGuid, Guid categoryGuid)
        {
            SubjectViewModel model = new SubjectViewModel(subjectGuid);
            model.RemoveCategory(subjectGuid, categoryGuid);
            return PartialView("_Subject", model);
        }

        #endregion
    }
}