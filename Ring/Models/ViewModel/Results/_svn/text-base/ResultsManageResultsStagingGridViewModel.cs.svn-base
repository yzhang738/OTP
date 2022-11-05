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
    [KnownType(typeof(ResultsManageResultsStagingGridViewModel))]
    public class ResultsManageResultsStagingGridViewModel
    {
        //TODO: Move localization from CommonLocalization to ResultsLocalization
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_Id")]
        public int ResultStagingId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_DownloadSourceType")]
        public string DownloadSourceType { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_DownloadTempalteId")]
        public int? DownloadTemplateId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_DownloadSourceId")]
        public int DownloadSourceId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_StatusCode")]
        public string StatusCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_StatusDesc")]
        public string StatusDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_Approved")]
        public bool Approved { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_DisciplineId")]
        public string DisciplineId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_DisciplineDesc")]
        public string DisciplineDesc { get; set; }

        [UIHint("ManageResultsEventTemplate")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_EventId")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_EventId")]
        public string EventId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_EventDesc")]
        public string EventDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_EventDescription")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_EventDescription")]
        public string EventDescription { get; set; }

        // RAllen - Dec 21 2010 - Modified EventDate from auto property format
        private DateTime _eventDate;

        [UIHint("ManageResultsEventDateTemplate")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_EventDate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_EventDate")]
        public DateTime EventDate
        {
            get
            {
                return DateTime.SpecifyKind(new DateTime(_eventDate.Year, _eventDate.Month, _eventDate.Day), DateTimeKind.Utc);
            }
            set
            {
                _eventDate = DateTime.SpecifyKind(new DateTime(value.Year, value.Month, value.Day), DateTimeKind.Utc);
            }
        }

        public string EventDateLiteral
        {
            get
            {
                return EventDate.ToShortDateString();
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_CompetitionId")]
        public int CompetitionId { get; set; }

        [UIHint("ManageResultsBenchmarkTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_BenchmarkTypeId")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_BenchmarkTypeId")]
        public int BenchmarkTypeId { get; set; }

        public string BenchmarkTypeName { get; set; }

        public int SourceRow { get; set; }

        [UIHint("ManageResultsCountryTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_CountryCode")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_CountryCode")]
        public string CountryCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_CountryDesc")]
        public string CountryDesc { get; set; }

        [UIHint("ManageResultsPositionTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_Position")]
        public int Position { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_PositionDesc")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_PositionDesc")]
        public string PositionDesc { get; set; }

        [UIHint("ManageResultsPerformanceTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_PerfValue")]
        public decimal? PerfValue { get; set; }

        [DataType(DataType.Time)]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_PerfTimeDesc")]
        public string PerfTimeDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_PerfTime")]
        public TimeSpan? PerfTime { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_TeamId")]
        public string TeamName { get; set; }

        public bool HasComments { get; set; }

        public bool TeamSport { get; set; }
    }
}