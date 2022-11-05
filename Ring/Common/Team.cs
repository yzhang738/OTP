using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class Team
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_SportId")]
        public string SportId { get; set; } //for individual sport, it is on sport level. for other types of sports, it is on event level

        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_SportDesc")]
        public string SportDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_CountryCode")]
        public string CountryCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_CountryDesc")]
        public string CountryDesc { get; set; }

        [DisplayName("Team")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Team_Title_Comment")]
        public string Comment { get; set; }
    }
}
