namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddescriptionpropertytoCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("lms.Course", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("lms.Course", "Description");
        }
    }
}
