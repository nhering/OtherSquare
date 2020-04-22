using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OtherSquare.Models
{
    [Table("UserSetting", Schema = "dbo")]
    public class UserSetting
    {
        [Key]
        public Guid UserSettingGuid { get; set; }
        public string UserId { get; set; }
        public int TimeStamp { get; set; }
        public string RedirectURL { get; set; }
        public string SettingsJSON { get; set; }

        public UserSetting() { }

        public UserSetting(string userId, string redirectURL = "", string settingJSON = "")
        {
            this.UserId = userId;
            this.TimeStamp = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.RedirectURL = redirectURL;
            if (settingJSON == "")
            {
                this.GetSettingsJSON();
            }
            else
            {
                this.SettingsJSON = settingJSON;
            }
        }

        public bool SaveSettings()
        {
            bool result = false;
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    var dbSetting = db.UserSettings.FirstOrDefault(s => s.UserId == this.UserId && s.RedirectURL == this.RedirectURL);
                    if (dbSetting != null)
                    {
                        dbSetting.TimeStamp = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        dbSetting.SettingsJSON = this.SettingsJSON;
                        db.SaveChanges();
                    }
                    else
                    {
                        UserSetting newSetting = new UserSetting()
                        {
                            UserSettingGuid = Guid.NewGuid(),
                            UserId = this.UserId,
                            TimeStamp = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                            RedirectURL = this.RedirectURL,
                            SettingsJSON = this.SettingsJSON
                        };
                        db.UserSettings.Add(newSetting);
                        db.SaveChanges();
                    }
                }
                result = true;
            }
            catch (Exception e)
            {
                //TODO Log exception
                throw;
            }
            return result;
        }

        public string GetLastRedirectURL()
        {
            try
            {
                using (OtherSquareDbContext db = new OtherSquareDbContext())
                {
                    //int? maxTimeStamp = null;
                    try
                    {
                        List<UserSetting> userSettings = db.UserSettings.Where(u => u.UserId == this.UserId).ToList();
                        if(userSettings != null)
                        {
                            if (userSettings.Count > 0)
                            {
                                // DON'T NEED THIS:  userSettings.OrderByDescending(t => t.TimeStamp);
                                // get the record with the max timestamp
                                var result = userSettings.OrderByDescending(o => o.TimeStamp).First().RedirectURL;
                                return result;
                            }
                            else
                            {
                                return "";
                            }
                        }
                        else
                        {
                            return "";
                        }
                    }
                    catch (Exception e)
                    {
                        // The user hasn't visited any pages yet.
                        // Use the default which will be used by the
                        // HomeController to send them to the home page.
                        return "";
                    }
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }

        private void GetSettingsJSON()
        {
            try
            {
                if (this.UserId != null && this.RedirectURL != "")
                {
                    using (OtherSquareDbContext db = new OtherSquareDbContext())
                    {
                        UserSetting setting = db.UserSettings.FirstOrDefault(s => s.UserId == this.UserId && s.RedirectURL == this.RedirectURL);
                        if (setting == null)
                        {
                            setting = new UserSetting()
                            {
                                UserSettingGuid = Guid.NewGuid(),
                                UserId = this.UserId,
                                TimeStamp = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                                RedirectURL = this.RedirectURL,
                                SettingsJSON = "",
                            };
                            db.UserSettings.Add(setting);
                            db.SaveChanges();
                        }
                        else
                        {
                            this.SettingsJSON = setting.SettingsJSON;
                        }
                    }
                }
                else
                {
                    this.SettingsJSON = "";
                }
            }
            catch
            {
                //TODO Log exception
                throw;
            }
        }
    }
}