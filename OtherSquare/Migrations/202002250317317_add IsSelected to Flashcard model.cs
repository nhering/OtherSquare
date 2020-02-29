namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsSelectedtoFlashcardmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("lms.FlashCard", "IsSelected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("lms.FlashCard", "IsSelected");
        }
    }
}
