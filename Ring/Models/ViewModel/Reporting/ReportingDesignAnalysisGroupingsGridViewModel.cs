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
    [KnownType(typeof(ReportingDesignAnalysisGroupingsGridViewModel))]
    public class ReportingDesignAnalysisGroupingsGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportGroupId")]
        public int ReportGroupId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportId")]
        public int ReportId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportGroupLevel")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_ReportGroupLevel")]
        public int ReportGroupLevel { get; set; }

        [UIHint("DesignAnalysisReportGroupingFieldName_ENTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FieldName_EN")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_FieldName_EN")]
        public string FieldName_EN { get; set; }

        [UIHint("DesignAnalysisReportGroupingFieldName_FRTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FieldName_FR")]
        public string FieldName_FR { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

        public bool Editable { get; set; }
    }
}