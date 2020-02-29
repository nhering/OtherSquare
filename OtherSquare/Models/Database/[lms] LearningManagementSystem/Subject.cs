using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OtherSquare.ViewModels;

namespace OtherSquare.Models
{
    [Table("Subject", Schema = "lms")]
    public class Subject
    {
        [Key]
        public Guid SubjectGuid { get; set; }
        public string Title { get; set; }
        public bool IsArchived { get; set; }
        /// <summary>UI Property to set the check box of a subject in a list</summary>
        public bool IsSelected { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public Subject()
        {
            this.SubjectGuid = Guid.Empty;
            this.Title = "";
            this.IsArchived = false;
            this.IsSelected = false;
        }

        public Subject(string title)
        {
            this.SubjectGuid = Guid.NewGuid();
            this.Title = title.Trim();
            this.IsArchived = false;
            this.IsSelected = false;
        }
    }
}