using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using OtherSquare.Models;

namespace OtherSquare.Controllers.API
{
    public class EntitiesController : ApiController
    {
        private OtherSquareDbContext db = new OtherSquareDbContext();

        // GET: api/Entities
        public IQueryable<Entity> GetEntities()
        {
            return db.Entities;
        }

        // GET: api/Entities/5
        [ResponseType(typeof(Entity))]
        public IHttpActionResult GetEntity(Guid id)
        {
            Entity entity = db.Entities.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PUT: api/Entities/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEntity(Guid id, Entity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Guid)
            {
                return BadRequest();
            }

            db.Entry(entity).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Entities
        [ResponseType(typeof(Entity))]
        public IHttpActionResult PostEntity(Entity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entities.Add(entity);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EntityExists(entity.Guid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = entity.Guid }, entity);
        }

        // DELETE: api/Entities/5
        [ResponseType(typeof(Entity))]
        public IHttpActionResult DeleteEntity(Guid id)
        {
            Entity entity = db.Entities.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            db.Entities.Remove(entity);
            db.SaveChanges();

            return Ok(entity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EntityExists(Guid id)
        {
            return db.Entities.Count(e => e.Guid == id) > 0;
        }
    }
}