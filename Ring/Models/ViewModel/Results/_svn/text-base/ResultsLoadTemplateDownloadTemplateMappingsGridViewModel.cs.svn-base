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
    [KnownType(typeof(ResultsLoadTemplateDownloadTemplateMappingsGridViewModel))]
    public class ResultsLoadTemplateDownloadTemplateMappingsGridViewModel
    {
        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_DownloadTemplateMappingId")]
        public int DownloadTemplateMappingId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_DownloadTemplateId")]
        public int DownloadTemplateId { get; set; }

        [UIHint("LoadTemplatesFieldNameTemplate")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_FieldName")]
        [Required(ErrorMessageResourceType = typeof(ResultsLocalization), ErrorMessageResourceName = "Error_Required_FieldName")]
        public string FieldName { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_BusinessKey")]
        public bool BusinessKey { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_ReplaceKey")]
        public bool ReplaceKey { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_Required")]
        public bool Required { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_Detail")]
        public bool Detail { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_TeamMember")]
        public int TeamMember { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SourceColumnName")]
        public string SourceColumnName { get; set; }

        [UIHint("LoadTemplatesSourceColumnTemplate")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SourceColumn")]
        public string SourceColumn { get; set; }

        [UIHint("LoadTemplatesSourceRowOffsetTemplate")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SourceRow")]
        public int SourceRowOffset { get; set; }

        [UIHint("LoadTemplatesColumnFormatTemplate")]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_ColumnFormat")]
        public string ColumnFormat { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SourceDefault")]
        public string SourceDefault { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_RequiredFormatType")]
        public string RequiredFormatType { get; set; }
    }
}