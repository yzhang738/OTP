using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data;
using OTP.Ring.Data.Models;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class MaintenanceIndexViewModel : BaseViewModel
    {
        public List<SummaryListViewModel> SummaryPageList { get; set; }
    }
}