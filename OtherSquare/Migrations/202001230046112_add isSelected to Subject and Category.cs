namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addisSelectedtoSubjectandCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("lms.Category", "IsSelected", c => c.Boolean(nullable: false));
            AddColumn("lms.Subject", "IsSelected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("lms.Subject", "IsSelected");
            DropColumn("lms.Category", "IsSelected");
        }
    }
}
