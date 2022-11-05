using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class YearSelectionViewModel
    {
        public int SelectedYearIndex { get; set; }
        public List<string> OlympicYears { get; set; }

        public YearSelectionViewModel(List<string> olympicYears = null, int selectedYearIndex = 0)
        {
            OlympicYears = olympicYears == null ? new List<string>() : olympicYears;
            SelectedYearIndex = selectedYearIndex;
        }
    }
}