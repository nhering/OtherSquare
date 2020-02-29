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
        // The default for a new user when they enter the LMS tab is 
        // to direct them to the Flashcards sub tab
        [HttpGet]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            UserSetting userSetting = new UserSetting(userId, "/LMS");
            LMS_Flashcards model = new LMS_Flashcards(userSetting);
            return View("LMS_Flashcards", model);
        }
    }
}