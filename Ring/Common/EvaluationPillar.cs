﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTP.Ring.Common
{
    [Serializable]
    public class EvaluationPillar
    {
        public class Comparer : IComparer<EvaluationPillar>
        {
            public int Compare(EvaluationPillar x, EvaluationPillar y)
            {
                return x.SortOrder.CompareTo(y.SortOrder);
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationPillar_Title_Id")]
        public string Id { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationPillar_Title_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationPillar_Title_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationPillar_Title_SortOrder")]
        public int SortOrder { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "EvaluationPillar_Title_Comment")]
        public string Comment { get; set; }

        public string Id_Name { get; set; }

        public IEnumerable<EvaluationQuestion> Questions { get; set; }
    }
}
