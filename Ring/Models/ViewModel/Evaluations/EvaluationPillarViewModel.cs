using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class EvaluationPillarViewModel
    {
        public int EvaluationId { get; set; }
        public string SportId { get; set; }
        public string ActionItemRatio { get; set; }

        public EvaluationPillar Pillar { get; set; }

        public bool Editable { get; set; }
        public bool InitialStatusEditable { get; set; }
        public bool CurrentStatusVisible { get; set; }
        public bool HasActionItem { get; set; }
        public bool CanEditSportQuestions { get; set; }
    }
}
