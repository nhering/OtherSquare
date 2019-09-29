using System;
using System.Collections.Generic;
using OtherSquare.Classes;
using OtherSquare.Models;
using System.Linq;
using System.Web;

namespace OtherSquare.ViewModels
{
    public class EntityPropertyViewModel
    {
        public EntityProperty EntityProperty { get; set; }
        public List<EntityProperty> History { get; set; }
        public bool IsDeleted { get; set; }

        public EntityPropertyViewModel()
        {
            this.EntityProperty = new EntityProperty();
            this.History = new List<EntityProperty>();
            this.IsDeleted = false;
        }

        public EntityPropertyViewModel(Guid entityPropertyGuid)
        {
            this.EntityProperty = new EntityProperty();
            this.History = new List<EntityProperty>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.History = db.EntityProperties.Where(r => r.Guid == entityPropertyGuid).ToList();
                    this.IsDeleted = this.History.Any(r => r.Type == EntityType.DELETED);
                }
            }
            catch(Exception e)
            {

            }
        }
    }
}