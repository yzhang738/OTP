using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class Evaluation
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_VersionId")]
        public int VersionId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_VersionYearId")]
        public string VersionYearId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_VersionName")]
        public string VersionName { get; set; }

        public string VersionPhase { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_SportId")]
        public string SportId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_SportDesc")]
        public string SportDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_Phase")]
        public string Phase { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_PhaseDesc")]
        public string PhaseDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Evaluation_Title_Comment")]
        public string Comment { get; set; }

        public IEnumerable<EvaluationPillar> Pillars { get; set; }
    }
}
