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

        public bool IsValid()
        {
            try
            {
                if (this.Title == null)
                {
                    return false;
                }
                if (this.Title.Trim() == "")
                {
                    return false;
                }
                if (this.SubjectGuid == null)
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
                return this.SubjectGuid == Guid.Empty;
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
                    Subject dbSubject = db.Subjects.FirstOrDefault(s => s.Title.ToLower() == this.Title.ToLower());
                    return dbSubject != null;
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
                    Subject newSubject = new Subject(this.Title);
                    db.Subjects.Add(newSubject);
                    db.SaveChanges();
                    this.SubjectGuid = newSubject.SubjectGuid;
                    this.Title = newSubject.Title;
                    this.IsArchived = newSubject.IsArchived;
                    this.IsSelected = newSubject.IsSelected;
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
                    Subject dbSubject = db.Subjects.FirstOrDefault(s => s.SubjectGuid == this.SubjectGuid);
                    dbSubject.Title = this.Title.Trim();
                    dbSubject.IsArchived = this.IsArchived;
                    dbSubject.IsSelected = this.IsSelected;
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }
    }
}