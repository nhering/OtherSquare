using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OtherSquare.Classes;
using OtherSquare.Models;

namespace OtherSquare.ViewModels
{
    /// <summary>Provides a list of each: Subject, Categories, and Flash Cards</summary>
    public class FlashCard_Index_ViewModel
    {
        public SCF_List Subjects { get; set; }
        public SCF_List Categories { get; set; }
        //public SCF_List FlashCards { get; set; }
        
        public FlashCard_Index_ViewModel()
        {
            this.Subjects = new SCF_List(SCF_ListType.SUBJECT);
            this.Categories = new SCF_List(SCF_ListType.CATEGORY);
            //this.FlashCards = new SCF_List(SCF_ListType.FLASH_CARD);
        }
    }

    public class SCF_List
    {
        public string Title { get; set; }
        public string Endpoint { get; set; }
        public List<SCF_ListItem> ListItems { get; set; }

        public SCF_List(string scf_ListType)
        {
            this.Title = scf_ListType;
            this.Endpoint = scf_ListType.Replace(" ", "");
            this.ListItems = new List<SCF_ListItem>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    switch (scf_ListType)
                    {
                        case SCF_ListType.SUBJECT:
                            List<Subject> subjects = db.Subjects.Where(s => s.IsDeleted == false).OrderBy(s => s.Title).ToList();
                            foreach (Subject s in subjects)
                            {
                                var correct = s.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.CorrectAnswers);
                                var incorrect = s.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.IncorrectAnswers);
                                var percent = Helpers.Math.Percent(correct, (correct + incorrect)) * 100;
                                this.ListItems.Add(new SCF_ListItem(s.Title, s.Guid, percent));
                            }
                            break;
                        case SCF_ListType.CATEGORY:
                            List<Category> categories = db.Categories.Where(c => c.IsDeleted == false).OrderBy(c => c.Title).ToList();
                            foreach (Category c in categories)
                            {
                                var correct = c.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.CorrectAnswers);
                                var incorrect = c.FlashCards.Where(f => f.IsDeleted == false).Sum(f => f.IncorrectAnswers);
                                var percent = Helpers.Math.Percent(correct, (correct + incorrect)) * 100;
                                this.ListItems.Add(new SCF_ListItem(c.Title, c.Guid, percent));
                            }
                            break;
                        //case SCF_ListType.FLASH_CARD:
                        //    List<FlashCard> flashCards = db.FlashCards.Where(f => f.IsDeleted == false).OrderBy(f => f.Title).ToList();
                        //    foreach (FlashCard card in flashCards)
                        //    {
                        //        var percent = Helpers.Math.Percent(card.CorrectAnswers, (card.CorrectAnswers + card.IncorrectAnswers)) * 100;
                        //        this.ListItems.Add(new SCF_ListItem(card.Title, card.Guid, percent));
                        //    }
                        //    break;
                    }
                }
            }
            catch (Exception e)
            {
                this.Title = scf_ListType;
                this.ListItems = new List<SCF_ListItem>();
            }
        }
    }

    public class SCF_ListItem
    {
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public int Percent { get; set; }

        public SCF_ListItem() { }
        public SCF_ListItem(string title, Guid guid, int percent)
        {
            this.Title = title;
            this.Guid = guid;
            this.Percent = percent;
        }
    }
}
