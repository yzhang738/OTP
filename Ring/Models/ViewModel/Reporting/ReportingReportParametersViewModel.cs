using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data;
using OTP.Ring.Data.Models;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class ReportingReportParametersViewModel
    {
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Locked { get; set; }
        public string SportId { get; set; }
        public string Comments { get; set; }
        public string ReportCategory { get; set; }
        public bool SecurityUserSport { get; set; }
        public string SecurityUserProfileID { get; set; }
        public string ReportViewId { get; set; }
        public string ReportType { get; set; }

        public List<ReportFilters> Filters { get; set; }

        public ReportingReportParametersViewModel()
        {
            Filters = new List<ReportFilters>();
        }
    }

    public class ReportFilters
    {
        public int ReportProcedureId { get; set; }
        public string Label { get; set; }
        public string StoredProcedureName { get; set; }
        public int SortOrder { get; set; }
        public List<int> ParentReportProcedureId { get; set; }
        public List<int> ChildReportProcedureId { get; set; }
        //public int ChildReportProcedureId { get; set; }
        public bool Display { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultID { get; set; }
    }
}