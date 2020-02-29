using OtherSquare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtherSquare.Models.ViewModels
{
    public class NAV_StudyViewModel
    {
        public NAV_StudySettings Settings { get; set; }
        public List<ListItemViewModel> Subjects { get; set; }
    }

    public class NAV_StudySettings
    {
        public Guid SelectedSubjectGuid { get; set; }
        public List<Guid> SelectedCategoryGuids { get; set; }
        public QuizQuestions QuizQuestions { get; set; }
    }

    public class QuizQuestions
    {
        public Dictionary<Guid, bool> Answered { get; set; }
        public List<Guid> Unanswered { get; set; }
        public Guid Current { get; set; }
    }
}