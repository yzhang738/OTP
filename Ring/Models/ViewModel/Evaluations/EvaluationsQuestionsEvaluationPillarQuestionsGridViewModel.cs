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
    [MetadataType(typeof(EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel))]
    [KnownType(typeof(EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel))]
    public class EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_ID")]
        public int EvaluationVersionQuestionId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationVersionId")]
        public int EvaluationVersionId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationMasterPillarId")]
        public string EvaluationMasterPillarId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationMasterQuestionId")]
        public int EvaluationMasterQuestionId { get; set; }

        [Required(ErrorMessageResourceType=typeof(EvaluationsLocalization), ErrorMessageResourceName="Error_Required_VersionQuestion")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_VersionQuestion")]
        [DataType(DataType.MultilineText)]
        public string VersionQuestion { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_SortOrder")]
        public int SortOrder { get; set; }

        [UIHint("EvaluationsQuestionsQuestionPhaseTemplate")]
        [Required(ErrorMessageResourceType = typeof(EvaluationsLocalization), ErrorMessageResourceName = "Error_Required_QuestionPhase")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_QuestionPhase")]
        public string QuestionPhase { get; set; }

        [UIHint("EvaluationsQuestionsQuestionSportTemplate")]
        [Required(ErrorMessageResourceType = typeof(EvaluationsLocalization), ErrorMessageResourceName = "Error_Required_SportId")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_SportId")]
        public string SportId { get; set; }

        public string SportName { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

        [ReadOnly(true)]
        public bool Editable { get; set; }
    }
}