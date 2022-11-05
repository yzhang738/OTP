using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class ReportingIndexViewModel : BaseViewModel
    {
        public List<SummaryListViewModel> SummaryPageList { get; set; }
    }
}