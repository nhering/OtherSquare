using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtherSquare.Models
{
    [Table("FlashCard", Schema = "lms")]
    public class FlashCard
    {
        [Key]
        public Guid FlashCardGuid { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsArchived { get; set; } //TODO Remove this property from the model

        public Guid CategoryGuid { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<FlashCardAnswer> FlashcardAnswers { get; set; }

        public FlashCard()
        {
            this.FlashCardGuid = Guid.Empty;
            this.Title = "";
            this.Question = "";
            this.Answer = "";
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.Title)) return;
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    FlashCard dbFlashcard = db.FlashCards.FirstOrDefault(s => s.FlashCardGuid == this.FlashCardGuid);
                    if (dbFlashcard == null)
                    {
                        dbFlashcard = new FlashCard()
                        {
                            FlashCardGuid = Guid.NewGuid(),
                            Title = this.Title,
                            Question = this.Question,
                            Answer = this.Answer
                        };
                        db.FlashCards.Add(dbFlashcard);
                    }
                    else
                    {
                        dbFlashcard.Title = this.Title;
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
    }
}