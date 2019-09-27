using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtherSquare.Models
{
    [Table("EntityProperty", Schema = "acct")]
    public class EntityProperty
    {
        [Key, Column(Order = 0)]
        public Guid Guid { get; set; }

        [Key, Column(Order = 1)]
        public DateTime Created { get; set; }

        [Required]
        public string Key { get; set; }

        public string Value { get; set; }

        [Required]
        public string Type { get; set; }

        [ForeignKey("Entity")]
        [Column(Order = 2)]
        public Guid EntityGuid { get; set; }
        [ForeignKey("Entity")]
        [Column(Order = 3)]
        public DateTime EntityCreated { get; set; }
        public virtual Entity Entity { get; set; }
    }
}