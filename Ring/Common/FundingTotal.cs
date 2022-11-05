using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingTotal : FundingBase 
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingTotal_Title_Type")]
        public string Type { get; set; }
    }
}
