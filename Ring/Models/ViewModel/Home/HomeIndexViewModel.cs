using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data.Models;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class HomeIndexViewModel : BaseViewModel
    {
        public List<SummaryListViewModel> SummaryPageList { get; set; }
        public bool HasAccess { get; set; }
    }
}