using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class MaintenanceAthletesViewModel : BaseViewModel
    {
        public bool Editable { get; set; }
        public YearSelectionViewModel CurrentOlympicYears { get; set; }
    }
}