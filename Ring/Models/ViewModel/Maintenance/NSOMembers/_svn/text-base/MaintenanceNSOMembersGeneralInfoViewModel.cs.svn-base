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
    [KnownType(typeof(MaintenanceNSOMembersGeneralInfoViewModel))]
    public class MaintenanceNSOMembersGeneralInfoViewModel
    {
        public bool Editable { get; set; }

        public MaintenanceNSOMembersGeneralInfoViewModel(Resource resource = null)
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
                SpecialtyId = resource.ResourceSpecialtyID;
                Comments = resource.Comments;
                TBD = resource.TBD;
                Pooled = resource.Pooled;
                Active = resource.Active;
            }
        }

        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceNSOMembersLocalization), ErrorMessageResourceName = "Error_Required_FirstName")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceNSOMembersLocalization), ErrorMessageResourceName = "Error_Required_LastName")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_LastName")]
        public string LastName { get; set; }

        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_CountryId")]
        public string CountryId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Gender")]
        public string Gender { get; set; }

        // RAllen - Dec 21 2010 - Modified DateOfBirth from auto property format
        private DateTime? _dateOfBirth;

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_DateOfBirth")]
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

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_PrimaryCSCId")]
        public string PrimaryCSCId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_SecondaryCSCId")]
        public string SecondaryCSCId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_SpecialtyId")]
        public string SpecialtyId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_TBD")]
        public bool TBD { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Pooled")]
        public bool Pooled { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Active")]
        public bool Active { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Comments")]
        public string Comments { get; set; }

        public List<ListItem> Countries { get; set; }

        public List<ListItem> Genders { get; set; }

        public List<ListItem> CSCs { get; set; }

        public List<ListItem> Specialties { get; set; }

    }
}