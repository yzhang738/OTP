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
    [KnownType(typeof(ReportingDesignAnalysisFiltersGridViewModel))]
    public class ReportingDesignAnalysisFiltersGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportFilterId")]
        public int ReportFilterId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportId")]
        public int ReportId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_SortOrder")]
        public int SortOrder { get; set; }

        [UIHint("DesignAnalysisReportFilterFieldName_ENTemplate")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_FilterFieldName_EN")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FilterFieldName_EN")]
        public string FilterFieldName_EN { get; set; }

        [UIHint("DesignAnalysisReportFilterFieldName_FRTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FilterFieldName_FR")]
        public string FilterFieldName_FR { get; set; }

        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_FilterName_EN")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FilterName_EN")]
        public string FilterName_EN { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FilterName_FR")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_FilterName_FR")]
        public string FilterName_FR { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_Display")]
        [UIHint("DesignAnalysisReportFilterDisplayTemplate")]
        public bool Display { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_Default_EN")]
        public string Default_EN { get; set; }

        [UIHint("DesignAnalysisReportFilterOperandTemplate")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_ReportOperand")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportOperand")]
        public string ReportOperand { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

        public bool Editable { get; set; }
    }
}