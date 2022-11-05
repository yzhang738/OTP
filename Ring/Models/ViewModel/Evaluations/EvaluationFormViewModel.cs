using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;


namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class EvaluationFormViewModel : BaseViewModel
    {
        public Evaluation Evaluation { get; set; }

        public string SportTitle { get; set; }
        public string EvaluationTitle { get; set; }

        public int SelectedTabIndex { get; set; }
        public List<string> ToolbarButtons { get; set; }
        public List<EvaluationStatusChangeViewModel> ChangeToStatus { get; set; }

        public bool Editable { get; set; }
        public bool CanEditSportQuestions { get; set; }
    }
}
