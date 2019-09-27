using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtherSquare.Models
{
    [Table("Entity", Schema = "acct")]
    public class Entity
    {
        [Key, Column(Order = 0)]
        public Guid Guid { get; set; }

        [Key, Column(Order = 1)]
        public DateTime Created { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        public virtual ICollection<EntityProperty> Properties { get; set; }
    }
}