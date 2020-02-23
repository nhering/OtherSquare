namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeIsArchivedfromFlashcardmodel : DbMigration
    {
        public override void Up()
        {
            DropColumn("lms.FlashCard", "IsArchived");
        }
        
        public override void Down()
        {
            AddColumn("lms.FlashCard", "IsArchived", c => c.Boolean(nullable: false));
        }
    }
}
