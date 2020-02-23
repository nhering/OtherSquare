using OtherSquare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OtherSquare.ViewModels;
using Microsoft.AspNet.Identity;

namespace OtherSquare.Controllers.API
{
    public class LMSController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        public void SelectSubject(Guid subjectGuid)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Subject subject = db.Subjects.FirstOrDefault(s => s.SubjectGuid == subjectGuid);
                    if (subject == null) return;
                    subject.IsSelected = !subject.IsSelected;
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/SaveSubject")]
        public string SaveSubject(UserSetting model)
        {
            model.UserId = User.Identity.GetUserId();
            InputValidation val = SubjectViewModel.SaveSubject(model);
            return val.ToJson();
        }

        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/SaveCategory")]
        public string SaveCategory(UserSetting model)
        {
            model.UserId = User.Identity.GetUserId();
            InputValidation val =CategoryViewModel.SaveCategory(model);
            return val.ToJson();
        }

        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/SaveFlashcard")]
        public string SaveFlashcard(UserSetting model) //Change the argument passed in to be a UserSettings object
        {
            model.UserId = User.Identity.GetUserId();
            InputValidation val = FlashcardViewModel.SaveFlashcard(model);
            return val.ToJson();
        }
    }
}