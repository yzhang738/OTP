using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingBlock : FundingBase 
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlock_Title_Id")]
        public int Id { get; set; }

        [DisplayName("Funding Block")]
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlock_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlock_Title_TypeDesc")]
        public string TypeDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlock_Title_Comment")]
        public string Comment { get; set; }
        public bool HasComment { get; set; }
        
        public List<FundingCategory> FundingCategories { get; set; }
    }
}
