using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class CompetitionEvent
    {
        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_Id")]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "CompetitionEvent_Required_DisciplineId")]
        [UIHint("SummaryDisciplineTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_DisciplineId")]
        public string DisciplineId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_DisciplineDesc")]
        public string DisciplineDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "CompetitionEvent_Required_EventId")]
        [UIHint("SummaryEventTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_EventId")]
        public string EventId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_EventDesc")]
        public string EventDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_FieldSize")]
        public int? FieldSize { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_Cancelled")]
        public bool Cancelled { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_DownloadCount")]
        public int DownloadCount { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "CompetitionEvent_Title_ApproveCount")]
        public int ApproveCount { get; set; }
    }
}
