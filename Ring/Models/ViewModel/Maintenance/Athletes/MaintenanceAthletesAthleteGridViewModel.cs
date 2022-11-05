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
    [KnownType(typeof(MaintenanceAthletesAthleteGridViewModel))]
    public class MaintenanceAthletesAthleteGridViewModel
    {
        [ReadOnly(true)]
        [ExportToExcel(CellLength = 50)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceIdShort")]
        public int ResourceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_LastName")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_FirstName")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CountryId")]
        public string CountryId { get; set; }

        [UIHint("AthletesGridCountryTemplate")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_CountryName")]
        public string CountryName { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_GenderId")]
        public string GenderId { get; set; }

        [UIHint("AthletesGridGenderTemplate")]
        [ExportToExcel(CellLength = 60)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_GenderName")]
        public string GenderName { get; set; }

        [UIHint("AthletesGridBirthDateTemplate")]
        [ExportToExcel(CellLength = 70)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DateOfBirth")]
        public string BirthDate { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_TBD")]
        public bool TBD { get; set; }

        //[ReadOnly(true)]
        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_IsNew")]
        public bool IsNew { get; set; }

        [ReadOnly(true)]
        [ExportToExcel(CellLength = 60, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_BenchmarksEntered")]
        public bool BenchmarksEntered { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Pooled")]
        public bool Pooled { get; set; }

        [ReadOnly(true)]
        //[UIHint("AthletesGridCategoryTemplate")]
        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Category")]
        public string Category { get; set; }

        [UIHint("AthletesGridSpecialtyTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceSpecialtyId")]
        public string ResourceSpecialtyId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceSpecialtyName")]
        public string ResourceSpecialtyName { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_PersonalCoachId")]
        public int? PersonalCoachId { get; set; }

        [UIHint("AthletesGridCoachResourcesTemplate")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_PersonalCoachName")]
        public string PersonalCoachName { get; set; }

        public string CoachFirstName { get; set; }

        public string CoachLastName { get; set; }

        public string AssignSportId { get; set; }
    }
}