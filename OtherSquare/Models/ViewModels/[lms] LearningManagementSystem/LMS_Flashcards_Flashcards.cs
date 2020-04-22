using Newtonsoft.Json;
using OtherSquare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtherSquare.ViewModels
{
    public class LMS_Flashcards_Flashcards
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

        private List<ListItemViewModel> _flashcardList { get; set; }
        public List<ListItemViewModel> FlashcardList
        {
            get
            {
                if (_flashcardList == null)
                {
                    _flashcardList = new List<ListItemViewModel>();
                }
                return _flashcardList;
            }
            set
            {
                _flashcardList = value;
            }
        }

        #endregion

        #region Constructor

        public LMS_Flashcards_Flashcards(LMS_Flashcards_Settings settings = null)
        {
            try
            {
                this.Settings = settings;
                this.buildFlashcardList();
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        #endregion

        #region Methods

        public static void DeleteFlashcards(List<Guid> flashcardGuids)
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<FlashCard> dbFlashcards = db.FlashCards.Where(f => flashcardGuids.Contains(f.FlashCardGuid)).ToList();
                    foreach (FlashCard f in dbFlashcards)
                    {
                        db.FlashCards.Remove(f);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //TODO Log Error
                throw;
            }
        }

        private void buildFlashcardList()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    List<FlashCard> flashcards = new List<FlashCard>();
                    flashcards = db.FlashCards.Where(f => f.CategoryGuid == Settings.SelectedCategoryGuid).ToList();

                    foreach (FlashCard f in flashcards)
                    {
                        int divisor = f.FlashcardAnswers.Count();
                        int dividend = f.FlashcardAnswers.Sum(fa => fa.IsCorrect ? 1 : 0);
                        bool scoreIsNa = true;
                        double score = 0;
                        if (divisor > 0 && dividend == 0)
                        {
                            scoreIsNa = false;
                        }
                        else if (divisor > 0 && dividend > 0)
                        {
                            scoreIsNa = false;
                            score = (double)dividend/divisor;
                        }

                        ListItemViewModel li = new ListItemViewModel()
                        {
                            Guid = f.FlashCardGuid,
                            Title = f.Title,
                            ScoreIsNA = scoreIsNa,
                            Score = Math.Floor(score * 100),
                            IsArchived = false,
                            Selected = f.IsSelected
                        };
                        this.FlashcardList.Add(li);
                        if (f.FlashCardGuid == this.Settings.SelectedFlashcardGuid)
                        {
                            this.Settings.SelectedFlashcardTitle = f.Title;
                            this.Settings.SelectedFlashcardQuestion = f.Question;
                            this.Settings.SelectedFlashcardAnswer = f.Answer;
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

        public static InputValidation SaveFlashcard(UserSetting userSetting)
        {
            LMS_Flashcards_Settings fSet = null;
            Guid uiCategoryGuid = Guid.Empty;
            Guid uiFlashcardGuid = Guid.Empty;
            string uiTitle = null;
            string uiQuestion = null;
            string uiAnswer = null;
            FlashCard dbFlashcard = null;
            try
            {
                fSet = JsonConvert.DeserializeObject<LMS_Flashcards_Settings>(userSetting.SettingsJSON);
                uiCategoryGuid = fSet.SelectedCategoryGuid;
                if (uiCategoryGuid == Guid.Empty || uiCategoryGuid == null) return InputValidation.Error("Something went wrong. This flashcard has no category. Can not create orphan flashcards.");
                uiFlashcardGuid = fSet.SelectedFlashcardGuid;
                uiTitle = fSet.SelectedFlashcardTitle.Trim();
                uiQuestion = fSet.SelectedFlashcardQuestion;
                uiAnswer = fSet.SelectedFlashcardAnswer;
                if (string.IsNullOrEmpty(uiTitle)) return InputValidation.Error("Title can not be empty.");
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    if (uiFlashcardGuid != Guid.Empty && uiFlashcardGuid != null)
                    {
                        dbFlashcard = db.FlashCards.FirstOrDefault(f => f.FlashCardGuid == uiFlashcardGuid
                                                                  && f.CategoryGuid == uiCategoryGuid);
                        if (dbFlashcard == null)
                        {
                            //Something is wrong. Why didn't we find a record?
                            throw new Exception($"The flashcard with the Guid {uiFlashcardGuid.ToString()} was not found in the database.");
                        }
                        dbFlashcard.Title = uiTitle;
                        dbFlashcard.Question = uiQuestion;
                        dbFlashcard.Answer = uiAnswer;
                        db.SaveChanges();

                        //update the user setting and save it
                        fSet.SelectedFlashcardTitle = uiTitle;
                        fSet.SelectedFlashcardQuestion = uiQuestion;
                        fSet.SelectedFlashcardAnswer = uiAnswer;
                        userSetting.SettingsJSON = JsonConvert.SerializeObject(fSet);
                        userSetting.SaveSettings();

                        return InputValidation.Success("Flashcard successfully updated.");
                    }
                    else
                    {
                        dbFlashcard = db.FlashCards.FirstOrDefault(f => f.FlashCardGuid == uiFlashcardGuid
                                                                  && f.Title.ToLower() == uiTitle.ToLower());
                        if (dbFlashcard != null)
                        {
                            return InputValidation.Alert($"A flashcard with the title {uiTitle} already exitst. (It could be archived)");
                        }

                        FlashCard newFlashcard = new FlashCard(uiCategoryGuid);
                        newFlashcard.Title = uiTitle;
                        newFlashcard.Question = uiQuestion;
                        newFlashcard.Answer = uiAnswer;
                        db.FlashCards.Add(newFlashcard);
                        db.SaveChanges();

                        //update the user setting and save it
                        fSet.SelectedFlashcardGuid = newFlashcard.FlashCardGuid;
                        fSet.SelectedFlashcardTitle = uiTitle;
                        fSet.SelectedFlashcardQuestion = uiQuestion;
                        fSet.SelectedFlashcardAnswer = uiAnswer;
                        userSetting.SettingsJSON = JsonConvert.SerializeObject(fSet);
                        userSetting.SaveSettings();
                        return InputValidation.Success("Flashcard successfully created.");
                    }

                }
            }
            catch (Exception e)
            {
                //TODO do something with the fSet here?
                return InputValidation.Error(e.Message);
            }
        }
        
        #endregion
    }
}