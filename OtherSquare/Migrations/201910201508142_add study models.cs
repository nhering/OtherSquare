namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addstudymodels : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.Guid)
                .ForeignKey("study.Category", t => t.CategoryGuid, cascadeDelete: true)
                .ForeignKey("study.Subject", t => t.SubjectGuid, cascadeDelete: true)
                .Index(t => t.SubjectGuid)
                .Index(t => t.CategoryGuid);
            
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
                "study.Subject_Category",
                c => new
                    {
                        SubjectGuid = c.Guid(nullable: false),
                        CategoryGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubjectGuid, t.CategoryGuid })
                .ForeignKey("study.Category", t => t.CategoryGuid, cascadeDelete: true)
                .ForeignKey("study.Subject", t => t.SubjectGuid, cascadeDelete: true)
                .Index(t => t.SubjectGuid)
                .Index(t => t.CategoryGuid);
            
            CreateTable(
                "study.Subject",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Guid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("study.Subject_Category", "SubjectGuid", "study.Subject");
            DropForeignKey("study.FlashCard", "SubjectGuid", "study.Subject");
            DropForeignKey("study.Subject_Category", "CategoryGuid", "study.Category");
            DropForeignKey("study.FlashCard", "CategoryGuid", "study.Category");
            DropIndex("study.Subject_Category", new[] { "CategoryGuid" });
            DropIndex("study.Subject_Category", new[] { "SubjectGuid" });
            DropIndex("study.FlashCard", new[] { "CategoryGuid" });
            DropIndex("study.FlashCard", new[] { "SubjectGuid" });
            DropTable("study.Subject");
            DropTable("study.Subject_Category");
            DropTable("study.Category");
            DropTable("study.FlashCard");
        }
    }
}
