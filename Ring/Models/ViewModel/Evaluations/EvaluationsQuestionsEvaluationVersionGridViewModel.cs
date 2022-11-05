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
    [MetadataType(typeof(EvaluationsQuestionsEvaluationVersionGridViewModel))]
    [KnownType(typeof(EvaluationsQuestionsEvaluationVersionGridViewModel))]
    public class EvaluationsQuestionsEvaluationVersionGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_ID")]
        public int EvaluationVersionId { get; set; }

        [UIHint("EvaluationsQuestionsVersionOrganizationTypeTemplate")]
        [Required(ErrorMessageResourceType = typeof(EvaluationsLocalization), ErrorMessageResourceName = "Error_Required_OrganizationTypeId")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_OrganizationTypeId")]
        public string OrganizationTypeId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_YearId")]
        public string YearId { get; set; }

        [UIHint("EvaluationsQuestionsVersionIDTemplate")]
        [Required(ErrorMessageResourceType = typeof(EvaluationsLocalization), ErrorMessageResourceName = "Error_Required_VersionId")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_VersionId")]
        public string VersionId { get; set; }

        [UIHint("EvaluationsQuestionsVersionPhaseTemplate")]
        [Required(ErrorMessageResourceType = typeof(EvaluationsLocalization), ErrorMessageResourceName = "Error_Required_VersionPhase")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_VersionPhase")]
        public string VersionPhase { get; set; }

        //[UIHint("EvaluationsQuestionsVersionNameEnTemplate")]
        [Required(ErrorMessageResourceType = typeof(EvaluationsLocalization), ErrorMessageResourceName = "Error_Required_Name_EN")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_Name_EN")]
        public string Name_EN { get; set; }

        //[UIHint("EvaluationsQuestionsVersionNameFrTemplate")]
        [Required(ErrorMessageResourceType = typeof(EvaluationsLocalization), ErrorMessageResourceName = "Error_Required_Name_FR")]
        [LocalizedDisplayName(typeof(EvaluationsLocalization), "Title_Name_FR")]
        public string Name_FR { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

        public string Name { get; set; }

    }
}