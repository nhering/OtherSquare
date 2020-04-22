using Newtonsoft.Json;
using OtherSquare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtherSquare.ViewModels
{
    public class LMS_Study
    {
        public LMS_Study_Settings Settings { get; set; }
        public List<ListItemViewModel> Subjects { get; set; }
        public List<ListItemViewModel> Categories { get; set; }

        public LMS_Study()
        {
            this.Settings = new LMS_Study_Settings();
            this.Categories = new List<ListItemViewModel>();
            this.Categories = new List<ListItemViewModel>();
        }

        public LMS_Study(UserSetting userSetting)
        {
            LMS_Study_Settings set = null;
            try
            {
                if (string.IsNullOrEmpty(userSetting.SettingsJSON))
                {
                    set = new LMS_Study_Settings();
                    userSetting.SettingsJSON = JsonConvert.SerializeObject(set);
                    userSetting.SaveSettings();
                }
                else
                {
                    set = JsonConvert.DeserializeObject<LMS_Study_Settings>(userSetting.SettingsJSON);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            this.Settings = set;
            this.Subjects = new List<ListItemViewModel>();
            this.Categories = new List<ListItemViewModel>();
            if (!this.Settings.QuizInProgress)
            {
                BuildSubjectList();
                if(this.Settings.SelectedSubjectGuid != Guid.Empty)
                {
                    BuildCategoryList(this.Settings.SelectedSubjectGuid);
                }
            }
        }

        private void BuildSubjectList()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Subject> dbSubjects = db.Subjects.Where(s => s.IsArchived == false).ToList();
                    if (dbSubjects == null)
                    {
                        this.Subjects = new List<ListItemViewModel>();
                    }
                    else if (dbSubjects.Count == 0)
                    {
                        this.Subjects = new List<ListItemViewModel>();
                    }
                    else
                    {
                        //TODO Look into adding a method to the Subject class for calculating it's score.
                        foreach(Subject s in dbSubjects)
                        {
                            int divisor = s.Categories.Sum(c => c.Flashcards.Sum(f => f.FlashcardAnswers.Count()));
                            int dividend = s.Categories.Sum(c => c.Flashcards.Sum(f => f.FlashcardAnswers.Sum(fa => fa.IsCorrect ? 1 : 0)));
                            bool scoreIsNa = true;
                            double score = 0;
                            if (divisor > 0 && dividend == 0)
                            {
                                scoreIsNa = false;
                            }
                            else if (divisor > 0 && dividend > 0)
                            {
                                scoreIsNa = false;
                                score = (double)dividend / divisor;
                            }

                            ListItemViewModel li = new ListItemViewModel()
                            {
                                Guid = s.SubjectGuid,
                                Title = s.Title,
                                ScoreIsNA = scoreIsNa,
                                Score = Math.Floor(score * 100)
                            };
                            this.Subjects.Add(li);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void BuildCategoryList(Guid subjectGuid)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<Category> dbCategories = db.Categories.Where(c => c.IsArchived == false && c.SubjectGuid == subjectGuid).ToList();
                    if (dbCategories == null)
                    {
                        this.Categories = new List<ListItemViewModel>();
                    }
                    else if (dbCategories.Count == 0)
                    {
                        this.Categories = new List<ListItemViewModel>();
                    }
                    else
                    {
                        //TODO Look into adding a method to the Category class for calculating it's score.
                        foreach (Category c in dbCategories)
                        {
                            int divisor = c.Flashcards.Sum(f => f.FlashcardAnswers.Count());
                            int dividend = c.Flashcards.Sum(f => f.FlashcardAnswers.Sum(fa => fa.IsCorrect ? 1 : 0));
                            bool scoreIsNa = true;
                            double score = 0;
                            if (divisor > 0 && dividend == 0)
                            {
                                scoreIsNa = false;
                            }
                            else if (divisor > 0 && dividend > 0)
                            {
                                scoreIsNa = false;
                                score = (double)dividend / divisor;
                            }

                            ListItemViewModel li = new ListItemViewModel()
                            {
                                Guid = c.CategoryGuid,
                                Title = c.Title,
                                ScoreIsNA = scoreIsNa,
                                Score = Math.Floor(score * 100),
                                Count = c.Flashcards.Count
                            };
                            this.Categories.Add(li);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    public class LMS_Study_Settings
    {
        public Guid SelectedSubjectGuid { get; set; }
        public List<Guid> SelectedCategoryGuids { get; set; }
        public bool QuizInProgress { get; set; }
        public FlashcardQuiz FlashcardQuiz { get; set; }
        public bool CurrentAnswerCorrect { get; set; }

        public LMS_Study_Settings()
        {
            this.SelectedSubjectGuid = Guid.Empty;
            this.SelectedCategoryGuids = new List<Guid>();
            this.QuizInProgress = false;
            this.FlashcardQuiz = new FlashcardQuiz();
        }

        public void StartQuiz()
        {
            this.FlashcardQuiz = new FlashcardQuiz();
            this.FlashcardQuiz.StartQuiz(this.SelectedCategoryGuids);
            this.QuizInProgress = true;
            this.SelectedCategoryGuids = new List<Guid>();
        }

        public void AnswerQuestion()
        {
            this.FlashcardQuiz.AnswerQuestion(CurrentAnswerCorrect);
        }
    }

    public class FlashcardQuiz
    {
        public string SubjectTitle { get; set; }
        public string CategoryTitle { get; set; }
        public Dictionary<Guid, bool> Answered { get; set; }
        public double Score { get; set; }
        public List<Guid> Unanswered { get; set; }
        public Guid Current { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

        public FlashcardQuiz()
        {
            this.SubjectTitle = "";
            this.CategoryTitle = "";
            this.Answered = new Dictionary<Guid, bool>();
            this.Score = 0;
            this.Unanswered = new List<Guid>();
            this.Current = Guid.Empty;
            this.Question = "";
            this.Answer = "";
        }

        public void StartQuiz(List<Guid> categoryGuids)
        {
            try
            {
                GetQuestionGuids(categoryGuids);
                this.Current = Guid.Empty;
                CycleToNextQuestion();
                this.Score = 0;
                LookUpNextQuestion();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void GetQuestionGuids(List<Guid> categoryGuids)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    var gui = categoryGuids[0];
                    this.SubjectTitle = db.Categories.FirstOrDefault(c => c.CategoryGuid == gui).Subject.Title;

                    var flashcardGuids = db.FlashCards.Where(f => categoryGuids.Contains(f.CategoryGuid));
                    foreach (var f in flashcardGuids)
                    {
                        this.Unanswered.Add(f.FlashCardGuid);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void AnswerQuestion(bool isCorrect)
        {
            try
            {
                AddFlashcardAnswerToDb(isCorrect);
                CycleToNextQuestion(isCorrect);
                CalculateScore();
                LookUpNextQuestion();
            }
            catch(Exception e)
            {
                throw;
            }
        }

        private void AddFlashcardAnswerToDb(bool isCorrect)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    var flashcard = db.FlashCards.FirstOrDefault(f => f.FlashCardGuid == this.Current);
                    var answer = new FlashCardAnswer(this.Current, isCorrect);
                    flashcard.FlashcardAnswers.Add(answer);

                    //db.FlashCardAnswers.Add(answer);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void CycleToNextQuestion(bool isCorrect = false)
        {
            try
            {
                if (this.Current != Guid.Empty)
                {
                    this.Answered.Add(this.Current, isCorrect);
                }
                if(this.Unanswered.Count > 0)
                {
                    Random rand = new Random();
                    int index = rand.Next(this.Unanswered.Count);
                    this.Current = this.Unanswered[index];
                    this.Unanswered.Remove(this.Current);
                }
                else
                {
                    this.Current = Guid.Empty;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void CalculateScore()
        {
            try
            {
                double score = 0;
                if (this.Answered.Count > 0)
                {
                    int divisor = this.Answered.Count;
                    int dividend = this.Answered.Sum(d => d.Value ? 1 : 0);
                    if (divisor > 0 && dividend > 0)
                    {
                        score = (double)dividend / divisor;
                    }
                }
                this.Score = Math.Floor(score * 100);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void LookUpNextQuestion()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    if(Current != Guid.Empty)
                    {
                        FlashCard currentQuestion = db.FlashCards.FirstOrDefault(f => f.FlashCardGuid == this.Current);
                        this.CategoryTitle = currentQuestion.Category.Title;
                        this.Question = currentQuestion.Question;
                        this.Answer = currentQuestion.Answer;
                    }
                    else
                    {
                        this.CategoryTitle = "";
                        this.Question = "";
                        this.Answer = "";
                    }
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }
    }
}