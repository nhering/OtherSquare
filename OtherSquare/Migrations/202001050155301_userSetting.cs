namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSetting",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        SiteMap = c.String(nullable: false, maxLength: 128),
                        SettingsJSON = c.String(),
                    })
                .PrimaryKey(t => new { t.UserId, t.SiteMap });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserSetting");
        }
    }
}
