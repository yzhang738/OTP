using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class BaseViewModel
    {
        public NavMenuViewModel Menu { get; set; }
        public string CurrentUserName { get; set; }
        public string CurrentLanguageCode { get; set; }
        public string CurrentSport { get; set; }
        public string CurrentYear { get; set; }

        public BaseViewModel(
            NavMenuViewModel menu = null,
            string currentLanguageCode = null,
            string currentUserName = null)
        {
            Menu = menu;
            CurrentLanguageCode = currentLanguageCode;
            CurrentUserName = currentUserName;
            
        }
    }
}