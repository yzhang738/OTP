using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OTP.Ring.Data.Models;
using System.Collections.Generic;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [MetadataType(typeof(EvaluationsSummaryGridViewModel))]
    [KnownType(typeof(EvaluationsSummaryGridViewModel))]
    public class EvaluationsSummaryGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_ID")]
        public int EvaluationId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_Rank")]
        public string Rank { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_SportName")]
        public string SportName { get; set; }

        public IEnumerable<ListItem> Pillars { get; set; }

        public int OpenActionItems { get; set; }

        public int TotalActionItems { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationPhase")]
        public string EvaluationPhase { get; set; }

        public List<EvaluationsSummaryOverallQuestionsGridViewModel> EvaluationOverallQuestions { get; set; }
    }
}