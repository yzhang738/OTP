using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingCategory : FundingBase 
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingCategory_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingCategory_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingCategory_Title_TypeDesc")]
        public string TypeDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingCategory_Title_Comment")]
        public string Comment { get; set; }
        public bool HasComment { get; set; }

        public List<FundingDetail> FundingDetails { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingCategory_Title_ResourceRequired")]
        public bool ResourceRequired { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingCategory_Title_ResourceType")]
        public string ResourceType { get; set; }
    }
}
