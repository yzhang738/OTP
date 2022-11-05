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
    [KnownType(typeof(MaintenanceNSOMembersGridViewModel))]
    public class MaintenanceNSOMembersGridViewModel
    {
        [ReadOnly(true)]
        [ExportToExcel(CellLength = 50)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceIdShort")]
        public int ResourceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceNSOMembersLocalization), ErrorMessageResourceName = "Error_Required_LastName")]
        [ExportToExcel(CellLength = 130)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceNSOMembersLocalization), ErrorMessageResourceName = "Error_Required_FirstName")]
        [ExportToExcel(CellLength = 130)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_CountryId")]
        public string CountryId { get; set; }

        [UIHint("NSOMembersGridCountryTemplate")]
        [ExportToExcel(CellLength = 100, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_CountryName")]
        public string CountryName { get; set; }

        [UIHint("NSOMembersGridBirthDateTemplate")]
        [ExportToExcel(CellLength = 60)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DateOfBirth")]
        public string BirthDate { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Pooled")]
        public bool Pooled { get; set; }

        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_TBD")]
        public bool TBD { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_GenderId")]
        public string GenderId { get; set; }

        [UIHint("NSOMembersGridGenderTemplate")]
        [ExportToExcel(CellLength = 60)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_GenderName")]
        public string GenderName { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceSpecialtyId")]
        public string ResourceSpecialtyId { get; set; }

        [UIHint("NSOMembersGridSpecialtyTemplate")]
        [ExportToExcel(CellLength = 150)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceSpecialtyName")]
        public string ResourceSpecialtyName { get; set; }

        public string AssignSportId { get; set; }

    }
}