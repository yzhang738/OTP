using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class FundingStatusChangeViewModel
    {
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
    }
}
