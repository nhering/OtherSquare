namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newdatamodels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "lms.Category",
                c => new
                    {
                        CategoryGuid = c.Guid(nullable: false),
                        Title = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                        SubjectGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryGuid)
                .ForeignKey("lms.Subject", t => t.SubjectGuid, cascadeDelete: true)
                .Index(t => t.SubjectGuid);
            
            CreateTable(
                "lms.FlashCard",
                c => new
                    {
                        FlashCardGuid = c.Guid(nullable: false),
                        Title = c.String(),
                        Question = c.String(),
                        Answer = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                        CategoryGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.FlashCardGuid)
                .ForeignKey("lms.Category", t => t.CategoryGuid, cascadeDelete: true)
                .Index(t => t.CategoryGuid);
            
            CreateTable(
                "lms.FlashCardAnswer",
                c => new
                    {
                        TimeStamp = c.DateTime(nullable: false),
                        IsCorrect = c.Boolean(nullable: false),
                        FlashCardGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.TimeStamp)
                .ForeignKey("lms.FlashCard", t => t.FlashCardGuid, cascadeDelete: true)
                .Index(t => t.FlashCardGuid);
            
            CreateTable(
                "lms.Subject",
                c => new
                    {
                        SubjectGuid = c.Guid(nullable: false),
                        Title = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SubjectGuid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("lms.Category", "SubjectGuid", "lms.Subject");
            DropForeignKey("lms.FlashCardAnswer", "FlashCardGuid", "lms.FlashCard");
            DropForeignKey("lms.FlashCard", "CategoryGuid", "lms.Category");
            DropIndex("lms.FlashCardAnswer", new[] { "FlashCardGuid" });
            DropIndex("lms.FlashCard", new[] { "CategoryGuid" });
            DropIndex("lms.Category", new[] { "SubjectGuid" });
            DropTable("lms.Subject");
            DropTable("lms.FlashCardAnswer");
            DropTable("lms.FlashCard");
            DropTable("lms.Category");
        }
    }
}
