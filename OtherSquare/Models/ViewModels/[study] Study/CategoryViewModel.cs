using OtherSquare.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtherSquare.Models.ViewModels._study__Study
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public Category_Subject_ViewModel Category_Subject_ViewModel { get; set; }
        public Category_FlashCard_ViewModel Category_FlashCard_ViewModel { get; set; }

        public CategoryViewModel()
        {
            this.Init();
        }

        public CategoryViewModel(Guid categoryGuid, string title)
        {
            Category category = null;
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    //if (db.Categories.Any(s => s.Title.ToLower() == title.ToLower())) return;
                    //TODO Decide how to handle duplicates

                    category = db.Categories.FirstOrDefault(c => c.Guid == categoryGuid);
                    if (category == null)
                    {
                        category = new Category(categoryGuid, title);
                        db.Categories.Add(category);
                    }
                    else
                    {
                        category.Title = title;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            this.Category = category;
            this.Category_Subject_ViewModel = new Category_Subject_ViewModel(categoryGuid);
            this.Category_FlashCard_ViewModel = new Category_FlashCard_ViewModel(categoryGuid);
        }

        /// <summary>Read an existing Category from the database.</summary>
        /// <param name="subjectGuid">The Guid of the Category to be loaded</param>
        public CategoryViewModel(Guid categoryGuid)
        {
            if (categoryGuid == null || categoryGuid == Guid.Empty)
            {
                //TODO handle the error
                return;
            }

            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.Category = db.Categories.FirstOrDefault(c => c.Guid == categoryGuid);
                    if (this.Category == null)
                    {
                        this.Init();
                        return;
                    }

                    this.Category_Subject_ViewModel = new Category_Subject_ViewModel(categoryGuid);
                    this.Category_FlashCard_ViewModel = new Category_FlashCard_ViewModel(categoryGuid);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void Init()
        {
            this.Category = new Category();
            this.Category_Subject_ViewModel = new Category_Subject_ViewModel();
            this.Category_FlashCard_ViewModel = new Category_FlashCard_ViewModel();

            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.Category_Subject_ViewModel.SubjectDDL = db.Subjects.Where(s => s.IsDeleted == false).ToList();
                }
            }
            catch (Exception e)
            {
                this.Category_Subject_ViewModel.SubjectDDL = new List<Subject>();
            }
        }

        public static bool DeleteSubject(Guid subjectGuid)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Subject existingSubject = db.Subjects.FirstOrDefault(s => s.Guid == subjectGuid);
                    if (existingSubject == null) return false;
                    existingSubject.IsDeleted = true;
                    db.SaveChanges();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool AddSubject(Guid categoryGuid, Guid subjectGuid)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Subject_Category sc = db.Subject_Categories.FirstOrDefault(s => s.SubjectGuid == subjectGuid && s.CategoryGuid == categoryGuid);
                    if (sc == null)
                    {
                        sc = new Subject_Category()
                        {
                            SubjectGuid = subjectGuid,
                            CategoryGuid = categoryGuid
                        };
                        db.Subject_Categories.Add(sc);
                        db.SaveChanges();
                        this.Category_Subject_ViewModel = new Category_Subject_ViewModel(categoryGuid);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;

        }

        public bool RemoveSubject(Guid subjectGuid, Guid categoryGuid)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Subject_Category sc = db.Subject_Categories.FirstOrDefault(s => s.SubjectGuid == subjectGuid && s.CategoryGuid == categoryGuid);
                    if (sc != null)
                    {
                        db.Subject_Categories.Remove(sc);
                        db.SaveChanges();
                        this.Category_Subject_ViewModel = new Category_Subject_ViewModel(categoryGuid);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    public class Category_Subject_ViewModel
    {
        public List<Category_Subject_ListItem> ListItems { get; set; }
        public List<Subject> SubjectDDL { get; set; }

        public Category_Subject_ViewModel()
        {
            this.ListItems = new List<Category_Subject_ListItem>();
            this.SubjectDDL = new List<Subject>();
        }

        public Category_Subject_ViewModel(Guid categoryGuid)
        {
            this.ListItems = new List<Category_Subject_ListItem>();
            this.SubjectDDL = new List<Subject>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Subject> thisSubjectsCategories = db.Subject_Categories.Where(sc => sc.CategoryGuid == categoryGuid).Select(c => c.Subject).OrderBy(c => c.Title).ToList();
                    if (thisSubjectsCategories == null)
                    {
                        this.ListItems = new List<Category_Subject_ListItem>();
                        this.SubjectDDL = db.Subjects.ToList();
                    }
                    else
                    {
                        foreach (var c in thisSubjectsCategories)
                        {
                            var correct = c.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.CorrectAnswers);
                            var incorrect = c.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.IncorrectAnswers);
                            this.ListItems.Add(new Category_Subject_ListItem(c.Title, c.Guid, correct, incorrect));
                        }
                        List<Subject> theRestOfTheSubjects = db.Subjects.Where(s => !thisSubjectsCategories.Contains(s)).OrderBy(s => s.Title).ToList();
                        if (theRestOfTheSubjects == null)
                        {
                            this.SubjectDDL = new List<Subject>();
                        }
                        else
                        {
                            this.SubjectDDL = theRestOfTheSubjects;
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }

    public class Category_Subject_ListItem
    {
        public string SubjectTitle { get; set; }
        public Guid SubjectGuid { get; set; }
        public int CorrectCount { get; set; }
        public int IncorrectCount { get; set; }
        public int Score
        {
            get
            {
                try
                {
                    return Helpers.Math.Percent(this.CorrectCount, this.CorrectCount + this.IncorrectCount) * 100;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public Category_Subject_ListItem() { }
        public Category_Subject_ListItem(string title, Guid guid, int correct, int incorrect)
        {
            this.SubjectTitle = title;
            this.SubjectGuid = guid;
            this.CorrectCount = correct;
            this.IncorrectCount = incorrect;
        }
    }

    public class Category_FlashCard_ViewModel
    {
        public List<Category_FlashCard_ListItem> ListItems { get; set; }

        public Category_FlashCard_ViewModel()
        {
            this.ListItems = new List<Category_FlashCard_ListItem>();
        }

        public Category_FlashCard_ViewModel(Guid categoryGuid)
        {
            this.ListItems = new List<Category_FlashCard_ListItem>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<FlashCard> cards = db.Categories.FirstOrDefault(s => s.Guid == categoryGuid).FlashCards.ToList();
                    if (cards == null) cards = new List<FlashCard>();

                    foreach (var card in cards)
                    {
                        this.ListItems.Add(new Category_FlashCard_ListItem(card));
                    }
                }
            }
            catch
            {

            }
        }
    }

    public class Category_FlashCard_ListItem
    {
        public string FlashCardTitle { get; set; }
        public Guid FlashCardGuid { get; set; }
        public string SubjectTitle { get; set; }
        public int CorrectCount { get; set; }
        public int IncorrectCount { get; set; }
        public int Score
        {
            get
            {
                try
                {
                    return this.CorrectCount / (this.CorrectCount + this.IncorrectCount) * 100;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public Category_FlashCard_ListItem() { }
        public Category_FlashCard_ListItem(FlashCard flashCard)
        {
            this.FlashCardTitle = flashCard.Title;
            this.FlashCardGuid = flashCard.Guid;
            this.SubjectTitle = flashCard.Subject.Title ?? "";
            this.CorrectCount = flashCard.CorrectAnswers;
            this.IncorrectCount = flashCard.IncorrectAnswers;
        }
    }
}