namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "lms.Chapter",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        CourseGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("lms.Course", t => t.CourseGuid, cascadeDelete: true)
                .Index(t => t.CourseGuid);
            
            CreateTable(
                "lms.Course",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "lms.Lesson",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        Content = c.String(),
                        ChapterGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("lms.Chapter", t => t.ChapterGuid, cascadeDelete: true)
                .Index(t => t.ChapterGuid);
            
            CreateTable(
                "lms.Question",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        QuestionText = c.String(),
                        AnswerText = c.String(),
                        LessonGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("lms.Lesson", t => t.LessonGuid, cascadeDelete: true)
                .Index(t => t.LessonGuid);
            
            CreateTable(
                "lms.ChapterTag",
                c => new
                    {
                        ChapterGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChapterGuid, t.TagGuid });
            
            CreateTable(
                "lms.CourseTag",
                c => new
                    {
                        CourseGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseGuid, t.TagGuid });
            
            CreateTable(
                "lms.LessonTag",
                c => new
                    {
                        LessonGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LessonGuid, t.TagGuid });
            
            CreateTable(
                "lms.QuestionTag",
                c => new
                    {
                        QuestionGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuestionGuid, t.TagGuid });
            
            CreateTable(
                "lms.StudyResult",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        QuestionGuid = c.Guid(nullable: false),
                        CorrectCount = c.Int(nullable: false),
                        IncorrectCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.QuestionGuid });
            
            CreateTable(
                "lms.Tag",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("lms.Question", "LessonGuid", "lms.Lesson");
            DropForeignKey("lms.Lesson", "ChapterGuid", "lms.Chapter");
            DropForeignKey("lms.Chapter", "CourseGuid", "lms.Course");
            DropIndex("lms.Question", new[] { "LessonGuid" });
            DropIndex("lms.Lesson", new[] { "ChapterGuid" });
            DropIndex("lms.Chapter", new[] { "CourseGuid" });
            DropTable("lms.Tag");
            DropTable("lms.StudyResult");
            DropTable("lms.QuestionTag");
            DropTable("lms.LessonTag");
            DropTable("lms.CourseTag");
            DropTable("lms.ChapterTag");
            DropTable("lms.Question");
            DropTable("lms.Lesson");
            DropTable("lms.Course");
            DropTable("lms.Chapter");
        }
    }
}
