using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class FundingAllocationViewModel
    {
        public FundingAllocation FundingAllocation { get; set; }

        public string actionName { get; set; }
        public int fundingId { get; set; }
    }
}
