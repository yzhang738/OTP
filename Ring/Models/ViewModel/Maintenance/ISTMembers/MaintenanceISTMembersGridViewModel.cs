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
    [KnownType(typeof(MaintenanceISTMembersGridViewModel))]
    public class MaintenanceISTMembersGridViewModel
    {
        [ReadOnly(true)]
        [ExportToExcel(CellLength = 50)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ID")]
        public int ResourceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceISTMembersLocalization), ErrorMessageResourceName = "Error_Required_LastName")]
        [ExportToExcel(CellLength = 120)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceISTMembersLocalization), ErrorMessageResourceName = "Error_Required_FirstName")]
        [ExportToExcel(CellLength = 130)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_CountryId")]
        public string CountryId { get; set; }

        [UIHint("ISTMembersGridCountryTemplate")]
        [ExportToExcel(CellLength = 100, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_CountryName")]
        public string CountryName { get; set; }

        [UIHint("ISTMembersGridBirthDateTemplate")]
        [ExportToExcel(CellLength = 60)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_DateOfBirth")]
        public string BirthDate { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_GenderId")]
        public string GenderId { get; set; }

        [UIHint("ISTMembersGridGenderTemplate")]
        [ExportToExcel(CellLength = 60)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_GenderName")]
        public string GenderName { get; set; }

        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_Pooled")]
        public bool Pooled { get; set; }

        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_TBD")]
        public bool TBD { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_PrimaryCscIdLong")]
        public string PrimaryCscId { get; set; }

        [UIHint("ISTMembersGridPrimaryCscTemplate")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_PrimaryCscName")]
        public string PrimaryCscName { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceSpecialtyId")]
        public string ResourceSpecialtyId { get; set; }

        [UIHint("ISTMembersGridSpecialtyTemplate")]
        [ExportToExcel(CellLength = 150)]
        [LocalizedDisplayName(typeof(MaintenanceISTMembersLocalization), "Title_ResourceSpecialtyName")]
        public string ResourceSpecialtyName { get; set; }

        public string AssignSportId { get; set; }

    }
}