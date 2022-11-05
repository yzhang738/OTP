using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTP.Ring.Data.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [Serializable]
    [KnownType(typeof(MaintenanceAthletesDirectAthleteSupportViewModel))]
    public class MaintenanceAthletesDirectAthleteSupportViewModel
    {
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceDirectAthleteSupportId")]
        public int ResourceDirectAthleteSupportId { get; set; }

        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [UIHint("AthletesDirectAthleteSupportSupportTypesTemplate")]
        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_DirectAthleteSupportTypeId")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DirectAthleteSupportTypeId")]
        public string DirectAthleteSupportTypeId { get; set; }

        [ExportToExcel(CellLength = 150)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DirectAthleteSupportTypeName")]
        public string DirectAthleteSupportTypeName { get; set; }

        [UIHint("AthletesDirectAthleteSupportSupportCategoriesTemplate")]
        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_DirectAthleteSupportCode")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DirectAthleteSupportCode")]
        public string DirectAthleteSupportCode { get; set; }

        [ExportToExcel(CellLength = 120)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_DirectAthleteSupportName")]
        public string DirectAthleteSupportName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_YearId")]
        [ExportToExcel(CellLength = 70)]
        [UIHint("AthletesDirectAthletesSupportYearsTemplate")]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_YearId")]
        public string YearId { get; set; }

        [Required(ErrorMessageResourceType = typeof(MaintenanceAthletesLocalization), ErrorMessageResourceName = "Error_Required_SupportName")]
        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SupportName")]
        public string SupportName { get; set; }

        [ExportToExcel(CellLength = 50)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Amount")]
        public decimal? Amount { get; set; }

        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_Description")]
        public string Description { get; set; }

        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SupportContact")]
        public string SupportContact { get; set; }

        [ExportToExcel(CellLength = 80)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_SupportEmail")]
        public string SupportEmail { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceAthletesLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

    }
}