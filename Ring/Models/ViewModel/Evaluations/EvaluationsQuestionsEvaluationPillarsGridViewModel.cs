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
    [MetadataType(typeof(EvaluationsQuestionsEvaluationPillarsGridViewModel))]
    [KnownType(typeof(EvaluationsQuestionsEvaluationPillarsGridViewModel))]
    public class EvaluationsQuestionsEvaluationPillarsGridViewModel
    {
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_ID")]
        public string EvaluationMasterPillarId { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationMasterPillarName")]
        public string EvaluationMasterPillarName { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_EvaluationVersionId")]
        public int EvaluationVersionId { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_NumberOfQuestions")]
        public int NumberOfQuestions { get; set; }
    }
}