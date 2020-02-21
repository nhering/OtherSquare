using Newtonsoft.Json;
using OtherSquare.Models;
using System;
using System.Collections.Generic;

namespace OtherSquare.ViewModels
{
    public class NAV_FlashCardViewModel
    {
        public NAV_FlashcardSettings Settings { get; set; }
        public SubjectViewModel SubjectViewModel { get; set; }
        public CategoryViewModel CategoryViewModel { get; set; }
        public FlashcardViewModel FlashcardViewModel { get; set; }

        public NAV_FlashCardViewModel() { }

        public NAV_FlashCardViewModel(UserSetting userSetting)
        {
            NAV_FlashcardSettings set = null;
            try
            {
                if (string.IsNullOrEmpty(userSetting.SettingsJSON))
                {
                    set = new NAV_FlashcardSettings();
                    userSetting.SettingsJSON = JsonConvert.SerializeObject(set);
                    userSetting.SaveSettings();
                }
                else
                {
                    set = JsonConvert.DeserializeObject<NAV_FlashcardSettings>(userSetting.SettingsJSON);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            this.Settings = set;
            this.SubjectViewModel = new SubjectViewModel(set);
            this.CategoryViewModel = new CategoryViewModel(set);
            this.FlashcardViewModel = new FlashcardViewModel(set);
        }

        public static NAV_FlashCardViewModel GetSubjectViewModel(UserSetting userSetting)
        {
            NAV_FlashCardViewModel viewModel = new NAV_FlashCardViewModel();
            NAV_FlashcardSettings set = null;
            try
            {
                if (string.IsNullOrEmpty(userSetting.SettingsJSON))
                {
                    // Shouldn't hit this if we are loading a partial to refresh 
                    // a section of the page that has already been loaded successfully
                    set = new NAV_FlashcardSettings();
                    userSetting.SettingsJSON = JsonConvert.SerializeObject(set);
                    userSetting.SaveSettings();
                }
                else
                {
                    //TODO Does this break if the SelectedSubject, SelectedCategory, or SelectedFlashcard are null?
                    set = JsonConvert.DeserializeObject<NAV_FlashcardSettings>(userSetting.SettingsJSON);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            viewModel.Settings = set;
            viewModel.SubjectViewModel = new SubjectViewModel(set);
            return viewModel;
        }

        public static NAV_FlashCardViewModel GetCategoryViewModel(UserSetting userSetting)
        {
            NAV_FlashCardViewModel viewModel = new NAV_FlashCardViewModel();
            NAV_FlashcardSettings set = null;
            try
            {
                if (string.IsNullOrEmpty(userSetting.SettingsJSON))
                {
                    // Shouldn't hit this if we are loading a partial to refresh 
                    // a section of the page that has already been loaded successfully
                    set = new NAV_FlashcardSettings();
                    userSetting.SettingsJSON = JsonConvert.SerializeObject(set);
                    userSetting.SaveSettings();
                }
                else
                {
                    //TODO Does this break if the SelectedSubject, SelectedCategory, or SelectedFlashcard are null?
                    set = JsonConvert.DeserializeObject<NAV_FlashcardSettings>(userSetting.SettingsJSON);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            viewModel.Settings = set;
            viewModel.CategoryViewModel = new CategoryViewModel(set);
            return viewModel;
        }

        public static NAV_FlashCardViewModel GetFlashCardViewModel(UserSetting userSetting)
        {
            NAV_FlashCardViewModel viewModel = new NAV_FlashCardViewModel();
            NAV_FlashcardSettings set = null;
            try
            {
                if (string.IsNullOrEmpty(userSetting.SettingsJSON))
                {
                    // Shouldn't hit this if we are loading a partial to refresh 
                    // a section of the page that has already been loaded successfully
                    set = new NAV_FlashcardSettings();
                    userSetting.SettingsJSON = JsonConvert.SerializeObject(set);
                    userSetting.SaveSettings();
                }
                else
                {
                    //TODO Does this break if the SelectedSubject, SelectedCategory, or SelectedFlashcard are null?
                    set = JsonConvert.DeserializeObject<NAV_FlashcardSettings>(userSetting.SettingsJSON);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            viewModel.Settings = set;
            //viewModel.SubjectViewModel = new SubjectViewModel(set);
            //viewModel.CategoryViewModel = new CategoryViewModel(set);
            viewModel.FlashcardViewModel = new FlashcardViewModel(set);
            return viewModel;
        }
    }

    public class NAV_FlashcardSettings
    {
        public bool SettingsAccordionExpanded { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IncludeArchive { get; set; }

        public bool SubjectAccordionExpanded { get; set; }
        public Subject SelectedSubject { get; set; }

        public bool CategoryAccordionExpanded { get; set; }
        public Category SelectedCategory { get; set; }

        public bool FlashcardAccordionExpanded { get; set; }
        public FlashCard SelectedFlashcard { get; set; }

        public NAV_FlashcardSettings()
        {
            this.SettingsAccordionExpanded = true;
            this.BeginDate = DateTime.UtcNow.AddMonths(-6);
            this.EndDate = DateTime.UtcNow;
            this.IncludeArchive = false;

            this.SubjectAccordionExpanded = true;
            this.SelectedSubject = new Subject();

            this.CategoryAccordionExpanded = true;
            this.SelectedCategory = new Category();

            this.FlashcardAccordionExpanded = true;
            this.SelectedFlashcard = new FlashCard();
        }
    }
}
