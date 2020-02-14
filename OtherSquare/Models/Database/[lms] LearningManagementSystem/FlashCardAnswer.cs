using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtherSquare.Models
{
    [Table("FlashCardAnswer", Schema = "lms")]
    public class FlashCardAnswer
    {
        [Key]
        public DateTime TimeStamp { get; set; }
        public bool IsCorrect { get; set; }

        public Guid FlashCardGuid { get; set; }
        public virtual FlashCard FlashCard { get; set; }

        public FlashCardAnswer()
        {
            this.TimeStamp = DateTime.UtcNow;
        }

        public FlashCardAnswer(Guid flashCardGuid, bool isCorrect)
        {
            this.TimeStamp = DateTime.UtcNow;
            this.FlashCardGuid = FlashCardGuid;
            this.IsCorrect = IsCorrect;
        }
    }
}