namespace OtherSquare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeLMS : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("lms.Chapter", "CourseGuid", "lms.Course");
            DropForeignKey("lms.Lesson", "ChapterGuid", "lms.Chapter");
            DropForeignKey("lms.Question", "LessonGuid", "lms.Lesson");
            DropIndex("lms.Chapter", new[] { "CourseGuid" });
            DropIndex("lms.Lesson", new[] { "ChapterGuid" });
            DropIndex("lms.Question", new[] { "LessonGuid" });
            DropTable("lms.Chapter");
            DropTable("lms.Course");
            DropTable("lms.Lesson");
            DropTable("lms.Question");
            DropTable("lms.ChapterTag");
            DropTable("lms.CourseTag");
            DropTable("lms.LessonTag");
            DropTable("lms.QuestionTag");
            DropTable("lms.StudyResult");
            DropTable("lms.Tag");
        }
        
        public override void Down()
        {
            CreateTable(
                "lms.Tag",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
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
                "lms.QuestionTag",
                c => new
                    {
                        QuestionGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuestionGuid, t.TagGuid });
            
            CreateTable(
                "lms.LessonTag",
                c => new
                    {
                        LessonGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LessonGuid, t.TagGuid });
            
            CreateTable(
                "lms.CourseTag",
                c => new
                    {
                        CourseGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseGuid, t.TagGuid });
            
            CreateTable(
                "lms.ChapterTag",
                c => new
                    {
                        ChapterGuid = c.Guid(nullable: false),
                        TagGuid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChapterGuid, t.TagGuid });
            
            CreateTable(
                "lms.Question",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        QuestionText = c.String(),
                        AnswerText = c.String(),
                        LessonGuid = c.Guid(nullable: false),
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
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "lms.Course",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "lms.Chapter",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Title = c.String(),
                        CourseGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid);
            
            CreateIndex("lms.Question", "LessonGuid");
            CreateIndex("lms.Lesson", "ChapterGuid");
            CreateIndex("lms.Chapter", "CourseGuid");
            AddForeignKey("lms.Question", "LessonGuid", "lms.Lesson", "Guid", cascadeDelete: true);
            AddForeignKey("lms.Lesson", "ChapterGuid", "lms.Chapter", "Guid", cascadeDelete: true);
            AddForeignKey("lms.Chapter", "CourseGuid", "lms.Course", "Guid", cascadeDelete: true);
        }
    }
}
