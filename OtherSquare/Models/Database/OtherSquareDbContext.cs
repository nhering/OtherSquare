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

        //[dbo] AspNetIdentity
        public DbSet<UserSetting> UserSettings { get; set; }

        //[lms] LearningManagementSystem
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }
        public DbSet<FlashCardAnswer> FlashCardAnswers { get; set; }
    }
}