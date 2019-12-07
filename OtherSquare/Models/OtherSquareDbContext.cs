using System.Data.Entity;
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

        //[study] Flash Cards
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subject_Category> Subject_Categories { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }
    }
}