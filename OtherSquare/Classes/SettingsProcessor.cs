using OtherSquare.Models;
using System;
using System.Linq;

namespace OtherSquare.Classes
{
    ////TODO Move this logic to the UserSettings model
    //public static class SetttingsProcessor
    //{
    //    public static bool SaveSettings(UserSetting settings)
    //    {
    //        bool result = false;
    //        try
    //        {
    //            using (OtherSquareDbContext db = new OtherSquareDbContext())
    //            {
    //                var dbSetting = db.UserSettings.FirstOrDefault(s => s.UserId == settings.UserId && s.RedirectURL == settings.RedirectURL);
    //                if (dbSetting != null)
    //                {
    //                    dbSetting.TimeStamp = settings.TimeStamp;
    //                    dbSetting.SettingsJSON = settings.SettingsJSON;
    //                    db.SaveChanges();
    //                }
    //                else
    //                {
    //                    UserSetting newSetting = new UserSetting() {
    //                        UserId = settings.UserId,
    //                        TimeStamp = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds(),
    //                        RedirectURL = settings.RedirectURL,
    //                        SettingsJSON = settings.SettingsJSON
    //                    };
    //                    db.UserSettings.Add(dbSetting);
    //                    db.SaveChanges();
    //                }
    //            }
    //            result = true;
    //        }
    //        catch (Exception e)
    //        {
    //            //TODO Log exception
    //            throw;
    //        }
    //        return result;
    //    }

    //    public static string GetLastRedirectURL(string userId)
    //    {
    //        try
    //        {
    //            using (OtherSquareDbContext db = new OtherSquareDbContext())
    //            {
    //                int? maxTimeStamp = db.UserSettings.Where(r => r.UserId == userId).Max(s => s.TimeStamp);
    //                if (maxTimeStamp != null)
    //                {
    //                    return db.UserSettings.FirstOrDefault(s => s.UserId == userId && s.TimeStamp == maxTimeStamp).RedirectURL;
    //                }
    //                else
    //                {
    //                    return "";
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            //TODO Log exception
    //            throw;
    //        }
    //    }

    //    public static UserSetting GetSetting(string userId, string redirect = "")
    //    {
    //        try
    //        {
    //            if (userId == null) return new UserSetting() { RedirectURL = redirect };

    //            using (OtherSquareDbContext db = new OtherSquareDbContext())
    //            {
    //                UserSetting setting = db.UserSettings.FirstOrDefault(s => s.UserId == userId && s.RedirectURL == redirect);
    //                if (setting != null)
    //                {
    //                    return setting;
    //                }
    //                else
    //                {
    //                    setting = new UserSetting(userId, redirect);
    //                    db.UserSettings.Add(setting);
    //                    db.SaveChanges();
    //                    return setting;
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            //TODO Log exception
    //            throw;
    //        }
    //    }
    //}

    //public class SettingsDTO
    //{
    //    public string RedirectURL { get; set; }
    //    public Object Settings { get; set; }
    //}
}