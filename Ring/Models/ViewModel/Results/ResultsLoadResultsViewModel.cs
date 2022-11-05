using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using OTP.Ring.Data.Models;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class ResultsLoadResultsViewModel : BaseViewModel
    {
        public List<ListItem> DownloadTemplateList { get; set; }
        public Common.Competition Competition { get; set; }
        public List<ListItem> EventList { get; set; }
        public List<ListItem> BenchmarkList { get; set; }
        public int BenchmarkSelectedIndex { get; set; }
        public YearSelectionViewModel CurrentOlympicYears { get; set; }
        public List<ResultsLoadResultsErrorGridViewModel> ErrorGridViewModel { get; set; }
        public string Errors { get; set; }
    }
}