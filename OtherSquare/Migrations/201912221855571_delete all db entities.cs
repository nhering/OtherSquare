namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletealldbentities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("acct.EntityProperty", new[] { "EntityGuid", "EntityCreated" }, "acct.Entity");
            DropForeignKey("study.FlashCard", "CategoryGuid", "study.Category");
            DropForeignKey("study.Subject_Category", "CategoryGuid", "study.Category");
            DropForeignKey("study.FlashCard", "SubjectGuid", "study.Subject");
            DropForeignKey("study.Subject_Category", "SubjectGuid", "study.Subject");
            DropIndex("acct.EntityProperty", new[] { "EntityGuid", "EntityCreated" });
            DropIndex("study.FlashCard", new[] { "SubjectGuid" });
            DropIndex("study.FlashCard", new[] { "CategoryGuid" });
            DropIndex("study.Subject_Category", new[] { "SubjectGuid" });
            DropIndex("study.Subject_Category", new[] { "CategoryGuid" });
            DropTable("acct.Entity");
            DropTable("acct.EntityProperty");
            DropTable("study.FlashCard");
            DropTable("study.Category");
            DropTable("study.Subject_Category");
            DropTable("study.Subject");
        }
        
        public override void Down()
        {
            CreateTable(
                "study.Subject",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "study.Subject_Category",
                c => new
                    {
                        SubjectGuid = c.Guid(nullable: false),
                        CategoryGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubjectGuid, t.CategoryGuid });
            
            CreateTable(
                "study.Category",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "study.FlashCard",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        Question = c.String(),
                        Answer = c.String(),
                        CorrectAnswers = c.Int(nullable: false),
                        IncorrectAnswers = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        SubjectGuid = c.Guid(nullable: false),
                        CategoryGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "acct.EntityProperty",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                        EntityGuid = c.Guid(nullable: false),
                        EntityCreated = c.DateTime(nullable: false),
                        Key = c.String(nullable: false),
                        Value = c.String(),
                        Type = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.Guid, t.Created });
            
            CreateTable(
                "acct.Entity",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Type = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.Guid, t.Created });
            
            CreateIndex("study.Subject_Category", "CategoryGuid");
            CreateIndex("study.Subject_Category", "SubjectGuid");
            CreateIndex("study.FlashCard", "CategoryGuid");
            CreateIndex("study.FlashCard", "SubjectGuid");
            CreateIndex("acct.EntityProperty", new[] { "EntityGuid", "EntityCreated" });
            AddForeignKey("study.Subject_Category", "SubjectGuid", "study.Subject", "Guid", cascadeDelete: true);
            AddForeignKey("study.FlashCard", "SubjectGuid", "study.Subject", "Guid", cascadeDelete: true);
            AddForeignKey("study.Subject_Category", "CategoryGuid", "study.Category", "Guid", cascadeDelete: true);
            AddForeignKey("study.FlashCard", "CategoryGuid", "study.Category", "Guid", cascadeDelete: true);
            AddForeignKey("acct.EntityProperty", new[] { "EntityGuid", "EntityCreated" }, "acct.Entity", new[] { "Guid", "Created" }, cascadeDelete: true);
        }
    }
}
