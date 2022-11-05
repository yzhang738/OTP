using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class ActionItemType
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItemType_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItemType_Title_Source")]
        public string Source { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ActionItemType_Title_Category")]
        public string Category { get; set; }
    }
}
