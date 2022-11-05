using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class ResultsSummaryViewModel : BaseViewModel
    {
        public bool Editable { get; set; }

        public YearSelectionViewModel CurrentOlympicYears { get; set; }
        public string VerifiedSelectedSport { get; set; }
        public string VerifiedSelectedSportDesc { get; set; }

        public List<ListItem> SelectorItems { get; set; }
        public string SelectorValue { get; set; }
    }
}
