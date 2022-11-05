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
    [MetadataType(typeof(EvaluationsSummaryOverallQuestionsGridViewModel))]
    [KnownType(typeof(EvaluationsSummaryOverallQuestionsGridViewModel))]
    public class EvaluationsSummaryOverallQuestionsGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_ID")]
        public int EvaluationQuestionId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvalutionMasterPillarId")]
        public string EvalutionMasterPillarId { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_VersionQuestion")]
        public string VersionQuestion { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_HPAAssessment")]
        public string HPAAssessment { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationStatusInitial")]
        public string EvaluationStatusInitial { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationStatusCurrent")]
        public string EvaluationStatusCurrent { get; set; }
    }
}