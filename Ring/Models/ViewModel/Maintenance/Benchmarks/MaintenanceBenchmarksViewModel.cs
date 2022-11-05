using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data;
using OTP.Ring.Data.Models;


namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class MaintenanceBenchmarksViewModel : BaseViewModel
    {
        public YearSelectionViewModel CurrentOlympicYears { get; set; }
        public string SelectedSport { get; set; }

        public MaintenanceBenchmarksInsertBenchmarkTierViewModel InsertBenchmarkTier { get; set; }
    }
}