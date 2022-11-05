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
    [KnownType(typeof(ResultManageResultsStagingResourceGridViewModel))]
    public class ResultManageResultsStagingResourceGridViewModel
    {
        //TODO: Move localization from CommonLocalization to ResultsLocalization
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_Id")]
        public int ResultStagingResourceId { get; set; }

        public int ResultStagingId { get; set; }

        public int SourceRow { get; set; }
        
        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_TeamMember")]
        public int TeamMember { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_SportTrackingRef")]
        public string SportTrackingRef { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_DownloadLastName")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_DownloadLastName")]
        public string DownloadLastName { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_DownloadFirstName")]
        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "ResultStaging_Required_DownloadFirstName")]
        public string DownloadFirstName { get; set; }

        [UIHint("ManageResultsGenderTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_GenderCode")]
        public string GenderCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_GenderDesc")]
        public string GenderDesc { get; set; }

        // RAllen - Dec 21 2010 - Modified BirthDate from auto property format
        private DateTime? _birthDate;

        [UIHint("ManageResultsBirthDateTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "ResultStaging_Title_BirthDate")]
        public DateTime? BirthDate
        {
            get
            {
                if (_birthDate == null)
                {
                    return _birthDate;
                }
                else
                {
                    var dob = (DateTime)_birthDate;
                    return DateTime.SpecifyKind(new DateTime(dob.Year, dob.Month, dob.Day), DateTimeKind.Utc);
                }
            }
            set
            {
                _birthDate = value;
                if (value != null)
                {
                    var dob = (DateTime)value;
                    _birthDate = DateTime.SpecifyKind(new DateTime(dob.Year, dob.Month, dob.Day), DateTimeKind.Utc);
                }
            }
        }

        public string BirthDateLiteral
        {
            get
            {
                return _birthDate == null ? "" : ((DateTime)BirthDate).ToShortDateString();
            }
        }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_ResourceId")]
        [UIHint("ManageResultsStagingResourceNameTemplate")]
        public int? ResourceId { get; set; }

        public string ResourceName { get; set; }

        [ReadOnly(true)]
        public bool HasComments { get; set; }
    }
}
