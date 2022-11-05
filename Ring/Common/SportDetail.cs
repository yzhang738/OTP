using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class SportDetail
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "SportDetail_Title_Id")]
        public string Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "SportDetail_Title_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "SportDetail_Title_Level")]
        public int Level { get; set; }
    }

}
