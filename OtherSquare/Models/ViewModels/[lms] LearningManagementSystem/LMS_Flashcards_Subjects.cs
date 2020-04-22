using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OtherSquare.Models;

namespace OtherSquare.ViewModels
{
    public class LMS_Flashcards_Subjects
    {
        #region Properties

        private LMS_Flashcards_Settings _settings { get; set; }
        private LMS_Flashcards_Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    this._settings = new LMS_Flashcards_Settings();
                }
                return _settings;
            }
            set
            {
                if (value == null)
                {
                    value = new LMS_Flashcards_Settings();
                }
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

        #endregion

        #region Constructor

        public LMS_Flashcards_Subjects(LMS_Flashcards_Settings settings = null)
        {
            try
            {
                this.Settings = settings;
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

        public static void ArchiveSubjects(List<Guid> subjectGuids)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Subject> selectedSubjects = db.Subjects.Where(s => subjectGuids.Contains(s.SubjectGuid)).ToList();
                    foreach (Subject s in selectedSubjects)
                    {
                        s.IsArchived = true;
                        s.IsSelected = false;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log Error
                throw;
            }
        }

        public static void UnArchiveSubjects(List<Guid> subjectGuids)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Subject> selectedSubjects = db.Subjects.Where(s => subjectGuids.Contains(s.SubjectGuid)).ToList();
                    foreach (Subject s in selectedSubjects)
                    {
                        s.IsArchived = false;
                        s.IsSelected = false;
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                //TODO Log Error
                throw;
            }
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
                        double score = 0;
                        if (divisor > 0 && dividend == 0)
                        {
                            scoreIsNa = false;
                        }
                        else if(divisor > 0 && dividend > 0)
                        {
                            scoreIsNa = false;
                            score = (double)dividend / divisor;
                        }

                        ListItemViewModel li = new ListItemViewModel()
                        {
                            Guid = s.SubjectGuid,
                            Title = s.Title,
                            ScoreIsNA = scoreIsNa,
                            Score = Math.Floor(score * 100),
                            IsArchived = s.IsArchived,
                            Selected = s.IsSelected
                        };
                        this.SubjectList.Add(li);
                        if (s.SubjectGuid == this.Settings.SelectedSubjectGuid)
                        {
                            this.Settings.SelectedSubjectTitle = s.Title;
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

        public static InputValidation SaveSubject(UserSetting userSetting)
        {
            LMS_Flashcards_Settings fSet = null;
            Guid uiSubjectGuid = Guid.Empty;
            string uiTitle = null;
            Subject dbSubject = null;
            try
            {
                fSet = JsonConvert.DeserializeObject<LMS_Flashcards_Settings>(userSetting.SettingsJSON);
                uiSubjectGuid = fSet.SelectedSubjectGuid;
                uiTitle = fSet.SelectedSubjectTitle.Trim();
                if (string.IsNullOrEmpty(uiTitle)) return InputValidation.Error("Title can not be empty.");
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    if (uiSubjectGuid != Guid.Empty && uiSubjectGuid != null) //Then there should be a record in the db that we can update
                    {
                        dbSubject = db.Subjects.FirstOrDefault(s => s.SubjectGuid == uiSubjectGuid);
                        if (dbSubject == null)
                        {
                            //Something is wrong. Why didn't we find a record?
                            throw new Exception($"The Subject with the Guid {uiSubjectGuid.ToString()} was not found in the database.");
                        }
                        dbSubject.Title = uiTitle;
                        db.SaveChanges();

                        //update the user setting and save it
                        fSet.SelectedSubjectTitle = uiTitle;
                        userSetting.SettingsJSON = JsonConvert.SerializeObject(fSet);
                        userSetting.SaveSettings();

                        return InputValidation.Success("Subject successfully updated.");
                    }
                    else
                    {
                        dbSubject = db.Subjects.FirstOrDefault(s => s.Title.ToLower() == uiTitle.ToLower());

                        if (dbSubject != null)
                        {
                            return InputValidation.Alert($"A subject with the title {uiTitle} already exitst. (It could be archived)");
                        }

                        Subject newSubject = new Subject(uiTitle);
                        db.Subjects.Add(newSubject);
                        db.SaveChanges();
                        return InputValidation.Success("Subject successfully created.");
                    }
                }
            } 
            catch(Exception e)
            {
                //TODO do something with the fSet here?
                return InputValidation.Error(e.Message);
            }
        }

        #endregion
    }
}