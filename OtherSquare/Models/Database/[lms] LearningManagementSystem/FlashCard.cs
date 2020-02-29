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
        public bool IsSelected { get; set; }

        public Guid CategoryGuid { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<FlashCardAnswer> FlashcardAnswers { get; set; }

        public FlashCard() { }

        public FlashCard(Guid categoryGuid)
        {
            this.FlashCardGuid = Guid.NewGuid();
            this.Title = "";
            this.Question = "";
            this.Answer = "";
            this.CategoryGuid = categoryGuid;
        }
    }
}