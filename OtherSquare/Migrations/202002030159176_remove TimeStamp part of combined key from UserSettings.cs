namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeTimeStamppartofcombinedkeyfromUserSettings : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserSetting");
            AddPrimaryKey("dbo.UserSetting", "UserId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserSetting");
            AddPrimaryKey("dbo.UserSetting", new[] { "UserId", "TimeStamp" });
        }
    }
}
