using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OtherSquare.Models
{
    public class OtherSquareDbContext : IdentityDbContext<ApplicationUser>
    {
        public OtherSquareDbContext()
            : base("OtherSquareConnection", throwIfV1Schema: false)
        {
        }

        public static OtherSquareDbContext Create()
        {
            return new OtherSquareDbContext();
        }

        //[acct] Accounts
        public DbSet<Entity> Entities { get; set; }
        public DbSet<EntityProperty> EntityProperties { get; set; }
    }
}