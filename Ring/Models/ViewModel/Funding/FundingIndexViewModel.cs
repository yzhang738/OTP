using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class FundingIndexViewModel : BaseViewModel
    {
        public List<SummaryListViewModel> SummaryPageList { get; set; }
    }
}