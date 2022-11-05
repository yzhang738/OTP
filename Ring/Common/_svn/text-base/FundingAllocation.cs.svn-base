using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingAllocation
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_SportId")]
        public string SportId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_YearId")]
        public string YearId { get; set; }

        [UIHint("FundingNumber")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_AvailableFundsSC")]
        public decimal? AvailableFundsSC { get; set; }

        [UIHint("FundingNumber")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_AvailableFundsCoc")]
        public decimal? AvailableFundsCoc { get; set; }

        [UIHint("FundingNumber")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_AvailableFundsOther")]
        public decimal? AvailableFundsOther { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_AllocatedFundsSC")]
        public decimal? AllocatedFundsSC { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_AllocatedFundsCoc")]
        public decimal? AllocatedFundsCoc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_AllocatedFundsOther")]
        public decimal? AllocatedFundsOther { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_UnallocatedFundsSC")]
        public decimal? UnallocatedFundsSC { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_UnallocatedFundsCoc")]
        public decimal? UnallocatedFundsCoc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingAllocation_Title_UnallocatedFundsOther")]
        public decimal? UnallocatedFundsOther { get; set; }

        public List<Funding> Fundings { get; set; }
    }
}
