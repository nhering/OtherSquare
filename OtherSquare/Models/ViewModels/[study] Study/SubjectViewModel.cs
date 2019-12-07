using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OtherSquare.Classes;
using OtherSquare.Models;

namespace OtherSquare.ViewModels
{
    public class SubjectViewModel
    {
        public Subject Subject { get; set; }
        public Subject_Category_ViewModel Subject_Category_ViewModel { get; set; }
        public Subject_FlashCard_ViewModel Subject_FlashCard_ViewModel { get; set; }
        
        public SubjectViewModel()
        {
            this.Init();
        }

        public SubjectViewModel(Guid subjectGuid, string title)
        {
            Subject subject = null;
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    //if (db.Subjects.Any(s => s.Title.ToLower() == title.ToLower())) return;
                    //TODO Decide how to handle duplicates

                    subject = db.Subjects.FirstOrDefault(s => s.Guid == subjectGuid);
                    if (subject == null)
                    {
                        subject = new Subject(subjectGuid, title);
                        db.Subjects.Add(subject);
                    }
                    else
                    {
                        subject.Title = title;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            this.Subject = subject;
            this.Subject_Category_ViewModel = new Subject_Category_ViewModel(subjectGuid);
            this.Subject_FlashCard_ViewModel = new Subject_FlashCard_ViewModel(subjectGuid);
        }

        /// <summary>Read an existing Subject from the database.</summary>
        /// <param name="subjectGuid">The Guid of the Subject to be loaded</param>
        public SubjectViewModel(Guid subjectGuid)
        {
            if (subjectGuid == null || subjectGuid == Guid.Empty)
            {
                //TODO handle the error
                return;
            }

            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.Subject = db.Subjects.FirstOrDefault(s => s.Guid == subjectGuid);
                    if (this.Subject == null)
                    {
                        this.Init();
                        return;
                    }

                    this.Subject_Category_ViewModel = new Subject_Category_ViewModel(subjectGuid);
                    this.Subject_FlashCard_ViewModel = new Subject_FlashCard_ViewModel(subjectGuid);
                }
            }
            catch(Exception e)
            {

            }
        }

        private void Init()
        {
            this.Subject = new Subject();
            this.Subject_Category_ViewModel = new Subject_Category_ViewModel();
            this.Subject_FlashCard_ViewModel = new Subject_FlashCard_ViewModel();

            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.Subject_Category_ViewModel.CategoryDDL = db.Categories.Where(c => c.IsDeleted == false).ToList();
                }
            }
            catch (Exception e)
            {
                this.Subject_Category_ViewModel.CategoryDDL = new List<Category>();
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

        public bool AddCategory(Guid subjectGuid, Guid categoryGuid)
        {
            try
            {
                using(OtherSquareDbContext db = new OtherSquareDbContext())
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
                        this.Subject_Category_ViewModel = new Subject_Category_ViewModel(subjectGuid);
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

        public bool RemoveCategory(Guid subjectGuid, Guid categoryGuid)
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
                        this.Subject_Category_ViewModel = new Subject_Category_ViewModel(subjectGuid);
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

    public class Subject_Category_ViewModel
    {
        public List<Subject_Category_ListItem> ListItems { get; set; }
        public List<Category> CategoryDDL { get; set; }

        public Subject_Category_ViewModel()
        {
            this.ListItems = new List<Subject_Category_ListItem>();
            this.CategoryDDL = new List<Category>();
        }

        public Subject_Category_ViewModel(Guid subjectGuid)
        {
            this.ListItems = new List<Subject_Category_ListItem>();
            this.CategoryDDL = new List<Category>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Category> thisSubjectsCategories = db.Subject_Categories.Where(sc => sc.SubjectGuid == subjectGuid).Select(c => c.Category).OrderBy(c => c.Title).ToList();
                    if (thisSubjectsCategories == null)
                    {
                        this.ListItems = new List<Subject_Category_ListItem>();
                        this.CategoryDDL = db.Categories.ToList();
                    }
                    else
                    {
                        foreach(var c in thisSubjectsCategories)
                        {
                            var correct = c.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.CorrectAnswers);
                            var incorrect = c.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.IncorrectAnswers);
                            this.ListItems.Add(new Subject_Category_ListItem(c.Title, c.Guid, correct, incorrect));
                        }
                        List<Category> theRestOfTheCategories = db.Categories.Where(c => !thisSubjectsCategories.Contains(c)).OrderBy(c => c.Title).ToList();
                        if (theRestOfTheCategories == null)
                        {
                            this.CategoryDDL = new List<Category>();
                        }
                        else
                        {
                            this.CategoryDDL = theRestOfTheCategories;
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }

    public class Subject_Category_ListItem
    {
        public string CategoryTitle { get; set; }
        public Guid CategoryGuid { get; set; }
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

        public Subject_Category_ListItem() { }
        public Subject_Category_ListItem(string title, Guid guid, int correct, int incorrect)
        {
            this.CategoryTitle = title;
            this.CategoryGuid = guid;
            this.CorrectCount = correct;
            this.IncorrectCount = incorrect;
        }
    }

    public class Subject_FlashCard_ViewModel
    {
        public List<Subject_FlashCard_ListItem> ListItems { get; set; }

        public Subject_FlashCard_ViewModel()
        {
            this.ListItems = new List<Subject_FlashCard_ListItem>();
        }

        public Subject_FlashCard_ViewModel(Guid subjectGuid)
        {
            this.ListItems = new List<Subject_FlashCard_ListItem>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<FlashCard> cards = db.Subjects.FirstOrDefault(s => s.Guid == subjectGuid).FlashCards.ToList();
                    if (cards == null) cards = new List<FlashCard>();

                    foreach(var card in cards)
                    {
                        this.ListItems.Add(new Subject_FlashCard_ListItem(card));
                    }
                }
            }
            catch
            {

            }
        }
    }

    public class Subject_FlashCard_ListItem
    {
        public string FlashCardTitle { get; set; }
        public Guid FlashCardGuid { get; set; }
        public string CategoryTitle { get; set; }
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

        public Subject_FlashCard_ListItem() { }
        public Subject_FlashCard_ListItem(FlashCard flashCard)
        {
            this.FlashCardTitle = flashCard.Title;
            this.FlashCardGuid = flashCard.Guid;
            this.CategoryTitle = flashCard.Category.Title ?? "";
            this.CorrectCount = flashCard.CorrectAnswers;
            this.IncorrectCount = flashCard.IncorrectAnswers;
        }
    }
}