using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtherSquare.Models
{
    [Table("Subject_Category", Schema = "study")]
    public class Subject_Category
    {
        [Key, Column(Order = 0)]
        public Guid SubjectGuid { get; set; }
        public virtual Subject Subject { get; set; }

        [Key, Column(Order = 1)]
        public Guid CategoryGuid { get; set; }
        public virtual Category Category { get; set; }
    }
}