using Newtonsoft.Json;
using OtherSquare.Models;
using System;
using System.Collections.Generic;

namespace OtherSquare.ViewModels
{
    public class LMS_Flashcards
    {
        public LMS_Flashcards_Settings Settings { get; set; }
        public LMS_Flashcards_Subjects Subjects { get; set; }
        public LMS_Flashcards_Categories Categories { get; set; }
        public LMS_Flashcards_Flashcards Flashcards { get; set; }

        public LMS_Flashcards() { }

        public LMS_Flashcards(UserSetting userSetting)
        {
            LMS_Flashcards_Settings set = null;
            try
            {
                if (string.IsNullOrEmpty(userSetting.SettingsJSON))
                {
                    set = new LMS_Flashcards_Settings();
                    userSetting.SettingsJSON = JsonConvert.SerializeObject(set);
                    userSetting.SaveSettings();
                }
                else
                {
                    set = JsonConvert.DeserializeObject<LMS_Flashcards_Settings>(userSetting.SettingsJSON);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            this.Settings = set;
            this.Subjects = new LMS_Flashcards_Subjects(set);
            this.Categories = new LMS_Flashcards_Categories(set);
            this.Flashcards = new LMS_Flashcards_Flashcards(set);
        }
    }

    public class LMS_Flashcards_Settings
    {
        public bool SettingsAccordionExpanded { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IncludeArchive { get; set; }

        public bool SubjectAccordionExpanded { get; set; }
        public Guid SelectedSubjectGuid { get; set; }
        public string SelectedSubjectTitle { get; set; }

        public bool CategoryAccordionExpanded { get; set; }
        public Guid SelectedCategoryGuid { get; set; }
        public string SelectedCategoryTitle { get; set; }

        public bool FlashcardAccordionExpanded { get; set; }
        public Guid SelectedFlashcardGuid { get; set; }
        public string SelectedFlashcardTitle { get; set; }
        public string SelectedFlashcardQuestion { get; set; }
        public string SelectedFlashcardAnswer { get; set; }

        public LMS_Flashcards_Settings()
        {
            this.SettingsAccordionExpanded = true;
            this.BeginDate = DateTime.UtcNow.AddMonths(-6);
            this.EndDate = DateTime.UtcNow;
            this.IncludeArchive = false;

            this.SubjectAccordionExpanded = true;
            this.SelectedSubjectGuid = Guid.Empty;
            this.SelectedSubjectTitle = "";

            this.CategoryAccordionExpanded = true;
            this.SelectedCategoryGuid = Guid.Empty;
            this.SelectedCategoryTitle = "";

            this.FlashcardAccordionExpanded = true;
            this.SelectedFlashcardGuid = Guid.Empty;
            this.SelectedFlashcardTitle = "";
            this.SelectedFlashcardQuestion = "";
            this.SelectedFlashcardAnswer = "";
        }
    }
}
