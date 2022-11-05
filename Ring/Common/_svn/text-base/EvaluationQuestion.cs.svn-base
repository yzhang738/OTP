using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class EvaluationQuestion
    {
        public class Comparer : IComparer<EvaluationQuestion>
        {
            public int Compare(EvaluationQuestion x, EvaluationQuestion y)
            {
                return x.SortOrder.CompareTo(y.SortOrder);
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_Id")]
        public int Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_VersionQuestionId")]
        public int VersionQuestionId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_VersionId")]
        public int VersionId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_MasterQuestionId")]
        public int MasterQuestionId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_MasterPillarId")]
        public string MasterPillarId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_SportId")]
        public string SportId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_Question")]
        public string Question { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_Phase")]
        public string Phase { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_SortOrder")]
        [ReadOnly(true)]
        public int SortOrder { get; set; }

        //Answer to the question
        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_NotApplicable")]
        public bool NotApplicable { get; set; }

        [UIHint("EvaluationFormInitialStatusTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_InitialStatus")]
        public string InitialStatus { get; set; }

        [UIHint("EvaluationFormCurrentStatusTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_CurrentStatus")]
        public string CurrentStatus { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_InitialAssessment")]
        [DataType(DataType.MultilineText)]
        public string InitialAssessment { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_CurrentAssessment")]
        [DataType(DataType.MultilineText)]
        public string CurrentAssessment { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationQuestion_Title_Comment")]
        public string Comment { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }
    }
}
