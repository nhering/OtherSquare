using OtherSquare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OtherSquare.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace OtherSquare.Controllers.API
{
    public class LMSController : ApiController
    {
        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/SelectItem")]
        public void SelectItem(ItemDTO dto)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    if(dto.modelType == "subject")
                    {
                        Subject item = db.Subjects.FirstOrDefault(o => o.SubjectGuid == dto.guid);
                        if (item == null) return;
                        item.IsSelected = !item.IsSelected;
                    }
                    if (dto.modelType == "category")
                    {
                        Category item = db.Categories.FirstOrDefault(o => o.CategoryGuid == dto.guid);
                        if (item == null) return;
                        item.IsSelected = !item.IsSelected;
                    }
                    if (dto.modelType == "flashcard")
                    {
                        FlashCard item = db.FlashCards.FirstOrDefault(o => o.FlashCardGuid == dto.guid);
                        if (item == null) return;
                        item.IsSelected = !item.IsSelected;
                    }
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
        [Route("api/LMS/ArchiveItems")]
        public void ArchiveItems(ItemsDTO dto)
        {
            try
            {
                if (dto.guids == null) return;
                if (dto.modelType == "subject")
                {
                    SubjectViewModel.ArchiveSubjects(dto.guids);
                }
                if (dto.modelType == "category")
                {
                    CategoryViewModel.ArchiveCategories(dto.guids);
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/UnArchiveItems")]
        public void UnArchiveItems(ItemsDTO dto)
        {
            try
            {
                if (dto.guids == null) return;
                if (dto.modelType == "subject")
                {
                    SubjectViewModel.UnArchiveSubjects(dto.guids);
                }
                if (dto.modelType == "category")
                {
                    CategoryViewModel.UnArchiveCategories(dto.guids);
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        [System.Web.Mvc.HttpPost]
        [Route("api/LMS/RemoveFlashcards")]
        public void RemoveFlashcards(ItemsDTO dto)
        {
            try
            {
                if (dto.guids == null) return;
                FlashcardViewModel.DeleteFlashcards(dto.guids);
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
        public string SaveFlashcard(UserSetting model)
        {
            model.UserId = User.Identity.GetUserId();
            InputValidation val = FlashcardViewModel.SaveFlashcard(model);
            return val.ToJson();
        }
    }

    public class ItemDTO
    {
        public string modelType { get; set; }
        public Guid guid { get; set; }
    }

    public class ItemsDTO
    {
        public string modelType { get; set; }
        public List<Guid> guids { get; set; }

    }
}