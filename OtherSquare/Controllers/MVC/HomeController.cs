using Microsoft.AspNet.Identity;
using OtherSquare.Classes;
using OtherSquare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OtherSquare.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            //Before the user is logged in, the userId will be null.
            if (userId == null) return View(); 

            //If the user is logged in, check to see if they have ever visited a page.
            UserSetting setting = new UserSetting(userId);
            string lastUrl = setting.GetLastRedirectURL();

            //If they this is the first time they have logged in, the URL will be null. Use a default of the home page.
            if (string.IsNullOrEmpty(lastUrl)) return View();

            //Otherwise, redirect them to the last page they visited.
            return Redirect(lastUrl);
        }
    }
}