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
    [KnownType(typeof(ResultsLoadTemplateDownloadTemplateGridViewModel))]
    public class ResultsLoadTemplateDownloadTemplateGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_DownloadTemplateId")]
        public int DownloadTemplateId { get; set; }

        [UIHint("LoadTemplatesTableTemplateTemplate")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_TableTemplateId")]
        //[Required(ErrorMessageResourceType = typeof(ResultsLocalization), ErrorMessageResourceName = "Error_Required_TableTemplateId")]
        public string TableTemplateId { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_TableTemplateName")]
        public string TableTemplateName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResultsLocalization), ErrorMessageResourceName = "Error_Required_Name_EN")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_Name_EN")]
        public string Name_EN { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResultsLocalization), ErrorMessageResourceName = "Error_Required_Name_FR")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_Name_FR")]
        public string Name_FR { get; set; }

        public string Name { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_Instructions")]
        [DataType(DataType.MultilineText)]
        public string Instructions { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_URL")]
        public string URL { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [UIHint("NullableBoolean")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_TeamSport")]
        public bool? TeamSport { get; set; }

        [UIHint("NullableBoolean")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_ResourceTracking")]
        public bool? ResourceTracking { get; set; }
    }
}