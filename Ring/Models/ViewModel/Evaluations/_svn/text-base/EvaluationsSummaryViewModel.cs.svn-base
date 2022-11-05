using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class EvaluationsSummaryViewModel : BaseViewModel
    {
        public YearSelectionViewModel CurrentOlympicYears { get; set; }
        public List<EvaluationsSummaryGridViewModel> Evaluations { get; set; }
		public string Year { get; set; }
        
        public int SelectedEvaluationVersionIndex {get; set;}
        public List<ListItem> EvaluationVersions { get; set; }
        public bool EvaluationVersionEditable { get; set; }
    }
}
