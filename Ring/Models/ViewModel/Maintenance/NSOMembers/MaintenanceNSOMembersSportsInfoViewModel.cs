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
    [KnownType(typeof(MaintenanceNSOMembersSportsInfoViewModel))]
    public class MaintenanceNSOMembersSportsInfoViewModel
    {
        [Editable(false)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceId")]
        public int ResourceId { get; set; }

        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_ResourceSportId")]
        public int ResourceSportId { get; set; }

        [UIHint("NSOMembersSportsInfoAllSportsTemplate")]
        [Required(ErrorMessageResourceType = typeof(MaintenanceNSOMembersLocalization), ErrorMessageResourceName = "Error_Required_SportsId")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_SportsId")]
        public string SportsId { get; set; }

        [ExportToExcel(CellLength = 100)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_SportName")]
        public string SportName { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_YearId")]
        public string YearId { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_HasComments")]
        public bool HasComments { get; set; }

        [UIHint("NullableBoolean")]
        [ExportToExcel(CellLength = 80, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_CrossCompetency")]
        public bool? CrossCompetency { get; set; }

        [UIHint("NullableBoolean")]
        [ExportToExcel(CellLength = 80, CellAlignment = "CENTER")]
        [LocalizedDisplayName(typeof(MaintenanceNSOMembersLocalization), "Title_Primary")]
        public bool? Primary { get; set; }
    }
}