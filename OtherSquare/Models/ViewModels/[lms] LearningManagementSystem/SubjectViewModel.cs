using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OtherSquare.Models;

namespace OtherSquare.ViewModels
{
    public class SubjectViewModel
    {
        #region Properties

        private NAV_FlashcardSettings _settings { get; set; }
        private NAV_FlashcardSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    this._settings = new NAV_FlashcardSettings();
                }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        private List<ListItemViewModel> _subjectList { get; set; }
        public List<ListItemViewModel> SubjectList
        {
            get
            {
                if (_subjectList == null)
                {
                    _subjectList = new List<ListItemViewModel>();
                }
                return _subjectList;
            }
            set
            {
                _subjectList = value;
            }
        }
        
        //private InputValidation _title_Validation { get; set; }
        //public InputValidation Title_Validation
        //{
        //    get
        //    {
        //        if (_title_Validation == null)
        //        {
        //            _title_Validation = InputValidation.Empty();
        //        }
        //        return _title_Validation;
        //    }
        //    set
        //    {
        //        _title_Validation = value;
        //    }
        //}

        #endregion

        #region Constructor

        public SubjectViewModel(NAV_FlashcardSettings settings = null)
        {
            try
            {
                this.Settings = settings;
                if (this.Settings.SelectedSubject == null)
                {
                    this.Settings.SelectedSubject = new Subject();
                }
                this.buildSubjectList();
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        #endregion

        #region Methods

        public SubjectViewModel ArchiveSubjects(List<Guid> subjectGuids, NAV_FlashcardSettings settings = null)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Subject> selectedSubjects = db.Subjects.Where(s => subjectGuids.Contains(s.SubjectGuid)).ToList();
                    foreach (Subject s in selectedSubjects)
                    {
                        s.IsArchived = true;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log Error
                throw;
            }
            return new SubjectViewModel(settings);
        }

        public SubjectViewModel UnArchiveSubjects(List<Guid> subjectGuids, NAV_FlashcardSettings settings = null)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Subject> selectedSubjects = db.Subjects.Where(s => subjectGuids.Contains(s.SubjectGuid)).ToList();
                    foreach (Subject s in selectedSubjects)
                    {
                        s.IsArchived = false;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log Error
                throw;
            }
            return new SubjectViewModel(settings);
        }

        private void buildSubjectList()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Subject> subjects = new List<Subject>();
                    if ((bool)this.Settings.IncludeArchive)
                    {
                        subjects = db.Subjects.ToList();
                    }
                    else
                    {
                        subjects = db.Subjects.Where(s => s.IsArchived == false).ToList();
                    }

                    foreach (Subject s in subjects)
                    {
                        int divisor = s.Categories.Sum(c => c.Flashcards.Sum(f => f.FlashcardAnswers.Count()));
                        int dividend = s.Categories.Sum(c => c.Flashcards.Sum(f => f.FlashcardAnswers.Sum(fa => fa.IsCorrect ? 1 : 0)));
                        bool scoreIsNa = true;
                        int score = 0;
                        if (divisor > 0 && dividend == 0)
                        {
                            scoreIsNa = false;
                        }
                        else if(divisor > 0 && dividend > 0)
                        {
                            scoreIsNa = false;
                            score = dividend / divisor;
                        }

                        ListItemViewModel li = new ListItemViewModel()
                        {
                            SelectedObject = s,
                            Guid = s.SubjectGuid,
                            Title = s.Title,
                            ScoreIsNA = scoreIsNa,
                            Score = score,
                            IsArchived = s.IsArchived,
                            Selected = s.IsSelected
                        };
                        this.SubjectList.Add(li);
                        if (s.SubjectGuid == this.Settings.SelectedSubject.SubjectGuid)
                        {
                            this.Settings.SelectedSubject = s;
                        }
                    }
                }
            }
            catch
            {
                //TODO Log Error
                throw;
            }
        }

        public static InputValidation SaveSubject(Subject subject)
        {
            if (subject.IsValid())
            {
                if (subject.IsNew())
                {
                    if (subject.TitleExistsInDatabase())
                    {
                        InputValidation val = InputValidation.Alert("A subject with that title already exitst. (It could be archived)");
                        val.Object = subject;
                        return val;
                    }
                    else
                    {
                        subject.Create();
                        InputValidation val = InputValidation.Empty();
                        val.Object = subject;
                        return val;
                    }
                }
                else
                {
                    subject.Update();
                    InputValidation val = InputValidation.Empty();
                    val.Object = subject;
                    return val;
                }
            }
            else
            {
                InputValidation val = InputValidation.Alert(subject.ValidationErrors());
                val.Object = subject;
                return val;
            }
        }

        #endregion
    }
}