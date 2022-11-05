using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data;
using OTP.Ring.Data.Models;
using Microsoft.Reporting.WebForms;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    public class ReportingReportDataViewModel
    {
        public int ReportId { get; set; }
        public string RDLCReference { get; set; }
        public string ReportDefinition { get; set; }
        public string DisplayName { get; set; }

        public List<ReportDataSource> DataSources { get; set; }

        public ReportingReportDataViewModel()
        {
            DataSources = new List<ReportDataSource>();
        }
    }
}