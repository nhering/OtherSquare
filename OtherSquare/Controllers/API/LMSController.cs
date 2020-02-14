using OtherSquare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OtherSquare.ViewModels;

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
        public string SaveSubject(Subject subject)
        {
            InputValidation val = SubjectViewModel.SaveSubject(subject);
            return val.ToJson();
        }

        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/SaveCategory")]
        public string SaveCategory(Category category)
        {
            InputValidation val = CategoryViewModel.SaveCategory(category);
            return val.ToJson();
        }

        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/SaveFlashcard")]
        public void SaveFlashcard(FlashCard flashcard)
        {
            flashcard.Save();
        }
    }
}