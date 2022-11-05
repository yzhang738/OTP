using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class ReportingDesignAnalysisViewModel : BaseViewModel
    {
        public List<ListItem> ReportList { get; set; }
        public bool Editable { get; set; }
    }
}