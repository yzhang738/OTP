using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class MaintenanceEditableViewModel
    {
        public bool Editable { get; set; }
        public string CurrentLanguageCode { get; set; }
        public YearSelectionViewModel CurrentOlympicYears { get; set; }
    }
}
