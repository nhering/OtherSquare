using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtherSquare.Models
{
    [Table("FlashCard", Schema = "study")]
    public class FlashCard
    {
        [Key]
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Subject Subject { get; set; }
        public Guid SubjectGuid { get; set; }

        public virtual Category Category { get; set; }
        public Guid CategoryGuid { get; set; }

        public FlashCard()
        {
            this.Guid = Guid.NewGuid();
            this.Title = "";
            this.Question = "";
            this.Answer = "";
            this.CorrectAnswers = 0;
            this.IncorrectAnswers = 0;
            this.IsDeleted = false;
        }
    }
}