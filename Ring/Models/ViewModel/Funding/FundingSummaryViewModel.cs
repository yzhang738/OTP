using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class FundingSummaryViewModel : BaseViewModel
    {
        public IEnumerable<FundingSummary> FundingSummaries { get; set; }

        public bool Editable { get; set; }

        public string VerifiedSelectedSport { get; set; }
        public string VerifiedSelectedSportDesc { get; set; }
        public YearSelectionViewModel CurrentOlympicYears { get; set; }

    }
}
