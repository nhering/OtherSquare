using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace OtherSquare.Models
{
    [Table("Category", Schema = "lms")]
    public class Category
    {
        [Key]
        public Guid CategoryGuid { get; set; }
        public string Title { get; set; }
        public bool IsArchived { get; set; }
        public bool IsSelected { get; set; }
        
        public Guid SubjectGuid { get; set; }
        public virtual Subject Subject { get; set; }

        public virtual ICollection<FlashCard> Flashcards { get; set; }

        public Category()
        {
            this.CategoryGuid = Guid.Empty;
            this.Title = "";
            this.IsArchived = false;
            this.IsSelected = false;
        }

        public Category(Guid subjectGuid, string categoryTitle)
        {
            this.CategoryGuid = Guid.NewGuid();
            this.Title = categoryTitle;
            this.IsArchived = false;
            this.SubjectGuid = subjectGuid;
        }
    }
}