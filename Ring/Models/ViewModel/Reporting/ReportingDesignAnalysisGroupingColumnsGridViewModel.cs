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
    [KnownType(typeof(ReportingDesignAnalysisGroupingColumnsGridViewModel))]
    public class ReportingDesignAnalysisGroupingColumnsGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportFilterId")]
        public int ReportColumnId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportGroupId")]
        public int ReportGroupId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportColumnLevel")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_ReportColumnLevel")]
        public int ReportColumnLevel { get; set; }

        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_ColumnDisplayName_EN")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ColumnDisplayName_EN")]
        public string ColumnDisplayName_EN { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ColumnDisplayName_FR")]
        public string ColumnDisplayName_FR { get; set; }

        [UIHint("DesignAnalysisReportGroupColumnFieldName_ENTemplate")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_ColumnFieldName_EN")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ColumnFieldName_EN")]
        public string ColumnFieldName_EN { get; set; }

        [UIHint("DesignAnalysisReportGroupColumnFieldName_FRTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ColumnFieldName_FR")]
        public string ColumnFieldName_FR { get; set; }

        [UIHint("DesignAnalysisReportGroupColumnAggregationTemplate")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_ReportAggregation")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportAggregation")]
        public string ReportAggregation { get; set; }

        public string ReportAggregationName { get; set; }

        [UIHint("DesignAnalysisReportGroupColumnFormatStringTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FormatString")]
        public string FormatString { get; set; }

        public string FormatStringName { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_FieldSize")]
        [UIHint("DesignAnalysisReportGroupColumnFieldSizeTemplate")]
        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_FiledSize")]
        public string FieldSize { get; set; }

        public string FieldSizeName { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }

        public bool Editable { get; set; }
    }
}