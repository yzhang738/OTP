using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class FundingViewModel : BaseViewModel
    {
        public Common.Funding Funding { get; set; }

        public string Title { get; set; }

        public YearSelectionViewModel CurrentOlympicYears { get; set; }

        public bool CanAccessAllocation { get; set; }
        public bool CanAccessAvailableFund { get; set; }
        public bool CanEditRevision { get; set; }
        public bool CanEditActual { get; set; }
        public bool CanEditHpaRecommend { get; set; }
        public bool CanEditOtpRecommend { get; set; }
        public bool CanEditFinalRecommend { get; set; }
        public bool ShrinkMode { get; set; }

        public bool Editable { get; set; }
        public bool Deleteable { get; set; }

        public List<FundingStatusChangeViewModel> ChangeToStatus { get; set; }

        public List<string> ToolbarButtons { get; set; }
    }
}
