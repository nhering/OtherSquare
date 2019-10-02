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
                    this.SearchResults = db.Entities.Where(r => r.Name.Contains(searchString)
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

    public class EntityViewModel
    {
        #region Properties

        public Entity Entity { get; set; }
        public List<Entity> History { get; set; }
        public List<EntityProperty> Properties { get; set; }
        public bool IsDeleted { get; set; }

        #endregion

        #region Constructors

        public EntityViewModel()
        {
            this.Entity = new Entity();
            this.History = new List<Entity>();
            this.Properties = new List<EntityProperty>();
            this.IsDeleted = false;
        }

        public EntityViewModel(Guid entityGuid)
        {
            this.Entity = new Entity();
            this.History = new List<Entity>();
            this.Properties = new List<EntityProperty>();
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    this.History = db.Entities.Where(r => r.Guid == entityGuid).ToList();
                    this.Entity = this.History.OrderBy(r => r.Created).Last();
                    this.IsDeleted = this.Entity.Type == EntityType.DELETED;
                    this.Properties = db.EntityProperties.Where(r => r.EntityGuid == entityGuid
                                                          && r.Type == EntityType.ACTIVE).ToList();
                }
            }
            catch (Exception e)
            {
                //Logger.Error("EntityDetailViewModel", "EntitiyDetailViewModel(int entityCreated)", e);
            }
        }

        #endregion

        public bool HasHistory(Guid entityGuid)
        {
            List<Entity> result = new List<Entity>();

            using (OtherSquareDbContext db = new OtherSquareDbContext())
            {
                result = db.Entities.Where(r => r.Guid == entityGuid).ToList();
            }

            return result.Count > 1;
        }

        public bool IsValid()
        {
            return this.Entity != null;
        }

        private void Create()
        {
            try
            {
                if (this.IsValid())
                {
                    using (OtherSquareDbContext db = new OtherSquareDbContext())
                    {
                        this.Entity.Created = DateTime.UtcNow;
                        this.Entity.Guid = Guid.NewGuid();
                        this.Entity.Type = EntityType.ACTIVE;
                        db.Entities.Add(this.Entity);

                        if (this.Properties != null)
                        {
                            this.Entity.Properties = this.Properties;
                            foreach (var prop in this.Properties)
                            {
                                prop.Guid = Guid.NewGuid();
                                prop.Created = DateTime.UtcNow;
                                prop.Entity = this.Entity;
                                prop.Value = prop.Value; //TODO Encrypt
                                prop.Type = EntityType.ACTIVE;
                                db.EntityProperties.Add(prop);
                            }
                        }
                        db.SaveChanges();
                    }
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
                    #region Update the entity

                    if (this.Entity.Guid == Guid.Empty || this.Entity.Guid == null )
                    {
                        this.Create();
                        return;
                    }

                    this.History = new List<Entity>();
                    this.History = db.Entities.Where(r => r.Guid == this.Entity.Guid).OrderBy(r => r.Created).ToList();
                    if (this.History.Count == 0)
                    {
                        //Shouldn't hit this.
                        //There should be at least be one record matching this.Entity.Guid.
                        //Some kind of error has occured.
                    }
                    
                    //TODO Confirm this is getting the youngest not the oldest record.
                    Entity currentEntity = this.History.OrderBy(r => r.Created).Last();
                    this.IsDeleted = currentEntity.Type == EntityType.DELETED;

                    if (this.Entity.Name == "" || this.Entity.Name == null)
                    {
                        currentEntity.Type = EntityType.HISTORY;
                        Entity deletedEntity = new Entity()
                        {
                            Created = DateTime.UtcNow,
                            Guid = currentEntity.Guid,
                            Name = currentEntity.Name,
                            Description = currentEntity.Description,
                            Type = EntityType.DELETED
                        };
                        db.Entities.Add(deletedEntity);
                        db.SaveChanges();
                        return;
                    }
                    else if (currentEntity.Description != this.Entity.Description || currentEntity.Name != this.Entity.Name)
                    {
                        currentEntity.Type = EntityType.HISTORY;
                        Entity updatedEntity = new Entity()
                        {
                            Created = DateTime.UtcNow,
                            Guid = this.Entity.Guid,
                            Name = this.Entity.Name,
                            Description = this.Entity.Description,
                            Type = EntityType.ACTIVE
                        };
                        db.Entities.Add(updatedEntity);
                    }

                    #endregion

                    #region Update each property

                    //TODO Add an EntityProperty viewModel to handle history and isDeleted etc.
                    //For now we'll just find the most recent marked as active and go from there.
                    List<EntityProperty> existingProperties = new List<EntityProperty>();
                    existingProperties = db.EntityProperties
                        .Where(r => r.EntityGuid == currentEntity.Guid
                        && r.Type == EntityType.ACTIVE).ToList();
                    if (this.Properties == null)
                    {
                        this.Properties = new List<EntityProperty>();
                    }
                    foreach (var prop in this.Properties)
                    {
                        if (prop.Guid != null && prop.Guid != Guid.Empty)
                        {
                            var currentProp = existingProperties.FirstOrDefault(r => r.Guid == prop.Guid && r.Type == EntityType.ACTIVE);

                            if (currentProp == null)
                            {
                                //There was an error.
                                continue;
                            }
                            else if (prop.Key == "" || prop.Key == null)
                            {
                                currentProp.Type = EntityType.HISTORY;
                                EntityProperty deletedProp = new EntityProperty()
                                {
                                    Created = DateTime.UtcNow,
                                    Guid = currentProp.Guid,
                                    Entity = currentEntity,
                                    Key = currentProp.Key,
                                    Value = currentProp.Value,
                                    Type = EntityType.DELETED
                                };
                                db.EntityProperties.Add(deletedProp);
                                continue;
                            }
                            else if (prop.Key != currentProp.Key || prop.Value != currentProp.Value)
                            {
                                currentProp.Type = EntityType.HISTORY;

                                EntityProperty updatedProp = new EntityProperty()
                                {
                                    Created = DateTime.UtcNow,
                                    Guid = currentProp.Guid,
                                    Entity = currentEntity,
                                    Key = prop.Key,
                                    Value = prop.Value,
                                    Type = EntityType.ACTIVE
                                };
                                db.EntityProperties.Add(updatedProp);
                            }
                        }
                        else
                        {
                            if (prop.Key != null && prop.Key != "")
                            {
                                EntityProperty newProp = new EntityProperty()
                                {
                                    Created = DateTime.UtcNow,
                                    Guid = Guid.NewGuid(),
                                    Entity = currentEntity,
                                    Key = prop.Key,
                                    Value = prop.Value,
                                    Type = EntityType.ACTIVE
                                };
                                db.EntityProperties.Add(newProp);
                            }
                        }
                    }

                    #endregion
                    
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