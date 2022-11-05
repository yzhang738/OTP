using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using OTP.Ring.Data.Models;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [KnownType(typeof(ReportingDesignAnalysisSecurityGridViewModel))]
    public class ReportingDesignAnalysisSecurityGridViewModel
    {
        [ReadOnly(true)]
        public int ReportSecurityGroupProfileID { get; set; }

        [ReadOnly(true)]
        public int ReportId { get; set; }

        [UIHint("DesignAnalysisReportSecuritySecurityGroupTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_SecurityGroupProfileID")]
        public string SecurityGroupProfileID { get; set; }

        public string SecurityGroupProfileName { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

        public bool Editable { get; set; }
    }
}