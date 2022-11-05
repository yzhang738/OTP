using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class FundingBlockType
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlockType_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlockType_Title_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlockType_Title_Comment")]
        public string Comment { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "FundingBlockType_Title_SortOrder")]
        public int SortOrder { get; set; }
    }
}
