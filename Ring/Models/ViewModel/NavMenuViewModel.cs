using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class NavMenuViewModel
    {
        public MenuHierarchy Menu { get; set; }
        public bool ShowSportMenu { get; set; }
        public bool ShowPageMenu { get; set; }
    }
}
