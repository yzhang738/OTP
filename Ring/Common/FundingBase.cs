using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingBase
    {
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_OriginalBudget")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_OriginalBudget")]
        public decimal? OriginalBudget { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RequestOtp")]
        public decimal? RequestOtp { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RequestNso")]
        public decimal? RequestNso { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RequestSC")]
        public decimal? RequestSC { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RequestOther")]
        public decimal? RequestOther { get; set; }   //Calculated

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_HpaRecommend")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_HpaRecommend")]
        public decimal? HpaRecommend { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_OtpRecommend")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_OtpRecommend")]
        public decimal? OtpRecommend { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_FinalRecommend")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_FinalRecommend")]
        public decimal? FinalRecommend { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_RecommendSC")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RecommendSC")]
        public decimal? RecommendSC { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_RecommendCoc")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RecommendCoc")]
        public decimal? RecommendCoc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RecommendOther")]
        public decimal? RecommendOther { get; set; } //Calculated

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_Revised")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_Revised")]
        public decimal? Revised { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_RevisedSC")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RevisedSC")]
        public decimal? RevisedSC { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_RevisedCoc")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RevisedCoc")]
        public decimal? RevisedCoc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RevisedOther")]
        public decimal? RevisedOther { get; set; }   //Calculated

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_PendingRevise")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_PendingRevise")]
        public decimal? PendingRevise { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_PendingReviseSC")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_PendingReviseSC")]
        public decimal? PendingReviseSC { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_PendingReviseCoc")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_PendingReviseCoc")]
        public decimal? PendingReviseCoc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_PendingReviseOther")]
        public decimal? PendingReviseOther { get; set; }   //Calculated

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingBase_Required_Actual")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_Actual")]
        public decimal? Actual { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_RevisedBudget")]
        public decimal? RevisedBudget { get; set; } //Calculated

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBase_Title_Variance")]
        public decimal? Variance { get; set; }   //Calculated
    }
}
