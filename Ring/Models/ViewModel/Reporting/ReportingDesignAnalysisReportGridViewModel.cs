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
    [KnownType(typeof(ReportingDesignAnalysisReportGridViewModel))]
    public class ReportingDesignAnalysisReportGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportId")]
        public int ReportId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_Name_EN")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_Name_EN")]
        [RegularExpression(@"^[^<>%$*?:/'""]*$", ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Regex_Name_EN")]
        public string Name_EN { get; set; }

        [Required(ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Required_Name_FR")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_Name_FR")]
        [RegularExpression(@"^[^<>%$*?:/'""]*$", ErrorMessageResourceType = typeof(ReportingLocalization), ErrorMessageResourceName = "Error_Regex_Name_EN")]
        public string Name_FR { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_Description_EN")]
        public string Description_EN { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_Description_FR")]
        public string Description_FR { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_Locked")]
        public bool Locked { get; set; }

        [UIHint("DesignAnalysisReportSportTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_SportId")]
        public string SportId { get; set; }

        public string SportName { get; set; }

        [UIHint("DesignAnalysisReportCategoryTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportCategoryId")]
        public string ReportCategoryId { get; set; }

        public string ReportCategoryName { get; set; }

        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_SecurityUserSport")]
        public bool SecurityUserSport { get; set; }

        public string SecurityUserProfileId { get; set; }

        [UIHint("DesignAnalysisReportViewTemplate")]
        [LocalizedDisplayName(typeof(ReportingLocalization), "Title_ReportViewId")]
        public string ReportViewId { get; set; }

        public string ReportViewName { get; set; }

        public bool HasComments { get; set; }

        public bool Editable { get; set; }
    }
}