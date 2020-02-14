using System.Web.Http;
using Microsoft.AspNet.Identity;
using OtherSquare.Models;
using System.Web.Mvc;

namespace OtherSquare.Controllers.API
{
    public class SettingsController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public bool Save(UserSetting model)
        {
            model.UserId = User.Identity.GetUserId();
            return model.SaveSettings();
        }
    }
}