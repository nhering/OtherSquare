using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OtherSquare.ViewModels
{
    /// <summary>
    /// Use this class on a per property basis in a View Model to send feedback to the UI.
    /// </summary>
    public class InputValidation
    {
        public string CSSClass { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }

        public InputValidation(string cssClass = "", string message = "")
        {
            this.CSSClass = cssClass;
            this.Message = message;
        }

        public static InputValidation Alert(string message = "")
        {
            return new InputValidation()
            {
                CSSClass = "alert",
                Message = message
            };
        }

        public static InputValidation Alert(List<string> message)
        {
            string msgHtml = "";
            foreach (string m in message)
            {
                msgHtml += $"{m}";
            }
            return new InputValidation()
            {
                CSSClass = "alert",
                Message = msgHtml
            };
        }

        public static InputValidation Empty(string message = "")
        {
            return new InputValidation()
            {
                CSSClass = "",
                Message = message,
            };
        }

        public static InputValidation Error(string message = "")
        {
            return new InputValidation()
            {
                CSSClass = "error",
                Message = message
            };
        }

        public static InputValidation Success(string message = "")
        {
            return new InputValidation()
            {
                CSSClass = "success",
                Message = message
            };
        }
        
        public static InputValidation Warn(string message = "")
        {
            return new InputValidation()
            {
                CSSClass = "warn",
                Message = message
            };
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
    public class ListItemViewModel
    {
        //public Object SelectedObject { get; set; }
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public bool ScoreIsNA { get; set; }
        public int Score { get; set; }
        public bool IsArchived { get; set; }
        public bool Selected { get; set; }
    }
}