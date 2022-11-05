using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OTP.Ring.Data.Models;
using System.Collections.Generic;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class EvaluationsQuestionsViewModel : BaseViewModel
    {
        public bool Editable { get; set; }

        public YearSelectionViewModel CurrentOlympicYears { get; set; }
        public List<EvaluationsQuestionsEvaluationVersionGridViewModel> VersionsGridModel { get; set; }
    }
}