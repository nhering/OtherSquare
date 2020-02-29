using OtherSquare.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Web;

namespace OtherSquare.ViewModels
{
    public class CategoryViewModel
    {
        #region Properties

        private NAV_FlashcardSettings _settings { get; set; }
        private NAV_FlashcardSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    this._settings = new NAV_FlashcardSettings();
                }
                return _settings;
            }
            set
            {
                if (value == null)
                {
                    value = new NAV_FlashcardSettings();
                }
                _settings = value;
            }
        }

        private List<ListItemViewModel> _categoryList { get; set; }
        public List<ListItemViewModel> CategoryList
        {
            get
            {
                if (_categoryList == null)
                {
                    _categoryList = new List<ListItemViewModel>();
                }
                return _categoryList;
            }
            set
            {
                _categoryList = value;
            }
        }

        #endregion

        #region Constructor

        public CategoryViewModel(NAV_FlashcardSettings settings = null)
        {
            try
            {
                this.Settings = settings;
                this.buildCategoryList();
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        #endregion

        #region Methods

        public static void ArchiveCategories(List<Guid> categoryGuids)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Category> selectedCategories = db.Categories.Where(c => categoryGuids.Contains(c.CategoryGuid)).ToList();
                    foreach (Category c in selectedCategories)
                    {
                        c.IsArchived = true;
                        c.IsSelected = false;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log Error
                throw;
            }
        }

        public static void UnArchiveCategories(List<Guid> categoryGuids)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Category> selectedCategories = db.Categories.Where(c => categoryGuids.Contains(c.CategoryGuid)).ToList();
                    foreach (Category c in selectedCategories)
                    {
                        c.IsArchived = false;
                        c.IsSelected = false;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log Error
                throw;
            }
        }

        private void buildCategoryList()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Category> Categories = new List<Category>();
                    if ((bool)this.Settings.IncludeArchive)
                    {
                        Categories = db.Categories.Where(c => c.SubjectGuid == Settings.SelectedSubjectGuid).ToList();
                    }
                    else
                    {
                        Categories = db.Categories.Where(c => c.SubjectGuid == Settings.SelectedSubjectGuid 
                                                              && c.IsArchived == false).ToList();
                    }

                    foreach (Category c in Categories)
                    {
                        int divisor = c.Flashcards.Sum(f => f.FlashcardAnswers.Count());
                        int dividend = c.Flashcards.Sum(f => f.FlashcardAnswers.Sum(fa => fa.IsCorrect ? 1 : 0));
                        bool scoreIsNa = true;
                        int score = 0;
                        if (divisor > 0 && dividend == 0)
                        {
                            scoreIsNa = false;
                        }
                        else if (divisor > 0 && dividend > 0)
                        {
                            scoreIsNa = false;
                            score = dividend / divisor;
                        }

                        ListItemViewModel li = new ListItemViewModel()
                        {
                            //SelectedObject = c.Copy(),
                            Guid = c.CategoryGuid,
                            Title = c.Title,
                            ScoreIsNA = scoreIsNa,
                            Score = score,
                            IsArchived = c.IsArchived,
                            Selected = c.IsSelected
                        };
                        this.CategoryList.Add(li);
                    }
                }
            }
            catch (Exception e)
            {
                //TODO Log Error
                throw;
            }
        }
        
        public static InputValidation SaveCategory(UserSetting userSetting)
        {
            NAV_FlashcardSettings fSet = null;
            Guid uiSubjectGuid = Guid.Empty;
            Guid uiCategoryGuid = Guid.Empty;
            string uiTitle = null;
            Category dbCategory = null;
            try
            {
                fSet = JsonConvert.DeserializeObject<NAV_FlashcardSettings>(userSetting.SettingsJSON);
                uiSubjectGuid = fSet.SelectedSubjectGuid;
                if (uiSubjectGuid == Guid.Empty || uiSubjectGuid == null) return InputValidation.Error("Something went wrong. This category has no subject. Can not create orphan categories.");
                uiCategoryGuid = fSet.SelectedCategoryGuid;
                uiTitle = fSet.SelectedCategoryTitle.Trim();
                if (string.IsNullOrEmpty(uiTitle)) return InputValidation.Error("Title can not be empty.");
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    if (uiCategoryGuid != Guid.Empty && uiCategoryGuid != null)
                    {
                        dbCategory = db.Categories.FirstOrDefault(c => c.CategoryGuid == uiCategoryGuid 
                                                                  && c.SubjectGuid == uiSubjectGuid);
                        if (dbCategory == null)
                        {
                            //Something is wrong. Why didn't we find a record?
                            throw new Exception($"The category with the Guid {uiCategoryGuid.ToString()} was not found in the database.");
                        }
                        dbCategory.Title = uiTitle;
                        db.SaveChanges();

                        //update the user setting and save it
                        fSet.SelectedCategoryTitle = uiTitle;
                        userSetting.SettingsJSON = JsonConvert.SerializeObject(fSet);
                        userSetting.SaveSettings();

                        return InputValidation.Success("Category successfully updated.");
                    }
                    else
                    {
                        dbCategory = db.Categories.FirstOrDefault(c => c.SubjectGuid == uiSubjectGuid
                                                                  && c.Title.ToLower() == uiTitle.ToLower());
                        if (dbCategory != null)
                        {
                            return InputValidation.Alert($"A category with the title {uiTitle} already exitst. (It could be archived)");
                        }

                        Category newCategory = new Category(uiSubjectGuid, uiTitle);
                        db.Categories.Add(newCategory);
                        db.SaveChanges();
                        return InputValidation.Success("Category successfully created.");
                    }

                }
            }
            catch(Exception e)
            {
                //TODO do something with the fSet here?
                return InputValidation.Error(e.Message);
            }
        }

        #endregion
    }
}