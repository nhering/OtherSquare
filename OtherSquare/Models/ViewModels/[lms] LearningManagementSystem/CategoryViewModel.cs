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
                this._settings = value;
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

        private InputValidation _title_Validation { get; set; }
        public InputValidation Title_Validation
        {
            get
            {
                if (_title_Validation == null)
                {
                    _title_Validation = InputValidation.Empty();
                }
                return _title_Validation;
            }
            set
            {
                _title_Validation = value;
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

        //public CategoryViewModel CreateCategory(string categoryTitle, NAV_FlashcardSettings settings = null)
        //{
        //    try
        //    {
        //        InputValidation titleValidation = new InputValidation();
        //        categoryTitle = categoryTitle.Trim();
        //        using (OtherSquareDbContext db = new OtherSquareDbContext())
        //        {
        //            Category existingCategory = db.Categories.FirstOrDefault(c => c.Title.ToLower() == categoryTitle.ToLower());
        //            if (existingCategory != null)
        //            {
        //                if (existingCategory.IsArchived)
        //                {
        //                    titleValidation = InputValidation.Error($"The category '{categoryTitle}' already exists, but was archived.");
        //                    return this;
        //                }
        //                else
        //                {
        //                    titleValidation = InputValidation.Error($"The category '{categoryTitle}' already exists.");
        //                    return this;
        //                }
        //            }
        //            else
        //            {
        //                Category newCategory = new Category(categoryTitle);
        //                db.Categories.Add(newCategory);
        //                db.SaveChanges();
        //                titleValidation = InputValidation.Success("New subject successfully created.");
        //            }
        //        }
        //        CategoryViewModel result = new CategoryViewModel(settings);
        //        result.Title_Validation = titleValidation;
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        this.Title_Validation = InputValidation.Error("There was an error saving the subject to the database.");
        //        //TODO Log exception
        //        throw;
        //    }
        //}

        public SubjectViewModel ArchiveCategories()
        {
            //TODO set the IsArchived property to true for the matching records
            return new SubjectViewModel();
        }

        public SubjectViewModel UnArchiveCategories()
        {
            //TODO set the IsArchived property to false for the matching records
            return new SubjectViewModel();
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
                        Categories = db.Categories.Where(c => c.SubjectGuid == Settings.SelectedSubject.SubjectGuid).ToList();
                    }
                    else
                    {
                        Categories = db.Categories.Where(c => c.IsArchived == false && c.SubjectGuid == Settings.SelectedSubject.SubjectGuid).ToList();
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
                            SelectedObject = c.Copy(),
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
        
        public static InputValidation SaveCategory(Category category)
        {
            if (category.IsValid())
            {
                if (category.IsNew())
                {
                    if (category.TitleExistsInDatabase())
                    {
                        InputValidation val = InputValidation.Alert("A category with that title already exitst. (It could be archived)");
                        val.Object = category;
                        return val;
                    }
                    else
                    {
                        category.Create();
                        InputValidation val = InputValidation.Empty();
                        val.Object = category;
                        return val;
                    }
                }
                else
                {
                    category.Update();
                    InputValidation val = InputValidation.Empty();
                    val.Object = category;
                    return val;
                }
            }
            else
            {
                InputValidation val = InputValidation.Alert(category.ValidationErrors());
                val.Object = category;
                return val;
            }
        }

        #endregion
    }
}