using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class ResultsManageTeamViewModel
    {
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SportId")]
        public string SportId { get; set; } //for individual sport, it is on sport level. for other types of sports, it is on event level

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SportDesc")]
        public string SportDesc { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_CountryCode")]
        public string CountryCode { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_CountryDesc")]
        public string CountryDesc { get; set; }
    }
}
