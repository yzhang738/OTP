using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OTP.Ring.Common
{
    [Serializable]
    public class Funding
    {
        public class Comparer : IComparer<Funding>
        {
            public int Compare(Funding x, Funding y)
            {
                return (x.SportDesc + x.OrganizationDesc).CompareTo(y.SportDesc + y.OrganizationDesc);
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_OrganizationId")]
        public string OrganizationId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_OrganizationDesc")]
        public string OrganizationDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_SportId")]
        public string SportId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_SportDesc")]
        public string SportDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_YearId")]
        public string YearId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_StatusCode")]
        public string StatusCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_StatusDesc")]
        public string StatusDesc { get; set; }

        [UIHint("FundingNumber")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_AvailableFundsSC")]
        public decimal? AvailableFundsSC { get; set; }

        [UIHint("FundingNumber")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_AvailableFundsCoc")]
        public decimal? AvailableFundsCoc { get; set; }

        [UIHint("FundingNumber")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_AvailableFundsOther")]
        public decimal? AvailableFundsOther { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_Comment")]
        public string Comment { get; set; }

        public List<FundingBlock> FundingBlocks { get; set; }

        public List<FundingTotal> FundingTotals { get; set; }

        //For Funding Summary screen
        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_OriginalBudget")]
        public decimal? OriginalBudget { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_RequestOtp")]
        public decimal? RequestOtp { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_HpaRecommend")]
        public decimal? HpaRecommend { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_OtpRecommend")]
        public decimal? OtpRecommend { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_FinalRecommend")]
        public decimal? FinalRecommend { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_RecommendSC")]
        public decimal? RecommendSC { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_RecommendCoc")]
        public decimal? RecommendCoc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_RecommendOther")]
        public decimal? RecommendOther { get; set; } //Calculated

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_Revised")]
        public decimal? Revised { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_Paid")]
        public decimal? Paid { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_Actual")]
        public decimal? Actual { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_RevisedBudget")]
        public decimal? RevisedBudget { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_ActionItem")]
        public string ActionItem { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_ShowHpaRecommendLink")]
        public bool ShowHpaRecommendLink { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_ShowOtpRecommendLink")]
        public bool ShowOtpRecommendLink { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_ShowFinalRecommendLink")]
        public bool ShowFinalRecommendLink { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Funding_Title_ShowRevisedLink")]
        public bool ShowRevisedLink { get; set; }
    }
}
