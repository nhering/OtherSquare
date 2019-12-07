using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtherSquare.Models
{
    [Table("Subject", Schema = "study")]
    public class Subject
    {
        [Key]
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Subject_Category> Subject_Categories { get; set; }
        public virtual ICollection<FlashCard> FlashCards { get; set; }
               
        public Subject()
        {
            this.Guid = Guid.NewGuid();
            this.Title = "";
            this.IsDeleted = false;
        }

        public Subject(Guid guid, string title)
        {
            this.Guid = guid;
            this.Title = title;
            this.IsDeleted = false;
        }
    }
}