using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class MaintenanceISTMembersViewModel : BaseViewModel
    {
        public bool Editable { get; set; }
        public YearSelectionViewModel CurrentOlympicYears { get; set; }
    }
}