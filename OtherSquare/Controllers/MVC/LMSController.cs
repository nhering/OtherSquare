using Microsoft.AspNet.Identity;
using OtherSquare.Models;
using OtherSquare.ViewModels;
using OtherSquare.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace OtherSquare.Controllers.MVC
{
    public class LMSController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            UserSetting userSetting = new UserSetting(userId, "/LMS");
            NAV_FlashCardViewModel model = new NAV_FlashCardViewModel(userSetting);
            return View("LMSFlashcards", model);
        }

        #region LMS/Flashcards

        [HttpGet]
        [Route("/LMS/GetSubjectPartial")]
        public ActionResult GetSubjectPartial(UserSetting model)
        {
            model.UserId = User.Identity.GetUserId();
            model.SaveSettings();
            NAV_FlashCardViewModel viewModel = NAV_FlashCardViewModel.GetSubjectViewModel(model);
            return PartialView("_LMSFlashcards_Subjects", viewModel);
        }

        [HttpGet]
        [Route("/LMS/GetCategoryPartial")]
        public ActionResult GetCategoryPartial(UserSetting model)
        {
            model.UserId = User.Identity.GetUserId();
            model.SaveSettings();
            NAV_FlashCardViewModel viewModel = NAV_FlashCardViewModel.GetCategoryViewModel(model);
            return PartialView("_LMSFlashcards_Categories", viewModel);
        }

        [HttpGet]
        [Route("/LMS/GetFlashcardPartial")]
        public ActionResult GetFlashcardPartial(UserSetting model)
        {
            model.UserId = User.Identity.GetUserId();
            model.SaveSettings();
            NAV_FlashCardViewModel viewModel = NAV_FlashCardViewModel.GetFlashCardViewModel(model);
            return PartialView("_LMSFlashcards_Flashcards", viewModel);
        }

        #endregion
    }
}