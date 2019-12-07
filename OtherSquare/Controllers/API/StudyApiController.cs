using OtherSquare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace OtherSquare.Controllers.API
{
    public class StudyApiController : ApiController
    {
        private OtherSquareDbContext db = new OtherSquareDbContext();

        [HttpGet]
        [Route("Study/Subject_Update")]
        public IHttpActionResult Subject_Update(Guid guid, string title)
        {
            try
            {
                Subject subject = db.Subjects.Find(guid);
                if (subject == null)
                {
                    subject = new Subject(guid, title);
                    db.Subjects.Add(subject);
                }
                else
                {
                    subject.Title = title;
                }
                db.SaveChanges();

                return Ok(subject);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [Route("Study/Subject_Delete")]
        public IHttpActionResult Subject_Delete(Guid guid)
        {
            try
            {
                Subject subject = db.Subjects.Find(guid);
                if (subject == null)
                {
                    return NotFound();
                }
                else
                {
                    db.Subjects.Remove(subject);
                }
                db.SaveChanges();

                return Ok();
            }
            catch
            {
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
