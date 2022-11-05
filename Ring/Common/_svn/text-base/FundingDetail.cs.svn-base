using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingDetail : FundingBase 
    {
        public class Comparer : IComparer<FundingDetail>
        {
            public int Compare(FundingDetail x, FundingDetail y)
            {
                return y.Id.CompareTo(x.Id);
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_Id")]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "FundingDetail_Required_Description")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_Description")]
        public string Description { get; set; }

        [UIHint("FundingCsc")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_CscId")]
        public string CscId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_CscDesc")]
        public string CscDesc { get; set; }

        [UIHint("FundingResource")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_ResourceId")]
        public int? ResourceId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_ResourceDesc")]
        public string ResourceDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_CommittedPercent")]
        public decimal? CommittedPercent { get; set; }

        [UIHint("FundingPriority")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_PriorityCode")]
        public string PriorityCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_PriorityDesc")]
        public string PriorityDesc { get; set; }

        [UIHint("FundingRecurring")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_Recurring")]
        public bool Recurring { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingDetail_Title_Comment")]
        public string Comment { get; set; }
        public bool HasComment { get; set; }
    }
}
