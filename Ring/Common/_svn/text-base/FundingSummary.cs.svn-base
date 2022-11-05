using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingSummary
    {
        public class Comparer : IComparer<FundingSummary>
        {
            public int Compare(FundingSummary x, FundingSummary y)
            {
                return x.Type.CompareTo(y.Type);
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_TypeDesc")]
        public string TypeDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_OriginalBudget")]
        public decimal? OriginalBudget { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_RequestOtp")]
        public decimal? RequestOtp { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_HpaRecommend")]
        public decimal? HpaRecommend { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_OtpRecommend")]
        public decimal? OtpRecommend { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_FinalRecommend")]
        public decimal? FinalRecommend { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_Revised")]
        public decimal? Revised { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_Paid")]
        public decimal? Paid { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_Actual")]
        public decimal? Actual { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingSummary_Title_RevisedBudget")]
        public decimal? RevisedBudget { get; set; }

        public List<Funding> Fundings { get; set; }
    }
}
