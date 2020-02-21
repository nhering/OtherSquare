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

        public bool IsValid()
        {
            try
            {
                if (this.Title == null || this.SubjectGuid == null)
                {
                    return false;
                }
                if (this.Title.Trim() == "" || this.SubjectGuid == Guid.Empty)
                {
                    return false;
                }
                if (this.CategoryGuid == null)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        public List<string> ValidationErrors()
        {
            try
            {
                List<string> response = new List<string>();

                if (this.Title == null)
                {
                    response.Add("The Title can not be Null.");
                }
                else if (this.Title.Trim() == "")
                {
                    response.Add("The Title can not be Empty.");
                }
                if (this.SubjectGuid == null)
                {
                    response.Add("The SubjectGuid can not be Null.");
                }
                if (this.SubjectGuid == null)
                {
                    response.Add("The CategoryGuid can not be Null.");
                }

                return response;
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        public bool IsNew()
        {
            try
            {
                return this.CategoryGuid == Guid.Empty;
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool TitleExistsInDatabase()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Category dbCategory = db.Categories.FirstOrDefault(
                                                        c => c.SubjectGuid == this.SubjectGuid 
                                                        && c.Title.ToLower() == this.Title.ToLower());
                    return dbCategory != null;
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        public void Create()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Category newCategory = new Category(this.SubjectGuid, this.Title);
                    db.Categories.Add(newCategory);
                    db.SaveChanges();
                    this.CategoryGuid = newCategory.CategoryGuid;
                    this.Title = newCategory.Title;
                    this.IsArchived = newCategory.IsArchived;
                    this.IsSelected = newCategory.IsSelected;
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        public void Update()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    Category dbCategory = db.Categories.FirstOrDefault(
                                                        c => c.CategoryGuid == this.CategoryGuid
                                                        && c.SubjectGuid == this.SubjectGuid);
                    dbCategory.Title = this.Title.Trim();
                    dbCategory.IsArchived = this.IsArchived;
                    dbCategory.IsSelected = this.IsSelected;
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        public Category Copy()
        {
            return new Category()
            {
                SubjectGuid = this.SubjectGuid,
                CategoryGuid = this.CategoryGuid,
                Title = this.Title,
                IsArchived = this.IsArchived,
                IsSelected = this.IsSelected,
                Subject = null,
                Flashcards = null
            };
        }
    }
}