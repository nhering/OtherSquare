using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OtherSquare.Classes;
using OtherSquare.Models;

namespace OtherSquare.ViewModels
{
    public class EntitySearchViewModel
    {
        public List<Entity> SearchResults { get; set; }

        public EntitySearchViewModel()
        {
            this.SearchResults = new List<Entity>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.SearchResults = db.Entities.Where(r => r.Type != EntityType.DELETED
                    && r.Type != EntityType.HISTORY
                    && r.Type != EntityType.TEMPLATE).OrderBy(r => r.Name).ToList();
                }
            }
            catch (Exception e)
            {
                //Logger.Error("EntitySearchModel", "EntitySearchViewModel()", e);
            }
        }

        public EntitySearchViewModel(string searchString)
        {
            this.SearchResults = new List<Entity>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Regex regex = new Regex(searchString.ToLower());
                    this.SearchResults = db.Entities.Where(r => regex.IsMatch(r.Name.ToLower())
                    && r.Type != EntityType.DELETED
                    && r.Type != EntityType.HISTORY
                    && r.Type != EntityType.TEMPLATE).OrderBy(r => r.Name).ToList();
                }
            }
            catch (Exception e)
            {
                //Logger.Error("EntitySearchViewModel", "EntitySearchViewModel(string searchResuts)", e);
            }
        }
    }

    public class EntityDetailViewModel
    {
        public Entity Entity { get; set; }
        public List<EntityProperty> Properties { get; set; }

        public EntityDetailViewModel()
        {
            this.Entity = new Entity();
            this.Properties = new List<EntityProperty>();
        }

        public EntityDetailViewModel(Guid entityGuid)
        {
            this.Entity = new Entity();
            this.Properties = new List<EntityProperty>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.Entity = db.Entities.FirstOrDefault(r => r.Guid == entityGuid && r.Type != EntityType.HISTORY);
                }
            }
            catch (Exception e)
            {
                //Logger.Error("EntityDetailViewModel", "EntitiyDetailViewModel(int entityCreated)", e);
            }
        }

        public bool IsValid()
        {
            if (this.Entity == null) return false;
            if (this.Entity.Name == "") return false;
            return true;
        }

        private void Create()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.Entity.Created = DateTime.UtcNow;
                    this.Entity.Guid = Guid.NewGuid();
                    this.Entity.Type = EntityType.ACTIVE;
                    this.Entity.Properties = this.Properties;
                    db.Entities.Add(this.Entity);
                    foreach (var prop in this.Properties)
                    {
                        prop.Guid = Guid.NewGuid();
                        prop.Created = DateTime.UtcNow;
                        prop.Entity = this.Entity;
                        prop.Type = EntityType.ACTIVE;
                        db.EntityProperties.Add(prop);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //Logger.Error("EntityDetailViewModel", "Create", e);
            }
        }

        public void Update()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    if (this.Entity.Guid == Guid.Empty) { this.Create(); return; }
                    Entity existingEntity = db.Entities.FirstOrDefault(r => r.Guid == this.Entity.Guid);
                    if (existingEntity == null) { this.Create(); return; }

                    if (existingEntity.Description != this.Entity.Description || existingEntity.Name != this.Entity.Name)
                    {
                        Entity history = new Entity()
                        {
                            Created = this.Entity.Created,
                            Guid = this.Entity.Guid,
                            Name = existingEntity.Name,
                            Description = existingEntity.Description,
                            Type = EntityType.HISTORY
                        };

                        //TODO: Create the EntityType.HISTORY version
                        //For now I'll just overwrite the existing data with the changes
                        existingEntity.Description = this.Entity.Description;
                        existingEntity.Name = this.Entity.Name;
                    }

                    List<EntityProperty> existingProperties = db.EntityProperties.Where(r => r.Entity == existingEntity).ToList();
                    foreach (var prop in this.Properties)
                    {
                        var thisProp = existingProperties.FirstOrDefault(r => r.Guid == prop.Guid);
                        if (thisProp == null)
                        {
                            EntityProperty newProp = new EntityProperty()
                            {
                                Created = DateTime.UtcNow,
                                Guid = Guid.NewGuid(),
                                Entity = existingEntity,
                                Key = prop.Key,
                                Value = prop.Value,
                                Type = EntityType.ACTIVE
                            };
                            db.EntityProperties.Add(newProp);
                        }
                        else
                        {
                            thisProp.Value = prop.Value;
                            thisProp.Key = prop.Key;
                            thisProp.Type = EntityType.ACTIVE;
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //Logger.Error("EntityDetailViewModel", "Update", e);
            }
        }
    }
}