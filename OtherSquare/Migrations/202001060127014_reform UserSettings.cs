namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reformUserSettings : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserSetting");
            AddColumn("dbo.UserSetting", "TimeStamp", c => c.Int(nullable: false));
            AddColumn("dbo.UserSetting", "RedirectURL", c => c.String());
            AddPrimaryKey("dbo.UserSetting", new[] { "UserId", "TimeStamp" });
            DropColumn("dbo.UserSetting", "SiteMap");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserSetting", "SiteMap", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.UserSetting");
            DropColumn("dbo.UserSetting", "RedirectURL");
            DropColumn("dbo.UserSetting", "TimeStamp");
            AddPrimaryKey("dbo.UserSetting", new[] { "UserId", "SiteMap" });
        }
    }
}
