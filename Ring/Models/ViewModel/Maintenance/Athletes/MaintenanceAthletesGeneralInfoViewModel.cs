using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [KnownType(typeof(MaintenanceAthletesGeneralInfoViewModel))]
    public class MaintenanceAthletesGeneralInfoViewModel
    {
        public bool Editable { get; set; }

        public MaintenanceAthletesGeneralInfoViewModel(Resource resource = null)
        {
            if (resource != null)
            {
                ResourceId = resource.ResourceID;
                FirstName = resource.FirstName;
                LastName = resource.LastName;
                CountryId = resource.CountryID;
                Gender = resource.Gender;
                DateOfBirth = resource.DateOfBirth;
                PrimaryCSCId = resource.PrimaryCSCID;
                SecondaryCSCId = resource.SecondaryCSCID;
                Comments = resource.Comments;
                TBD = resource.TBD;
                Pooled = resource.Pooled;
                Active = resource.Active;
                IsNew = resource.New;
            }
        }

        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_FirstName")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_LastName")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_LastName")]
        public string LastName { get; set; }

        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CountryId")]
        public string CountryId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Gender")]
        public string Gender { get; set; }

        // RAllen - Dec 21 2010 - Modified DateOfBirth from auto property format
        private DateTime? _dateOfBirth;

        //[DisplayName("Date of Birth")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DateOfBirth")]
        public DateTime? DateOfBirth
        {
            get
            {
                if (_dateOfBirth == null)
                {
                    return _dateOfBirth;
                }
                else
                {
                    var dob = (DateTime)_dateOfBirth;
                    return DateTime.SpecifyKind(new DateTime(dob.Year, dob.Month, dob.Day), DateTimeKind.Utc);
                }
            }
            set
            {
                _dateOfBirth = value;
                if (value != null)
                {
                    var dob = (DateTime)value;
                    _dateOfBirth = DateTime.SpecifyKind(new DateTime(dob.Year, dob.Month, dob.Day), DateTimeKind.Utc);
                }
            }
        }

        public string DateOfBirthLiteral
        {
            get
            {
                return _dateOfBirth == null ? "" : ((DateTime)DateOfBirth).ToShortDateString();
            }
        }

        //[DisplayName("Primary")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_PrimaryCSCId")]
        public string PrimaryCSCId { get; set; }

        //[DisplayName("Secondary")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SecondaryCSCId")]
        public string SecondaryCSCId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_TBD")]
        public bool TBD { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Pooled")]
        public bool Pooled { get; set; }

        //[DisplayName("New Athlete")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_IsNewAthlete")]
        public bool IsNew { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Active")]
        public bool Active { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Comments")]
        public string Comments { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_PersonalCoachId")]
        public int? PersonalCoachId { get; set; }

        public List<ListItem> Countries { get; set; }

        public List<ListItem> Genders { get; set; }

        public List<ListItem> CSCs { get; set; }

        public List<ListItem> CoachResources { get; set; }

    }
}